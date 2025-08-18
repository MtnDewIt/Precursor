using PrecursorShell.Cache.BuildTable.Handlers;
using System;
using System.Collections.Generic;
using System.IO;

namespace PrecursorShell.Cache.BuildTable
{
    public class BuildTableProperties
    {
        public List<BuildTableEntry> Builds { get; set; }

        public BuildTableProperties()
        {
            Builds = new List<BuildTableEntry>();
        }

        public class BuildTableEntry
        {
            public CacheBuild Build { get; set; }
            public string Path { get; set; }

            public BuildTableEntry(CacheBuild build, string path)
            {
                Build = build;
                Path = path;
            }
        }

        public static void GenerateProperties(string outputPath)
        {
            var properties = new BuildTableProperties();

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
                CacheBuild.All,
            };

            foreach (CacheBuild build in Enum.GetValues(typeof(CacheBuild)))
            {
                if (filter.Contains(build))
                    continue;

                properties.Builds.Add(new BuildTableEntry(build, null));
            }

            var handler = new BuildTablePropertiesHandler();

            var outputObject = handler.Serialize(properties);

            File.WriteAllText(outputPath, outputObject);
        }
    }
}
