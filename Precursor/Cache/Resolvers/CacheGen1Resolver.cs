using Precursor.Cache.BuildTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.IO;

namespace Precursor.Cache.Resolvers
{
    public class CacheGen1Resolver : CacheResolver
    {
        public static List<string> SharedFiles = new List<string>
        {
            "bitmaps.map",
            "loc.map",
            "sounds.map"
        };

        public override void VerifyBuild(BuildTableProperties.BuildTableEntry build) 
        {
            var cacheFiles = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories).ToList();

            if (cacheFiles.Count == 0)
            {
                Console.WriteLine($"> Build Type: {build.Build} - No .Map Files Found in Directory, Skipping Verification...");
                return;
            }

            var cacheFileCount = 0;
            var validFiles = 0;

            foreach (var cacheFile in cacheFiles)
            {
                if (!SharedFiles.Contains(Path.GetFileName(cacheFile)))
                {
                    var file = new FileInfo(cacheFile);

                    using (var stream = file.OpenRead())
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
                            case CacheBuild.HaloXbox:
                                if (mapFile.Header.GetBuild() == "01.10.12.2276")
                                {
                                    validFiles++;
                                }
                                else
                                {
                                    Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                    continue;
                                }
                                break;
                            case CacheBuild.HaloPC:
                                if (mapFile.Header.GetBuild() == "01.00.00.0564")
                                {
                                    validFiles++;
                                }
                                else
                                {
                                    Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                    continue;
                                }
                                break;
                            case CacheBuild.HaloCustomEdition:
                                if (mapFile.Header.GetBuild() == "01.00.00.0609")
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

                    cacheFileCount++;
                }
            }

            Console.WriteLine($"Successfully Verified {validFiles}/{cacheFileCount} Files\n");
        }

        public override bool IsValidCacheFile()
        {
            return false;
        }
    }
}
