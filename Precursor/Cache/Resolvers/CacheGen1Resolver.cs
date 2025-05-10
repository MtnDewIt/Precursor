using Precursor.Cache.Objects;
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
        public List<string> HaloXboxFiles { get; set; }
        public List<string> HaloXboxSharedFiles { get; set; }
        public List<string> HaloPCFiles { get; set; }
        public List<string> HaloPCSharedFiles { get; set; }
        public List<string> HaloCustomEditionFiles { get; set; }
        public List<string> HaloCustomEditionSharedFiles { get; set; }

        public static List<string> SharedFiles = new List<string>
        {
            "bitmaps.map",
            "loc.map",
            "sounds.map"
        };

        public CacheGen1Resolver()
        {
            HaloXboxFiles = new List<string>();
            HaloXboxSharedFiles = new List<string>();
            HaloPCFiles = new List<string>();
            HaloPCSharedFiles = new List<string>();
            HaloCustomEditionFiles = new List<string>();
            HaloCustomEditionSharedFiles = new List<string>();
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
                                    HaloXboxFiles.Add(cacheFile);
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
                                    HaloPCFiles.Add(cacheFile);
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
                                    HaloCustomEditionFiles.Add(cacheFile);
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
                else 
                {
                    switch (build.Build) 
                    {
                        case CacheBuild.HaloXbox:
                            HaloXboxSharedFiles.Add(cacheFile);
                            break;
                        case CacheBuild.HaloPC:
                            HaloPCSharedFiles.Add(cacheFile);
                            break;
                        case CacheBuild.HaloCustomEdition:
                            HaloCustomEditionSharedFiles.Add(cacheFile);
                            break;
                    }
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
