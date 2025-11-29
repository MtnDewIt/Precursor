using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.Cache.Gen1;
using TagTool.Cache.Gen2;
using TagTool.Cache.Gen3;
using TagTool.Cache.Gen4;
using TagTool.Cache.HaloOnline;
using TagTool.Cache.Monolithic;
using TagTool.Common;
using TagTool.Geometry.BspCollisionGeometry;
using TagTool.IO;
using TagTool.Serialization;
using TagTool.Shaders;
using TagTool.Tags;
using TagGroup = TagTool.Common.Tag;

namespace PrecursorShell.Serialization
{
    public class Deserializer
    {
        private readonly CacheVersion Version;
        private readonly CachePlatform Platform;
        private readonly int TagBlockSize;
        private readonly int TagDataSize;
        private readonly TagStructure.VersionedCache StructCache;
        private readonly TagEnum.VersionedCache EnumCache;
        private readonly Stack<string> PathStack;

        private string CurrentFieldPath => string.Join(".", PathStack.Reverse());

        public readonly List<string> Problems;

        public Deserializer(CacheVersion version, CachePlatform platform) 
        {
            Version = version;
            Platform = platform;
            TagBlockSize = CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0x8 : 0xC;
            TagDataSize = CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0x8 : 0x14;
            StructCache = TagStructure.GetVersonedCache(version, platform);
            EnumCache = TagEnum.GetVersonedCache(version, platform);
            PathStack = new Stack<string>();
            Problems = new List<string>();
        }

        public Blf DeserializeBlf(Stream stream) 
        {
            if (Version == CacheVersion.HaloOnlineED) 
            {
                var isLittleEndian = CacheVersionDetection.IsLittleEndian(Version, Platform);

                var contextReader = new EndianReader(stream, isLittleEndian ? EndianFormat.LittleEndian : EndianFormat.BigEndian);

                var blf = new Blf(Version, Platform);

                if (blf.Read(contextReader))
                    return blf;
            }

            return null;
        }

        public CacheFileReports DeserializeCacheFileReports(Stream stream, CacheFileHeader header) 
        {
            if (CacheVersionDetection.IsInGen(CacheGeneration.HaloOnline, Version) && Version != CacheVersion.HaloOnlineED) 
            {
                var isLittleEndian = CacheVersionDetection.IsLittleEndian(Version, Platform);

                var contextReader = new EndianReader(stream, isLittleEndian ? EndianFormat.LittleEndian : EndianFormat.BigEndian);

                var context = new DataSerializationContext(contextReader);

                var reports = new CacheFileReports(Version);

                var reportSize = (int)TagStructure.GetTagStructureInfo(typeof(CacheFileReports.CacheFileReport), Version, CachePlatform.Original).TotalSize;

                reports.Count = header.GetReports().Size / reportSize;

                reports.Reports = new CacheFileReports.CacheFileReport[reports.Count];

                for (int i = 0; i < reports.Count; i++)
                {
                    var info = TagStructure.GetTagStructureInfo(typeof(CacheFileReports.CacheFileReport), Version, Platform);

                    var reader = context.BeginDeserialize(info);

                    if (reader.Length == 0)
                        return null;

                    var result = DeserializeObjectStruct(null, reader, context, info);

                    context.EndDeserialize(info, result);

                    reports.Reports[i] = result as CacheFileReports.CacheFileReport;
                }

                return reports;
            }

            return null;
        }

        public object DeserializeTagInstance(GameCache cache, Stream stream, Type type, CachedTag instance)
        {
            ISerializationContext context = null;

            switch (cache) 
            {
                case GameCacheGen1 gameCacheGen1:
                    context = new Gen1SerializationContext(stream, gameCacheGen1, (CachedTagGen1)instance);
                    break;
                case GameCacheGen2 gameCacheGen2:
                    context = new Gen2SerializationContext(stream, gameCacheGen2, (CachedTagGen2)instance);
                    break;
                case GameCacheGen3 gameCacheGen3:
                    context = new Gen3SerializationContext(stream, gameCacheGen3, (CachedTagGen3)instance);
                    break;
                case GameCacheMonolithic gameCacheMonolithic:
                    context = new TagSerializationContextMonolithic(stream, gameCacheMonolithic, (CachedTagMonolithic)instance);
                    break;
                case GameCacheHaloOnlineBase gameCacheHaloOnline:
                    context = new HaloOnlineSerializationContext(stream, gameCacheHaloOnline, (CachedTagHaloOnline)instance);
                    break;
                case GameCacheGen4 gameCacheGen4:
                    context = new Gen4SerializationContext(stream, gameCacheGen4, (CachedTagGen4)instance);
                    break;
            }

            var info = ObjectStructure.GetObjectStructureInfo(type, Version, Platform);

            var reader = context.BeginDeserialize(info);

            if (reader.Length == 0)
                return null;

            var result = DeserializeObjectStruct(cache, reader, context, info);

            context.EndDeserialize(info, result);

