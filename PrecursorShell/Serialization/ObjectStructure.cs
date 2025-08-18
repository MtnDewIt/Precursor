using System;
using System.Collections.Generic;
using System.Reflection;
using TagTool.Cache;
using TagTool.Tags;

namespace PrecursorShell.Serialization
{
    public class ObjectStructure
    {
        private static readonly Dictionary<(CacheVersion version, CachePlatform platform), ObjectCache> ObjectLookup =
            new Dictionary<(CacheVersion version, CachePlatform platform), ObjectCache> { };

        static ObjectStructure()
        {
            lock (ObjectLookup)
            {
                foreach (var platform in Enum.GetValues(typeof(CachePlatform)) as CachePlatform[])
                    foreach (var version in Enum.GetValues(typeof(CacheVersion)) as CacheVersion[])
                        ObjectLookup[(version, platform)] = new ObjectCache();
            }
        }

        public static TagStructureAttribute GetObjectStructureAttribute(Type type, CacheVersion version, CachePlatform cachePlatform) =>
            ObjectLookup[(version, cachePlatform)].GetObjectStructureAttribute(type, version, cachePlatform);

        public static TagStructureInfo GetObjectStructureInfo(Type type, CacheVersion version, CachePlatform cachePlatform) =>
            ObjectLookup[(version, cachePlatform)].GetObjectStructureInfo(type, version, cachePlatform);

        public static ObjectFieldEnumerable GetObjectFieldEnumerable(TagStructureInfo info) =>
            ObjectLookup[(info.Version, info.CachePlatform)].GetObjectFieldEnumerable(info);

        public static TagFieldAttribute GetObjectFieldAttribute(Type type, FieldInfo field, CacheVersion version, CachePlatform cachePlatform) =>
            ObjectLookup[(version, cachePlatform)].GetObjectFieldAttribute(type, field, version, cachePlatform);

        public static ObjectFieldEnumerable GetObjectFieldEnumerable(Type type, CacheVersion version, CachePlatform cachePlatform) =>
            GetObjectFieldEnumerable(GetObjectStructureInfo(type, version, cachePlatform));

        public static uint GetStructureSize(Type type, CacheVersion version, CachePlatform cachePlatform)
        {
            uint size = 0;

            var currentType = type;

            while (currentType != typeof(object))
            {
                var attribute = ObjectLookup[(version, cachePlatform)].GetObjectStructureAttribute(currentType, version, cachePlatform);

                currentType = currentType.BaseType;

                if (attribute == null)
                    continue;

                size += attribute.Size;
            }
            return size;
        }
    }
}
