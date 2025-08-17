using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace Precursor.Serialization
{
    public class Deserializer
    {
        private readonly CacheVersion Version;
        private readonly CachePlatform Platform;
        private readonly Stack<string> PathStack;

        public readonly List<string> Problems;

        public string CurrentFieldPath => string.Join(".", PathStack.Reverse());

        public Deserializer(CacheVersion version, CachePlatform platform) 
        {
            Version = version;
            Platform = platform;
            PathStack = new Stack<string>();
            Problems = new List<string>();
        }

        public object DeserializeTagInstance(GameCache cache, Stream stream, CachedTag instance)
        {
            ISerializationContext context = null;
            Type type = null;

            switch (cache) 
            {
                case GameCacheGen1 gameCacheGen1:
                    context = new Gen1SerializationContext(stream, gameCacheGen1, (CachedTagGen1)instance);
                    type = gameCacheGen1.TagCache.TagDefinitions.GetTagDefinitionType(instance.Group);
                    break;
                case GameCacheGen2 gameCacheGen2:
                    context = new Gen2SerializationContext(stream, gameCacheGen2, (CachedTagGen2)instance);
                    type = gameCacheGen2.TagCache.TagDefinitions.GetTagDefinitionType(instance.Group);
                    break;
                case GameCacheGen3 gameCacheGen3:
                    context = new Gen3SerializationContext(stream, gameCacheGen3, (CachedTagGen3)instance);
                    type = gameCacheGen3.TagCache.TagDefinitions.GetTagDefinitionType(instance.Group);
                    break;
                case GameCacheMonolithic gameCacheMonolithic:
                    context = new TagSerializationContextMonolithic(stream, gameCacheMonolithic, (CachedTagMonolithic)instance);
                    type = gameCacheMonolithic.TagCache.TagDefinitions.GetTagDefinitionType(instance.Group);
                    break;
                case GameCacheHaloOnlineBase gameCacheHaloOnline:
                    context = new HaloOnlineSerializationContext(stream, gameCacheHaloOnline, (CachedTagHaloOnline)instance);
                    type = gameCacheHaloOnline.TagCache.TagDefinitions.GetTagDefinitionType(instance.Group);
                    break;
                case GameCacheGen4 gameCacheGen4:
                    context = new Gen4SerializationContext(stream, gameCacheGen4, (CachedTagGen4)instance);
                    type = gameCacheGen4.TagCache.TagDefinitions.GetTagDefinitionType(instance.Group);
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

        public object DeserializeObjectStruct(GameCache cache, EndianReader reader, ISerializationContext context, TagStructureInfo info) 
        {
            var baseOffset = reader.BaseStream.Position;
            var instance = Activator.CreateInstance(info.Types[0]);

            var fieldEnumerable = ObjectStructure.GetObjectFieldEnumerable(info.Types[0], info.Version, info.CachePlatform);

            if (fieldEnumerable.Problems.Count > 0)
                Problems.AddRange(fieldEnumerable.Problems);

            foreach (var tagFieldInfo in fieldEnumerable)
                DeserializeObjectProperty(cache, reader, context, instance, tagFieldInfo, baseOffset);

            if (info.TotalSize > 0)
                reader.BaseStream.Position = baseOffset + info.TotalSize;

            return instance;
        }

        public void DeserializeObjectProperty(GameCache cache, EndianReader reader, ISerializationContext context, object instance, TagFieldInfo tagFieldInfo, long baseOffset)
        {
            var attr = tagFieldInfo.Attribute;

            if ((attr.Flags & TagFieldFlags.Runtime) != 0 || !CacheVersionDetection.TestAttribute(attr, Version, Platform))
                return;

            uint align = TagFieldInfo.GetFieldAlignment(tagFieldInfo.FieldType, tagFieldInfo.Attribute, Version, Platform);

            if (align > 0)
            {
                var fieldOffset = (uint)(reader.BaseStream.Position - baseOffset);

                reader.BaseStream.Position += -fieldOffset & (align - 1);
            }

            if ((attr.Flags & TagFieldFlags.Padding) != 0)
            {
                //disable padding warnings for gen2 defs
                if (Version <= CacheVersion.Halo2PC)
                {
                    reader.BaseStream.Position += attr.Length;
                    return;
                }

                var unused = reader.ReadBytes(attr.Length);

                foreach (var b in unused)
                {
                    if (b != 0)
                    {
                        Problems.Add($"Non-zero padding found in {tagFieldInfo.FieldInfo.DeclaringType.FullName}.{tagFieldInfo.FieldInfo.Name} = {b}");
                        break;
                    }
                }
            }
            else
            {
                PathStack.Push(tagFieldInfo.FieldInfo.Name);

                var value = DeserializeObjectValue(cache, reader, context, attr, tagFieldInfo.FieldType);

                tagFieldInfo.SetValue(instance, value);

                PathStack.Pop();
            }
        }

        public object DeserializeObjectValue(GameCache cache, EndianReader reader, ISerializationContext context, TagFieldAttribute valueInfo, Type valueType) 
        {
            if (valueType.IsPrimitive)
                return DeserializeObjectPrimitiveValue(reader, valueType);

            return DeserializeObjectComplexValue(cache, reader, context, valueInfo, valueType);
        }

        public object DeserializeObjectPrimitiveValue(EndianReader reader, Type valueType) 
        {
            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Single:
                    float value = DeserializeObjectSingle(reader);
                    return PrimitiveValueCache.For(value);
                case TypeCode.Byte:
                    return PrimitiveValueCache.For(reader.ReadByte());
                case TypeCode.Int16:
                    return PrimitiveValueCache.For(reader.ReadInt16());
                case TypeCode.Int32:
                    return PrimitiveValueCache.For(reader.ReadInt32());
                case TypeCode.Int64:
                    return PrimitiveValueCache.For(reader.ReadInt64());
                case TypeCode.SByte:
                    return PrimitiveValueCache.For(reader.ReadSByte());
                case TypeCode.UInt16:
                    return PrimitiveValueCache.For(reader.ReadUInt16());
                case TypeCode.UInt32:
                    return PrimitiveValueCache.For(reader.ReadUInt32());
                case TypeCode.UInt64:
                    return PrimitiveValueCache.For(reader.ReadUInt64());
                case TypeCode.Boolean:
                    return PrimitiveValueCache.For(reader.ReadBoolean());
                case TypeCode.Double:
                    return PrimitiveValueCache.For(reader.ReadDouble());
                default:
                    throw new ArgumentException("Unsupported type " + valueType.Name);
            }
        }

        public float DeserializeObjectSingle(EndianReader reader) 
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

            if (valueType == typeof(Tag))
                return new Tag(reader.ReadInt32());

            // TagInstance = Tag reference
            if (valueType == typeof(CachedTag))
                return DeserializeObjectTagReference(cache, reader, context, valueInfo);

            // ResourceAddress = Resource address
            if (valueType == typeof(CacheAddress))
                return new CacheAddress(reader.ReadUInt32());

            // Byte array = Data reference
            // TODO: Allow other types to be in data references, since sometimes they can point to a structure
            if (valueType == typeof(byte[]) && valueInfo.Length == 0)
                return DeserializeObjectDataReference(reader, context);

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

            if (valueType == typeof(Point2d))
                return new Point2d(reader.ReadInt16(), reader.ReadInt16());
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

            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(FlagBits<>))
                return DeserializeObjectFlagBits(reader, valueInfo, valueType);

            // Assume the value is a structure
            return DeserializeObjectStruct(cache, reader, context, TagStructure.GetTagStructureInfo(valueType, Version, Platform));
        }

        public object DeserializeObjectRealPlane3d(EndianReader reader, TagFieldCompression compression) 
        {
            var value = new RealPlane3d(reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression), reader.ReadSingle(compression));

            if (!IsValidNormal3d(value.Normal)) 
            {
                Problems.Add($"Invalid plane normal: {CurrentFieldPath} = {value}");
            }

            return value;
        }

        public static bool IsValidNormal3d(RealVector3d normal)
        {
            float magnitude = RealVector3d.Magnitude(normal);

            if (float.IsNaN(magnitude) && float.IsInfinity(magnitude))
            {
                return false;
            }

            return magnitude < 0.0001f || Math.Abs(magnitude - 1.0f) < 0.0001f;
        }

        public object DeserializeObjectStringId(GameCache cache, EndianReader reader) 
        {
            var value = new StringId(reader.ReadUInt32());

            if (value != StringId.Invalid || value.Value != 0xFFFFFFFF) 
            {
                try
                {
                    cache.StringTable.GetString(value);
                }
                catch
                {
                    Problems.Add($"Invalid stringId: {CurrentFieldPath} {value}");
                }
            }

            return value;
        }

        public object DeserializeObjectFlagBits(EndianReader reader, TagFieldAttribute valueInfo, Type valueType) 
        {
            var enumType = valueType.GenericTypeArguments[0];
            object value = DeserializeObjectPrimitiveValue(reader, valueInfo.EnumType ?? valueType.GetEnumUnderlyingType());
            uint castedValue = (uint)Convert.ChangeType(value, typeof(uint));

            return VersionedEnum.ImportFlags(enumType, castedValue, Version, Platform);
        }

        public object DeserializeObjectEnum(EndianReader reader, TagFieldAttribute valueInfo, Type valueType) 
        {
            var storageType = valueInfo.EnumType ?? valueType.GetEnumUnderlyingType();

            object value = DeserializeObjectPrimitiveValue(reader, storageType);

            var enumInfo = TagEnum.GetInfo(valueType, Version, Platform);

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
            var result = Activator.CreateInstance(valueType);
            var elementType = valueType.GenericTypeArguments[0];

            var startOffset = reader.BaseStream.Position;
            var count = reader.ReadInt32();
            var pointer = new CacheAddress(reader.ReadUInt32());

            if (count == 0 || (count != 0 && pointer.Value == 0))
            {
                reader.BaseStream.Position = startOffset + (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0xC : 0x8);
                return result;
            }

            var addMethod = valueType.GetMethod("Add");

            reader.BaseStream.Position = context.AddressToOffset((uint)startOffset + 4, pointer.Value);

            for (var i = 0; i < count; i++)
            {
                var element = DeserializeObjectValue(cache,reader, context, null, elementType);

                addMethod.Invoke(result, new[] { element });
            }

            reader.BaseStream.Position = startOffset + (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0xC : 0x8);

            return result;
        }

        public object DeserializeObjectTagBlock(GameCache cache, EndianReader reader, ISerializationContext context, Type valueType) 
        {
            var result = Activator.CreateInstance(valueType);
            var elementType = valueType.GenericTypeArguments[0];

            var startOffset = reader.BaseStream.Position;
            var count = reader.ReadInt32();

            var pointer = new CacheAddress(reader.ReadUInt32());
            if (count == 0 || (count != 0 && pointer.Value == 0))
            {
                reader.BaseStream.Position = startOffset + (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0xC : 0x8);
                return result;
            }

            reader.BaseStream.Position = context.AddressToOffset((uint)startOffset + 4, pointer.Value);

            object[] pooledValuesToAdd = ArrayPool<object>.Shared.Rent(count);
            Span<object> valuesToAdd = pooledValuesToAdd.AsSpan()[..count];

            for (var i = 0; i < count; i++)
            {
                valuesToAdd[i] = DeserializeObjectValue(cache, reader, context, null, elementType);
            }

            ReflectionHelpers.GetAddRangeBoxedDelegate(valueType)(result, valuesToAdd);
            ArrayPool<object>.Shared.Return(pooledValuesToAdd);

            reader.BaseStream.Position = startOffset + (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0xC : 0x8);

            return result;
        }

        public object DeserializeObjectD3DStructure(GameCache cache, EndianReader reader, ISerializationContext context, Type valueType) 
        {
            var result = Activator.CreateInstance(valueType);
            var elementType = valueType.GenericTypeArguments[0];

            var startOffset = reader.BaseStream.Position;
            var pointer = reader.ReadUInt32();

            reader.BaseStream.Position = context.AddressToOffset((uint)startOffset + 4, pointer);

            var definition = DeserializeObjectValue(cache,reader, context, null, elementType);
            valueType.GetField("Definition").SetValue(result, definition);

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
            Tag group = Tag.Null;

            if (valueInfo == null || (valueInfo.Flags & TagFieldFlags.Short) == 0)
            {
                group = reader.ReadTag();

                if (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version))
                    reader.BaseStream.Position += 0x8;
            }

            var result = context.GetTagByIndex(reader.ReadInt32());

            if (group != Tag.Null && group.Value != 0)
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
            var startOffset = reader.BaseStream.Position;
            var size = reader.ReadInt32();

            if (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version))
                reader.BaseStream.Position = startOffset + 0xC;

            var pointer = reader.ReadUInt32();

            if (pointer == 0)
            {
                reader.BaseStream.Position = startOffset + (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0x14 : 0x8);
                return new byte[0];
            }

            var result = new byte[size];
            reader.BaseStream.Position = context.AddressToOffset((uint)(startOffset + (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0xC : 0x4)), pointer);
            reader.Read(result, 0, size);
            reader.BaseStream.Position = startOffset + (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0x14 : 0x8);

            return result;
        }

        public TagData DeserializeObjectTagData(EndianReader reader, ISerializationContext context) 
        {
            var tagData = new TagData();

            var startOffset = reader.BaseStream.Position;
            var size = reader.ReadInt32();

            if (CacheVersionDetection.IsInGen(CacheGeneration.First, Version))
            {
                reader.ReadUInt32();
                tagData.Gen1ExternalOffset = reader.ReadUInt32();
            }
            else if (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version))
                reader.BaseStream.Position = startOffset + 0xC;

            var pointer = reader.ReadUInt32();

            tagData.Data = new byte[0];
            tagData.Size = size;
            tagData.Address = pointer;

            if (pointer == 0)
            {
                reader.BaseStream.Position = startOffset + (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0x14 : 0x8);
                return tagData;
            }

            var result = new byte[size];
            reader.BaseStream.Position = context.AddressToOffset((uint)(startOffset + (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0xC : 0x4)), pointer);
            reader.Read(result, 0, size);
            reader.BaseStream.Position = startOffset + (!CacheVersionDetection.IsInGen(CacheGeneration.Second, Version) ? 0x14 : 0x8);

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
            var platformType = CacheVersionDetection.GetPlatformType(Platform);

            switch (platformType)
            {
                case PlatformType._64Bit:
                    return new PlatformUnsignedValue(reader.ReadUInt64());

                default:
                case PlatformType._32Bit:
                    return new PlatformUnsignedValue(reader.ReadUInt32());

            }
        }

        public PlatformSignedValue DeserializeObjectPlatfornSignedValue(EndianReader reader) 
        {
            var platformType = CacheVersionDetection.GetPlatformType(Platform);

            switch (platformType)
            {
                case PlatformType._64Bit:
                    return new PlatformSignedValue(reader.ReadInt64());

                default:
                case PlatformType._32Bit:
                    return new PlatformSignedValue(reader.ReadInt32());
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
