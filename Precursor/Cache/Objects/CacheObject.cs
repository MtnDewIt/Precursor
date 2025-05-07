using System.Collections.Generic;

namespace Precursor.Cache.Objects
{
    public class CacheObject
    {
        public List<CacheBuildObject> Builds { get; set; }

        public CacheObject()
        {
            Builds = new List<CacheBuildObject>();
        }

        public class CacheBuildObject 
        {
            public CacheBuild Build { get; set; }
            public string Path { get; set; }

            public CacheBuildObject(CacheBuild build, string path)
            {
                Build = build;
                Path = path;
            }
        }
    }
}
