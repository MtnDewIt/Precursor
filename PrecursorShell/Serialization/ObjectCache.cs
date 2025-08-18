using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TagTool.Cache;
using TagTool.Tags;

namespace PrecursorShell.Serialization
{
    public class ObjectCache
    {
        public readonly Dictionary<Type, TagStructureAttribute> ObjectStructureAttributes =
            new Dictionary<Type, TagStructureAttribute> { };

        public readonly Dictionary<Type, TagStructureInfo> ObjectStructureInfos =
            new Dictionary<Type, TagStructureInfo> { };

        public readonly Dictionary<Type, ObjectFieldEnumerable> ObjectFieldEnumerables =
            new Dictionary<Type, ObjectFieldEnumerable> { };

        public readonly Dictionary<FieldInfo, TagFieldAttribute> ObjectFieldAttributes =
            new Dictionary<FieldInfo, TagFieldAttribute> { };

        public TagStructureInfo GetObjectStructureInfo(Type type, CacheVersion version, CachePlatform cachePlatform)
        {
            if (!ObjectStructureInfos.TryGetValue(type, out TagStructureInfo info)) 
            {
                lock (ObjectStructureInfos)
                {
                    if (!ObjectStructureInfos.TryGetValue(type, out info))
                        ObjectStructureInfos[type] = info = new TagStructureInfo(type, version, cachePlatform);
                }
            }

            return info;
        }

        public ObjectFieldEnumerable GetObjectFieldEnumerable(TagStructureInfo info)
        {
            if (!ObjectFieldEnumerables.TryGetValue(info.Types[0], out ObjectFieldEnumerable enumerator)) 
            {
                lock (ObjectFieldEnumerables)
                {
                    if (!ObjectFieldEnumerables.TryGetValue(info.Types[0], out enumerator))
                        ObjectFieldEnumerables[info.Types[0]] = enumerator = new ObjectFieldEnumerable(info);
                }
            }

            return enumerator;
        }

        public TagStructureAttribute GetObjectStructureAttribute(Type type, CacheVersion version, CachePlatform cachePlatform)
        {
            TagStructureAttribute GetStructureAttribute()
            {
                var attributes = type.GetCustomAttributes<TagStructureAttribute>(false);
                var matchingAttributes = attributes.Where(a => CacheVersionDetection.TestAttribute(a, version, cachePlatform));
                return matchingAttributes.FirstOrDefault();
            }

            if (!ObjectStructureAttributes.TryGetValue(type, out TagStructureAttribute attribute)) 
            {
                lock (ObjectStructureAttributes)
                {
                    if (!ObjectStructureAttributes.TryGetValue(type, out attribute))
                        ObjectStructureAttributes[type] = attribute = GetStructureAttribute();
                }
            }

            return attribute;
        }

        public TagFieldAttribute GetObjectFieldAttribute(Type type, FieldInfo field, CacheVersion version, CachePlatform cachePlatform)
        {
            if (field.DeclaringType != type && !type.IsSubclassOf(field.DeclaringType))
                throw new ArgumentException(nameof(field), new TypeAccessException(type.FullName));

            TagFieldAttribute GetFieldAttribute()
            {
                var attributes = field.GetCustomAttributes<TagFieldAttribute>(false);
                var matchingAttributes = attributes.Where(a => CacheVersionDetection.TestAttribute(a, version, cachePlatform));
                return matchingAttributes.FirstOrDefault() ?? attributes.DefaultIfEmpty(TagFieldAttribute.Default).First();
            }

            if (!ObjectFieldAttributes.TryGetValue(field, out TagFieldAttribute attribute)) 
            {
                lock (ObjectFieldAttributes)
                {
                    if (!ObjectFieldAttributes.TryGetValue(field, out attribute))
                        ObjectFieldAttributes[field] = attribute = GetFieldAttribute();
                }
            }

            return attribute;
        }
    }
}