            return result;
        }

        public object DeserializeStructure(Stream stream, Type type) 
        {
            var isLittleEndian = CacheVersionDetection.IsLittleEndian(Version, Platform);

            var contextReader = new EndianReader(stream, isLittleEndian ? EndianFormat.LittleEndian : EndianFormat.BigEndian);

            var context = new DataSerializationContext(contextReader);

            var info = TagStructure.GetTagStructureInfo(type, Version, Platform);

            var reader = context.BeginDeserialize(info);

            if (reader.Length == 0)
                return null;

            var result = DeserializeObjectStruct(null, reader, context, info);

            context.EndDeserialize(info, result);

            return result;
        }

        public object DeserializeObjectStruct(GameCache cache, EndianReader reader, ISerializationContext context, TagStructureInfo info) 
        {
            var baseOffset = reader.BaseStream.Position;
            var instance = info.CreateInstance();

            foreach (var tagFieldInfo in info.TagFields)
                DeserializeObjectProperty(cache, reader, context, instance, tagFieldInfo, baseOffset);

            if (info.TotalSize > 0)
                reader.BaseStream.Position = baseOffset + info.TotalSize;

            return instance;
        }

        public void DeserializeObjectProperty(GameCache cache, EndianReader reader, ISerializationContext context, object instance, TagFieldInfo tagFieldInfo, long baseOffset)
        {
            var attr = tagFieldInfo.Attribute;

            if ((attr.Flags & TagFieldFlags.Runtime) != 0)
                return;

            uint align = TagFieldInfo.GetFieldAlignment(tagFieldInfo.FieldType, tagFieldInfo.Attribute, Version, Platform);
            if (align > 0)
            {
                var fieldOffset = (uint)(reader.BaseStream.Position - baseOffset);
                reader.BaseStream.Position += -fieldOffset & (align - 1);
            }

            if ((attr.Flags & TagFieldFlags.Padding) != 0)
            {
                DeserializeObjectPadding(reader, tagFieldInfo);
            }
            else
            {
                if (tagFieldInfo.FieldType.IsPrimitive)
                {
                    if (DeserializeObjectPrimitiveProperty(reader, context, attr, tagFieldInfo, instance))
                        return;
                }

                PathStack.Push(tagFieldInfo.FieldInfo.Name);

                var value = DeserializeObjectValue(cache, reader, context, attr, tagFieldInfo.FieldType);
                tagFieldInfo.SetValue(instance, value);

                PathStack.Pop();
            }
        }

        private static bool DeserializeObjectPrimitiveProperty(EndianReader reader, ISerializationContext context, TagFieldAttribute attr, TagFieldInfo tagFieldInfo, object instance)
        {
            switch (Type.GetTypeCode(tagFieldInfo.FieldType))
            {
                case TypeCode.Boolean:
                    tagFieldInfo.SetValueTyped(instance, reader.ReadBoolean());
                    break;
                case TypeCode.SByte:
                    tagFieldInfo.SetValueTyped(instance, reader.ReadSByte());
                    break;
                case TypeCode.Byte:
                    tagFieldInfo.SetValueTyped(instance, reader.ReadByte());
                    break;
                case TypeCode.Int16:
                    tagFieldInfo.SetValueTyped(instance, reader.ReadInt16());
                    break;
                case TypeCode.UInt16:
                    tagFieldInfo.SetValueTyped(instance, reader.ReadUInt16());
                    break;
                case TypeCode.Int32:
                    tagFieldInfo.SetValueTyped(instance, reader.ReadInt32());
                    break;
                case TypeCode.UInt32:
                    tagFieldInfo.SetValueTyped(instance, reader.ReadUInt32());
                    break;
                case TypeCode.Int64:
                    tagFieldInfo.SetValueTyped(instance, reader.ReadInt64());
                    break;
                case TypeCode.UInt64:
                    tagFieldInfo.SetValueTyped(instance, reader.ReadUInt64());
                    break;
                case TypeCode.Single:
                    tagFieldInfo.SetValueTyped(instance, reader.ReadSingle());
                    break;
                case TypeCode.Double:
                    tagFieldInfo.SetValueTyped(instance, reader.ReadDouble());
                    break;
                default:
                    return false;
            }

            return true;
        }

        private void DeserializeObjectPadding(EndianReader reader, TagFieldInfo tagFieldInfo)
        {
            var attr = tagFieldInfo.Attribute;

            //disable padding warnings for gen2 defs
            if (Version <= CacheVersion.Halo2PC)
            {
                reader.BaseStream.Position += attr.Length;
                return;
            }

            if (attr.Length <= 16)
            {
                Span<byte> buffer = stackalloc byte[attr.Length];
                reader.Read(buffer);
                CheckObjectPadding(tagFieldInfo, buffer);
            }
            else
            {
                CheckObjectPadding(tagFieldInfo, reader.ReadBytes(attr.Length));
            }
        }

