using System.Collections.Generic;

namespace Precursor.Cache.Objects
{
    public class CacheObject
    {
        public List<CacheTypeObject> Caches { get; set; }

        public CacheObject()
        {
            Caches = new List<CacheTypeObject>();
        }

        public class CacheTypeObject 
        {
            public CacheType CacheType { get; set; }
            public string Path { get; set; }

            public CacheTypeObject(CacheType cacheType, string path)
            {
                CacheType = cacheType;
                Path = path;
            }
        }
    }
}
