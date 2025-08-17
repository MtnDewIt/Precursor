using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TagTool.Cache;
using TagTool.Common;
using TagTool.Tags;

namespace Precursor.Serialization
{
    public class ObjectFieldEnumerable : IEnumerable<TagFieldInfo>
    {
        private readonly List<TagFieldInfo> TagFieldInfos = new List<TagFieldInfo> { };

        public List<string> Problems { get; set; }

        public TagStructureInfo Info { get; set; }

        public TagFieldInfo TagFieldInfo { get; set; }

        public int Count => TagFieldInfos.Count;

        public struct Enumerator : IEnumerator<TagFieldInfo>
        {
            private List<TagFieldInfo>.Enumerator enumerator;
            public TagFieldInfo Current => enumerator.Current;
            object IEnumerator.Current => enumerator.Current;
            public void Dispose() => enumerator.Dispose();
            public bool MoveNext() => enumerator.MoveNext();
            void IEnumerator.Reset() => ((IEnumerator<TagFieldInfo>)enumerator).Reset();
        }

        public Enumerator GetEnumerator()
        {
            var impl = TagFieldInfos.GetEnumerator();

            return Unsafe.As<List<TagFieldInfo>.Enumerator, Enumerator>(ref impl);
        }

        IEnumerator<TagFieldInfo> IEnumerable<TagFieldInfo>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public TagFieldInfo this[int index] => TagFieldInfos[index];

        public ObjectFieldEnumerable(TagStructureInfo info) 
        {
            Info = info;
            Problems = new List<string>();
            ParseFieldEnumerable();
        }

        public FieldInfo Find(Predicate<FieldInfo> match) => TagFieldInfos.Find(f => match.Invoke(f.FieldInfo))?.FieldInfo ?? null;

        public void ParseFieldEnumerable() 
        {
            uint offset = 0;

            foreach (var type in Info.Types.Reverse<Type>())
            {
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).OrderBy(i => i.MetadataToken))
                {
                    var attr = ObjectStructure.GetObjectFieldAttribute(type, field, Info.Version, Info.CachePlatform);

                    if (CacheVersionDetection.TestAttribute(attr, Info.Version, Info.CachePlatform))
                    {
                        if (attr.Flags.HasFlag(TagFieldFlags.Padding))
                        {
                            if (field.FieldType != typeof(byte[]))
                                new Exception("Padding fields must be of type Byte[]");
                        }
                        else
                        {
                            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(FlagBits<>))
                            {
                                var enumType = field.FieldType.GenericTypeArguments[0];
                                var info = TagEnum.GetInfo(enumType, Info.Version, Info.CachePlatform);

                                if (!info.IsVersioned)
                                    throw new Exception("FlagBits Enum must have a 'TagEnum' attribute with IsVersioned=True");

                                if (attr.EnumType == null)
                                    throw new Exception("FlagBits Enum must have the 'EnumType' TagField attribute set");
                            }
                            else if (field.FieldType.IsEnum)
                            {
                                var info = TagEnum.GetInfo(field.FieldType, Info.Version, Info.CachePlatform);

                                if (info.IsVersioned)
                                {
                                    if (attr.EnumType == null)
                                        throw new Exception("Versioned Enum must have the 'EnumType' TagField attribute set");
                                }
                            }
                        }

                        var fieldSize = TagFieldInfo.GetFieldSize(field.FieldType, attr, Info.Version, Info.CachePlatform);

                        if (fieldSize == 0 && !attr.Flags.HasFlag(TagFieldFlags.Runtime))
                            throw new InvalidOperationException();

                        uint align = TagFieldInfo.GetFieldAlignment(field.FieldType, attr, Info.Version, Info.CachePlatform);

                        if (align > 0)
                            offset = offset + (align - 1) & ~(align - 1);

                        var tagFieldInfo = new TagFieldInfo(field, attr, offset, fieldSize);

                        TagFieldInfos.Add(tagFieldInfo);
                        offset += fieldSize;
                    }
                }
            }

            uint expectedSize = ObjectStructure.GetStructureSize(Info.Types[0], Info.Version, Info.CachePlatform);

            if (Info.Structure.Align > 0)
                offset = offset + (Info.Structure.Align - 1) & ~(Info.Structure.Align - 1);

            if (Info.Structure.Align > 0)
                expectedSize = expectedSize + (Info.Structure.Align - 1) & ~(Info.Structure.Align - 1);

            var typename = Info.Types[0].FullName.Replace("TagTool.", "").Replace("Tags.Definitions.", "");

            if (offset != expectedSize)
                Problems.Add($"Bad Size. Version: {Info.Version}:{Info.CachePlatform}, Type: '{typename}', Expected: 0x{expectedSize:X}, Actual: 0x{offset:X}");
        }
    }
}
