using Precursor.Cache.Handlers;
using System;
using System.Collections.Generic;
using System.IO;

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

        public static void GenerateBuildData(string outputPath)
        {
            var cacheObject = new CacheObject();

            foreach (CacheBuild build in Enum.GetValues(typeof(CacheBuild)))
            {
                if (build == CacheBuild.None)
                    continue;

                cacheObject.Builds.Add(new CacheBuildObject(build, null));
            }

            var handler = new CacheObjectHandler();

            var outputObject = handler.Serialize(cacheObject);

            File.WriteAllText(outputPath, outputObject);
        }
    }
}
