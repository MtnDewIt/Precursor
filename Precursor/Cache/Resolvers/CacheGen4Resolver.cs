using Precursor.Cache.BuildTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.IO;

namespace Precursor.Cache.Resolvers
{
    public class CacheGen4Resolver : CacheResolver
    {
        public static List<string> SharedFiles = new List<string>
        {
            "campaign.map",
            "shared.map"
        };

        public static List<string> RetailBuilds = new List<string> 
        {
            "20810.12.09.22.1647.main",
            "21122.12.11.21.0101.main",
            "21165.12.12.12.0112.main",
            "21339.13.02.05.0117.main",
            "21391.13.03.13.1711.main"
        };

        public override void VerifyBuild(BuildTableProperties.BuildTableEntry build)
        {
            var cacheFiles = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories).ToList();

            if (cacheFiles.Count == 0)
            {
                Console.WriteLine($"> Build Type: {build.Build} - No .Map Files Found in Directory, Skipping Verification...");
                return;
            }

            var validFiles = 0;

            foreach (var cacheFile in cacheFiles)
            {
                if (!SharedFiles.Contains(Path.GetFileName(cacheFile)))
                {
                    var fileInfo = new FileInfo(cacheFile);

                    using (var stream = fileInfo.OpenRead())
                    using (var reader = new EndianReader(stream))
                    {
                        var mapFile = new MapFile();

                        mapFile.Read(reader);

                        if (!mapFile.Header.IsValid())
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - Invalid Cache File");
                            continue;
                        }

                        switch (build.Build)
                        {
                            case CacheBuild.Halo4Retail:
                                if (RetailBuilds.Contains(mapFile.Header.GetBuild()))
                                {
                                    validFiles++;
                                }
                                else
                                {
                                    Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                    continue;
                                }
                                break;
                        }
                    }
                }
            }

            Console.WriteLine($"Successfully Verified {validFiles}/{cacheFiles.Count} Files\n");
        }

        public override bool IsValidCacheFile()
        {
            return false;
        }
    }
}
