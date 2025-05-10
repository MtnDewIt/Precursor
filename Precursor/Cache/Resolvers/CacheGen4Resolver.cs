using Precursor.Cache.Objects;
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
        public List<string> Halo4RetailFiles { get; set; }
        public List<string> Halo4RetailSharedFiles { get; set; }

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

        public CacheGen4Resolver()
        {
            Halo4RetailFiles = new List<string>();
            Halo4RetailSharedFiles = new List<string>();
        }

        public override void VerifyBuild(CacheObject.CacheBuildObject build)
        {
            if (string.IsNullOrEmpty(build.Path) || !Path.Exists(build.Path))
            {
                Console.WriteLine($"> Build Type: {build.Build} - Invalid or Missing Path, Skipping Verification...");
                return;
            }

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
                                    Halo4RetailFiles.Add(cacheFile);
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
                else 
                {
                    switch (build.Build) 
                    {
                        case CacheBuild.Halo4Retail:
                            Halo4RetailSharedFiles.Add(cacheFile);
                            break;
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
