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

            var filter = new List<CacheBuild> 
            {
                // We resolve the paths for each MCC build collectively, so there is no need to define them individually
                CacheBuild.None,
                CacheBuild.Halo1MCC,
                CacheBuild.Halo2MCC,
                CacheBuild.Halo3MCC,
                CacheBuild.Halo3ODSTMCC,
                CacheBuild.HaloReachMCC,
                CacheBuild.Halo4MCC,
                CacheBuild.Halo2AMPMCC,
            };

            foreach (CacheBuild build in Enum.GetValues(typeof(CacheBuild)))
            {
                if (filter.Contains(build))
                    continue;

                cacheObject.Builds.Add(new CacheBuildObject(build, null));
            }

            var handler = new CacheObjectHandler();

            var outputObject = handler.Serialize(cacheObject);

            File.WriteAllText(outputPath, outputObject);
        }
    }
}