        private void CheckObjectPadding(TagFieldInfo tagFieldInfo, ReadOnlySpan<byte> bytes)
        {
            int nonZeroIndex = bytes.IndexOfAnyExcept((byte)0);
            if (nonZeroIndex != -1)
                Problems.Add($"Non-zero padding found in {tagFieldInfo.FieldInfo.DeclaringType.FullName}.{tagFieldInfo.FieldInfo.Name} = {bytes[nonZeroIndex]}");
        }

        public object DeserializeObjectValue(GameCache cache, EndianReader reader, ISerializationContext context, TagFieldAttribute valueInfo, Type valueType) 
        {
            if (valueType.IsPrimitive)
                return DeserializeObjectPrimitiveValue(reader, valueType);

            return DeserializeObjectComplexValue(cache, reader, context, valueInfo, valueType);
        }

        public object DeserializeObjectPrimitiveValue(EndianReader reader, Type valueType) 
        {
            return Type.GetTypeCode(valueType) switch
            {
                TypeCode.Single => PrimitiveValueCache.For(DeserializeObjectSingle(reader)),
                TypeCode.Byte => PrimitiveValueCache.For(reader.ReadByte()),
                TypeCode.Int16 => PrimitiveValueCache.For(reader.ReadInt16()),
                TypeCode.Int32 => PrimitiveValueCache.For(reader.ReadInt32()),
                TypeCode.Int64 => PrimitiveValueCache.For(reader.ReadInt64()),
                TypeCode.SByte => PrimitiveValueCache.For(reader.ReadSByte()),
                TypeCode.UInt16 => PrimitiveValueCache.For(reader.ReadUInt16()),
                TypeCode.UInt32 => PrimitiveValueCache.For(reader.ReadUInt32()),
                TypeCode.UInt64 => PrimitiveValueCache.For(reader.ReadUInt64()),
                TypeCode.Boolean => PrimitiveValueCache.For(reader.ReadBoolean()),
                TypeCode.Double => PrimitiveValueCache.For(reader.ReadDouble()),
                _ => throw new ArgumentException("Unsupported type " + valueType.Name),
            };
        }

        private float DeserializeObjectSingle(EndianReader reader) 
        {
            var value = reader.ReadSingle();

            if (float.IsInfinity(value) && float.IsNaN(value)) 
            {
                Problems.Add($"Invalid float value: {CurrentFieldPath} = {value}");
            }

            return value;
        }

        public object DeserializeObjectComplexValue(GameCache cache, EndianReader reader, ISerializationContext context, TagFieldAttribute valueInfo, Type valueType) 
        {
            // Indirect objects
            // TODO: Remove ResourceReference hax, the Indirect flag wasn't available when I generated the tag structures
            if (valueInfo != null && (valueInfo.Flags & TagFieldFlags.Pointer) != 0)
                return DeserializeObjectIndirectValue(cache, reader, context, valueType);

            var compression = TagFieldCompression.None;

            if (valueInfo != null && valueInfo.Compression != TagFieldCompression.None)
                compression = valueInfo.Compression;

            // enum = Enum type
            if (valueType.IsEnum)
                return DeserializeObjectEnum(reader, valueInfo, valueType);

            // string = ASCII string
            if (valueType == typeof(string))
                return DeserializeObjectString(reader, valueInfo);

            if (valueType == typeof(TagGroup))
                return new TagGroup(reader.ReadInt32());

            // TagInstance = Tag reference
            if (valueType == typeof(CachedTag))
                return DeserializeObjectTagReference(cache, reader, context, valueInfo);

            // ResourceAddress = Resource address
            if (valueType == typeof(CacheAddress))
                return new CacheAddress(reader.ReadUInt32());

            // Byte array = Data reference
            // TODO: Allow other types to be in data references, since sometimes they can point to a structure
            if (valueType == typeof(byte[]))
            {
                if (valueInfo.Length == 0)
                    return DeserializeObjectDataReference(reader, context);
                else
                    return reader.ReadBytes(valueInfo.Length);
            }

            if (valueType == typeof(TagData))
                return DeserializeObjectTagData(reader, context);

            // Color types
            if (valueType == typeof(RealRgbColor))
                return new RealRgbColor(reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));
            else if (valueType == typeof(RealArgbColor))
                return new RealArgbColor(reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));
            else if (valueType == typeof(RealRgbaColor))
                return new RealRgbaColor(reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));
            else if (valueType == typeof(ArgbColor))
                return new ArgbColor(reader.ReadUInt32());

            if (valueType == typeof(Int16Point2d))
                return new Int16Point2d(reader.ReadInt16(), reader.ReadInt16());
            if (valueType == typeof(Rectangle2d))
                return new Rectangle2d(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
            if (valueType == typeof(RealRectangle2d))
                return new RealRectangle2d(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            if (valueType == typeof(RealRectangle3d))
                return new RealRectangle3d(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            if (valueType == typeof(RealBoundingBox))
                return new RealBoundingBox(
                    reader.ReadSingle(), reader.ReadSingle(),
                    reader.ReadSingle(), reader.ReadSingle(),
                    reader.ReadSingle(), reader.ReadSingle());

            if (valueType == typeof(RealEulerAngles2d))
            {
                var i = Angle.FromRadians(reader.ReadSingle(compression));
                var j = Angle.FromRadians(reader.ReadSingle(compression));
                return new RealEulerAngles2d(i, j);
            }
            else if (valueType == typeof(RealEulerAngles3d))
            {
                var i = Angle.FromRadians(reader.ReadSingle(compression));
                var j = Angle.FromRadians(reader.ReadSingle(compression));
                var k = Angle.FromRadians(reader.ReadSingle(compression));
                return new RealEulerAngles3d(i, j, k);
            }

            if (valueType == typeof(RealPoint2d))
                return new RealPoint2d(reader.ReadSingle(compression), reader.ReadSingle(compression));
            if (valueType == typeof(RealPoint3d))
                return new RealPoint3d(reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));
            if (valueType == typeof(RealVector2d))
                return new RealVector2d(reader.ReadSingle(compression), reader.ReadSingle(compression));
            if (valueType == typeof(RealVector3d))
                return new RealVector3d(reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));
            if (valueType == typeof(RealVector4d))
                return new RealVector4d(reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));
            if (valueType == typeof(RealQuaternion))
                return new RealQuaternion(reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));
            if (valueType == typeof(RealPlane2d))
                return new RealPlane2d(reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));
            if (valueType == typeof(RealPlane3d))
                return DeserializeObjectRealPlane3d(reader, compression);
            if (valueType == typeof(RealMatrix4x3))
                return new RealMatrix4x3(
                    reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression),
                    reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression),
                    reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression),
                    reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));
            if (valueType == typeof(RealMatrix4x4))
                return new RealMatrix4x4(
                    reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression),
                    reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression),
                    reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression),
                    reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));

            // StringID
            if (valueType == typeof(StringId))
                return DeserializeObjectStringId(cache, reader);

            // Angle (radians)
            if (valueType == typeof(Angle))
                return Angle.FromRadians(reader.ReadSingle(compression));

            if (valueType == typeof(DatumHandle))
                return new DatumHandle(reader.ReadUInt32());

            // Non-byte array = Inline array
            // TODO: Define more clearly in general what constitutes a data reference and what doesn't
            if (valueType.IsArray)
                return DeserializeObjectInlineArray(cache, reader, context, valueInfo, valueType);

            // List = Tag block
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
                return DeserializeObjectTagBlockAsList(cache, reader, context, valueType);

            // actual tag blocks, used in resource definitions
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(TagBlock<>))
                return DeserializeObjectTagBlock(cache, reader, context, valueType);

            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(D3DStructure<>))
                return DeserializeObjectD3DStructure(cache, reader, context, valueType);

            // Ranges
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Bounds<>))
                return DeserializeObjectRange(cache, reader, context, valueType);

            if (valueType == typeof(ComputeShaderReference))
                return DeserializeObjectComputeShaderReference(cache, reader, context);

            if (valueType == typeof(VertexShaderReference))
                return DeserializeObjectVertexShaderReference(cache, reader, context);

            if (valueType == typeof(PixelShaderReference))
                return DeserializeObjectPixelShaderReference(cache, reader, context);

            if (valueType == typeof(PlatformUnsignedValue))
                return DeserializeObjectPlatfornUnsignedValue(reader);

            if (valueType == typeof(PlatformSignedValue))
                return DeserializeObjectPlatfornSignedValue(reader);

            if (valueType == typeof(IndexBufferIndex))
                return DeserializeObjectIndexBufferIndex(reader);

            if (valueType == typeof(StructureSurfaceToTriangleMapping))
                return DeserializeObjectPlaneReference(reader);

            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(BitFlags<>))
                return DeserializeObjectFlagBits(reader, valueInfo, valueType);

            // Assume the value is a structure
            return DeserializeObjectStruct(cache, reader, context, TagStructure.GetTagStructureInfo(valueType, Version, Platform));
        }

        private object DeserializeObjectRealPlane3d(EndianReader reader, TagFieldCompression compression) 
        {
            var value = new RealPlane3d(reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));

            if (!IsValidNormal3d(value.Normal)) 
            {
                Problems.Add($"Invalid plane normal: {CurrentFieldPath} = {value}");
            }

            return value;
        }

        private static bool IsValidNormal3d(RealVector3d normal)
        {
            float magnitude = RealVector3d.Magnitude(normal);

            if (float.IsNaN(magnitude) && float.IsInfinity(magnitude))
            {
                return false;
            }

            return magnitude < 0.0001f || Math.Abs(magnitude - 1.0f) < 0.0001f;
        }

        private object DeserializeObjectStringId(GameCache cache, EndianReader reader) 
        {
            var value = new StringId(reader.ReadUInt32());

            if (value != StringId.Invalid) 
            {
                try
                {
                    cache.StringTable.GetString(value);
                }
                catch
                {
                    Problems.Add($"Invalid stringId: {CurrentFieldPath} = {value}");
                }
            }

            return value;
        }

        public object DeserializeObjectFlagBits(EndianReader reader, TagFieldAttribute valueInfo, Type valueType) 
        {
            TagEnumInfo enumInfo = EnumCache.GetInfo(valueType.GenericTypeArguments[0]);
            Type storageType = valueInfo.EnumType;

            ulong value;
            if (storageType == typeof(byte))
            {
                value = reader.ReadByte();
            }
            else if (storageType == typeof(ushort)) 
            {
                value = reader.ReadUInt16();
            }
            else if (storageType == typeof(uint))
            {
                value = reader.ReadUInt32();
            }
            else 
            {
                Problems.Add($"Unsupported storage type '{storageType}' for Enum '{enumInfo.Type}'");
                value = 0;
            }

            if (!VersionedEnum.ValidateFlagsForImport(enumInfo, value))
                Problems.Add($"Deserializer: Enum value out of range {enumInfo.Type.FullName} = {value}");

            value = VersionedEnum.ImportFlags(enumInfo, value);

            return (IBitFlags)Activator.CreateInstance(valueType, [value]);
        }

        public object DeserializeObjectEnum(EndianReader reader, TagFieldAttribute valueInfo, Type valueType) 
        {
            var storageType = valueInfo.EnumType ?? valueType.GetEnumUnderlyingType();
            object value = DeserializeObjectPrimitiveValue(reader, storageType);

            var enumInfo = EnumCache.GetInfo(valueType);

            if (enumInfo.Attribute.IsVersioned)
            {
                return ConvertVersionedObjectEnumValue(valueInfo, valueType, value, enumInfo);
            }
            else
            {
                if (valueInfo.EnumType != null) 
                {
                    value = CastObjectEnumValue(valueType, valueInfo.EnumType, value);
                }

                // #TODO: Replace with existing enum system
                if (!ObjectEnumHelper.IsEnumDefined(valueType, value))
                {
                    Problems.Add($"Enum out of range: {CurrentFieldPath} = {value}");
                }

                return value;
            }
        }

        public object ConvertVersionedObjectEnumValue(TagFieldAttribute valueInfo, Type valueType, object value, TagEnumInfo enumInfo) 
        {
            try
            {
                return VersionedEnum.ImportValue(valueType, (int)Convert.ChangeType(value, typeof(int)), Version, Platform);
            }
            catch (ArgumentOutOfRangeException)
            {
                Problems.Add($"Enum out of range for version {Version}:{Platform}: {CurrentFieldPath} = {value}");

                return CastObjectEnumValue(enumInfo.Type, valueInfo.EnumType, value);
            }
        }

        public static object CastObjectEnumValue(Type enumType, Type valueType, object value) 
        {
            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Byte:
                    return Enum.ToObject(enumType, (byte)value);
                case TypeCode.UInt16:
                    return Enum.ToObject(enumType, (ushort)value);
                case TypeCode.UInt32:
                    return Enum.ToObject(enumType, (uint)value);
                case TypeCode.SByte:
                    return Enum.ToObject(enumType, (sbyte)value);
                case TypeCode.Int16:
                    return Enum.ToObject(enumType, (short)value);
                case TypeCode.Int32:
                    return Enum.ToObject(enumType, (int)value);
                default:
                    throw new NotImplementedException();
            }
        }

        public object DeserializeObjectTagBlockAsList(GameCache cache, EndianReader reader, ISerializationContext context, Type valueType) 
        {
            long startOffset = reader.BaseStream.Position;

            int count = reader.ReadInt32();
            if (count == 0)
            {
                reader.BaseStream.Position = startOffset + TagBlockSize;
                return Activator.CreateInstance(valueType);
            }

            uint pointer = reader.ReadUInt32();
            reader.BaseStream.Position = context.AddressToOffset((uint)startOffset + 4, pointer);

            var result = (IList)Activator.CreateInstance(valueType, [count]);
            DeserializeObjectTagBlockCore(cache, reader, context, result, count, valueType);

            reader.BaseStream.Position = startOffset + TagBlockSize;

            return result;
        }

        public object DeserializeObjectTagBlock(GameCache cache, EndianReader reader, ISerializationContext context, Type valueType) 
        {
            long startOffset = reader.BaseStream.Position;

            int count = reader.ReadInt32();
            if (count == 0)
            {
                reader.BaseStream.Position = startOffset + TagBlockSize;
                return Activator.CreateInstance(valueType);
            }

            var pointer = new CacheAddress(reader.ReadUInt32());
            reader.BaseStream.Position = context.AddressToOffset((uint)startOffset + 4, pointer.Value);

            var result = (IList)Activator.CreateInstance(valueType, [count]);
            DeserializeObjectTagBlockCore(cache, reader, context, result, count, valueType);

            reader.BaseStream.Position = startOffset + TagBlockSize;
            return result;
        }

        protected void DeserializeObjectTagBlockCore(GameCache cache, EndianReader reader, ISerializationContext context, IList list, int count, Type valueType)
        {
            Type elementType = valueType.GenericTypeArguments[0];

            if (list is TagBlock<byte> typedTagBlock)
            {
                CollectionsMarshal.SetCount(typedTagBlock.Elements, count);
                reader.Read(CollectionsMarshal.AsSpan(typedTagBlock.Elements));
            }
            else if (list is List<byte> typedListBlock)
            {
                CollectionsMarshal.SetCount(typedListBlock, count);
                reader.Read(CollectionsMarshal.AsSpan(typedListBlock));
            }
            else if (elementType.IsClass && !elementType.IsGenericType && elementType.IsSubclassOf(typeof(TagStructure)))
            {
                var info = StructCache.GetTagStructureInfo(elementType);
                for (int i = 0; i < count; i++)
                    list.Add(DeserializeObjectStruct(cache, reader, context, info));
            }
            else
            {
                // We generally don't use value types in blocks other than byte, but this is here in case
                for (int i = 0; i < count; i++)
                    list.Add(DeserializeObjectValue(cache, reader, context, null, elementType));
            }
        }

        public object DeserializeObjectD3DStructure(GameCache cache, EndianReader reader, ISerializationContext context, Type valueType) 
        {
            var result = (ID3DStructure)Activator.CreateInstance(valueType);
            var elementType = valueType.GenericTypeArguments[0];

            var startOffset = reader.BaseStream.Position;
            var pointer = reader.ReadUInt32();

            reader.BaseStream.Position = context.AddressToOffset((uint)startOffset + 4, pointer);

            result.Definition = DeserializeObjectValue(cache, reader, context, null, elementType);

            reader.BaseStream.Position = startOffset + 0xC;
            return result;
        }

        public object DeserializeObjectIndirectValue(GameCache cache, EndianReader reader, ISerializationContext context, Type valueType) 
        {
            var pointer = reader.ReadUInt32();

            if (valueType == typeof(PageableResource) && pointer == 0)
                return null;

            var nextOffset = reader.BaseStream.Position;
            reader.BaseStream.Position = context.AddressToOffset((uint)nextOffset - 4, pointer);

            var result = DeserializeObjectValue(cache, reader, context, null, valueType);
            reader.BaseStream.Position = nextOffset;

            return result;
        }

        public CachedTag DeserializeObjectTagReference(GameCache cache, EndianReader reader, ISerializationContext context, TagFieldAttribute valueInfo) 
        {
            TagGroup group = TagGroup.Null;

            if (valueInfo == null || (valueInfo.Flags & TagFieldFlags.Short) == 0)
            {
                group = reader.ReadTag();

                if (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version))
                    reader.BaseStream.Position += 0x8;
            }

            var result = context.GetTagByIndex(reader.ReadInt32());

            if (group != TagGroup.Null && group.Value != 0)
            {
                if (result != null && valueInfo != null && valueInfo.ValidTags != null)
                {
                    if (!valueInfo.ValidTags.Any(x => result.IsInGroup(x)))
                    {
                        var groups = string.Join(", ", valueInfo.ValidTags);

                        Problems.Add($"Tag reference with invalid group found during deserialization:"
                            + $"\n - {result.Name}.{result.Group.Tag}"
                            + $"\n - valid groups: {groups}");
                    }

                    if (!cache.TagCache.TagDefinitions.TagDefinitionExists(result.Group.Tag) || (cache is not GameCacheHaloOnlineBase && !cache.TagCache.IsTagIndexValid((int)(result.ID & 0xFFFF))))
                    {
                        Problems.Add($"Invalid tag reference: {CurrentFieldPath} = {result}");
                    }
                }

                return result;
            }

            return null;
        }

        public byte[] DeserializeObjectDataReference(EndianReader reader, ISerializationContext context) 
        {
            long startOffset = reader.BaseStream.Position;
            int size = reader.ReadInt32();

            if (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version))
                reader.Skip(8);

            var pointer = reader.ReadUInt32();

            if (pointer == 0)
            {
                reader.BaseStream.Position = startOffset + TagDataSize;
                return [];
            }

            byte[] result = new byte[size];
            reader.BaseStream.Position = context.AddressToOffset((uint)(reader.Position - 4), pointer);
            reader.Read(result);
            reader.BaseStream.Position = startOffset + TagDataSize;

            return result;
        }

        public TagData DeserializeObjectTagData(EndianReader reader, ISerializationContext context) 
        {
            var tagData = new TagData();

            var startOffset = reader.BaseStream.Position;
            var size = reader.ReadInt32();

            if (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version))
                reader.Skip(8);

            var pointer = reader.ReadUInt32();

            tagData.Data = [];
            tagData.Size = size;
            tagData.Address = pointer;

            if (pointer == 0)
            {
                reader.BaseStream.Position = startOffset + TagDataSize;
                return tagData;
            }

            byte[] result = new byte[size];
            reader.BaseStream.Position = context.AddressToOffset((uint)(reader.Position - 4), pointer);
            reader.Read(result);
            reader.BaseStream.Position = startOffset + TagDataSize;

            tagData.Data = result;

            return tagData;
        }

        public Array DeserializeObjectInlineArray(GameCache cache,EndianReader reader, ISerializationContext context, TagFieldAttribute valueInfo, Type valueType) 
        {
            var elementCount = valueInfo.Length;
            var elementType = valueType.GetElementType();
            var result = Array.CreateInstance(elementType, elementCount);

            for (var i = 0; i < elementCount; i++)
                result.SetValue(DeserializeObjectValue(cache,reader, context, null, elementType), i);

            return result;
        }

        public static string DeserializeObjectString(EndianReader reader, TagFieldAttribute valueInfo) 
        {
            if (valueInfo == null || valueInfo.Length == 0)
                throw new ArgumentException("Cannot deserialize a string with no length set");

            switch (valueInfo.CharSet)
            {
                case CharSet.Ansi:
                case CharSet.Unicode:
                    return reader.ReadNullTerminatedString(valueInfo.Length, valueInfo.CharSet);
                default:
                    throw new NotSupportedException($"{valueInfo.CharSet}");
            }
        }

        public object DeserializeObjectRange(GameCache cache, EndianReader reader, ISerializationContext context, Type rangeType) 
        {
            var boundsType = rangeType.GenericTypeArguments[0];
            var min = DeserializeObjectValue(cache, reader, context, null, boundsType);
            var max = DeserializeObjectValue(cache, reader, context, null, boundsType);

            return Activator.CreateInstance(rangeType, min, max);
        }

        public IndexBufferIndex DeserializeObjectIndexBufferIndex(EndianReader reader)
        {
            if (Version >= CacheVersion.HaloReach || Version == CacheVersion.HaloOnlineED)
                return new IndexBufferIndex(reader.ReadInt32());
            else
                return new IndexBufferIndex(reader.ReadUInt16());
        }

        public object DeserializeObjectPlaneReference(EndianReader reader)
        {
            if (Version >= CacheVersion.HaloReach || Version == CacheVersion.HaloOnlineED)
            {
                var value = reader.ReadUInt32();
                return new StructureSurfaceToTriangleMapping((int)(value >> 12), (int)(value & 0xFFF));
            }
            else
            {
                ushort triangleIndex = reader.ReadUInt16();
                ushort clusterIndex = reader.ReadUInt16();
                return new StructureSurfaceToTriangleMapping(triangleIndex, clusterIndex);
            }
        }

        public PlatformUnsignedValue DeserializeObjectPlatfornUnsignedValue(EndianReader reader)
        {
            switch (CacheVersionDetection.GetPlatformType(Platform))
            {
                case PlatformType._64Bit:
                    return new PlatformUnsignedValue(reader.ReadUInt64());
                case PlatformType._32Bit:
                    return new PlatformUnsignedValue(reader.ReadUInt32());
                default:
                    throw new NotImplementedException();
            }
        }

        public PlatformSignedValue DeserializeObjectPlatfornSignedValue(EndianReader reader)
        {
            switch (CacheVersionDetection.GetPlatformType(Platform))
            {
                case PlatformType._64Bit:
                    return new PlatformSignedValue(reader.ReadInt64());
                case PlatformType._32Bit:
                    return new PlatformSignedValue(reader.ReadInt32());
                default:
                    throw new NotImplementedException();
            }
        }

        public ComputeShaderReference DeserializeObjectComputeShaderReference(GameCache cache, EndianReader reader, ISerializationContext context) 
        {
            return null;
        }

        public PixelShaderReference DeserializeObjectPixelShaderReference(GameCache cache, EndianReader reader, ISerializationContext context) 
        {
            var endPosition = reader.BaseStream.Position + 0x04;

            var headerAddress = reader.ReadUInt32();

            if (headerAddress < 1)
                return null;

            var headerOffset = context.AddressToOffset((uint)(reader.BaseStream.Position - 4), headerAddress);
            reader.SeekTo(headerOffset);

            var header = (PixelShaderHeader)DeserializeObjectStruct(cache, reader, context, ObjectStructure.GetObjectStructureInfo(typeof(PixelShaderHeader), Version, Platform));

            if (header.ShaderDataAddress == 0)
                return null;

            var debugHeaderOffset = reader.Position;
            var debugHeader = (ShaderDebugHeader)DeserializeObjectStruct(cache, reader, context, ObjectStructure.GetObjectStructureInfo(typeof(ShaderDebugHeader), Version, Platform));

            if ((debugHeader.Magic >> 16) != 0x102A)
                return null;

            if (debugHeader.StructureSize == 0)
                return null;

            reader.SeekTo(debugHeaderOffset);
            var debugData = reader.ReadBytes((int)debugHeader.StructureSize);

            var updbName = "";

            if (debugHeader.UpdbPointerOffset != 0)
            {
                reader.SeekTo(debugHeaderOffset + (long)debugHeader.UpdbPointerOffset);
                var updbNameLength = reader.ReadUInt64();

                if (updbNameLength > 0)
                    updbName = new string(reader.ReadChars((int)updbNameLength));
            }

            var totalSize = debugHeader.ShaderDataSize;
            var constantSize = 0U;
            var codeSize = totalSize;

            if (debugHeader.CodeHeaderOffset != 0)
            {
                reader.SeekTo(debugHeaderOffset + debugHeader.CodeHeaderOffset);
                constantSize = reader.ReadUInt32();
                codeSize = reader.ReadUInt32();
            }

            var constant_block_offset = context.AddressToOffset(headerOffset + 0x10, header.ShaderDataAddress);
            reader.SeekTo(constant_block_offset);
            var constantData = reader.ReadBytes((int)constantSize);

            var shader_data_block_offset = constant_block_offset + constantSize;
            reader.SeekTo(shader_data_block_offset);
            var shaderData = reader.ReadBytes((int)codeSize);

            reader.SeekTo(endPosition);

            var info = new XboxShaderInfo
            {
                DataAddress = shader_data_block_offset,
                DebugInfoOffset = (uint)debugHeaderOffset,
                DebugInfoSize = debugHeader.StructureSize,
                DatabasePath = updbName,
                DataSize = totalSize,
                ConstantDataSize = constantSize,
                CodeDataSize = codeSize
            };

            return new PixelShaderReference
            {
                Info = info,
                UpdbName = updbName,
                Header = header,
                DebugHeader = debugHeader,
                DebugData = debugData,
                ShaderData = shaderData,
                ConstantData = constantData
            };
        }

        public VertexShaderReference DeserializeObjectVertexShaderReference(GameCache cache, EndianReader reader, ISerializationContext context) 
        {
            var endPosition = reader.BaseStream.Position + 0x04;

            var headerAddress = reader.ReadUInt32();

            if (headerAddress < 1)
                return null;

            var headerOffset = context.AddressToOffset((uint)(reader.BaseStream.Position - 4), headerAddress);
            reader.SeekTo(headerOffset);

            var header = (VertexShaderHeader)DeserializeObjectStruct(cache, reader, context, ObjectStructure.GetObjectStructureInfo(typeof(VertexShaderHeader), Version, Platform));

            if (header.ShaderDataAddress == 0)
                return null;

            var debugHeaderOffset = reader.Position;
            var debugHeader = (ShaderDebugHeader)DeserializeObjectStruct(cache, reader, context, ObjectStructure.GetObjectStructureInfo(typeof(ShaderDebugHeader), Version, Platform));

            if ((debugHeader.Magic >> 16) != 0x102A)
                return null;

            if (debugHeader.StructureSize == 0)
                return null;

            reader.SeekTo(debugHeaderOffset);
            var debugData = reader.ReadBytes((int)debugHeader.StructureSize);

            var updbName = "";

            if (debugHeader.UpdbPointerOffset != 0)
            {
                reader.SeekTo(debugHeaderOffset + (long)debugHeader.UpdbPointerOffset);
                var updbNameLength = reader.ReadUInt64();

                if (updbNameLength > 0)
                    updbName = new string(reader.ReadChars((int)updbNameLength));
            }

            var totalSize = debugHeader.ShaderDataSize;
            var constantSize = 0U;
            var codeSize = totalSize;

            if (debugHeader.CodeHeaderOffset != 0)
            {
                reader.SeekTo(debugHeaderOffset + debugHeader.CodeHeaderOffset);
                constantSize = reader.ReadUInt32();
                codeSize = reader.ReadUInt32();
            }

            var constant_block_offset = context.AddressToOffset(headerOffset + 0x10, header.ShaderDataAddress);
            reader.SeekTo(constant_block_offset);
            var constantData = reader.ReadBytes((int)constantSize);

            var shader_data_block_offset = constant_block_offset + constantSize;
            reader.SeekTo(shader_data_block_offset);
            var shaderData = reader.ReadBytes((int)codeSize);

            reader.SeekTo(endPosition);

            var info = new XboxShaderInfo
            {
                DataAddress = shader_data_block_offset,
                DebugInfoOffset = (uint)debugHeaderOffset,
                DebugInfoSize = debugHeader.StructureSize,
                DatabasePath = updbName,
                DataSize = totalSize,
                ConstantDataSize = constantSize,
                CodeDataSize = codeSize
            };

            return new VertexShaderReference
            {
                Info = info,
                UpdbName = updbName,
                Header = header,
                DebugHeader = debugHeader,
                DebugData = debugData,
                ShaderData = shaderData,
                ConstantData = constantData
            };
        }
    }
}
