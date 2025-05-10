using Precursor.Cache.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagTool.BlamFile;
using TagTool.IO;

namespace Precursor.Cache.Resolvers
{
    public class CacheGen3Resolver : CacheResolver
    {
        public List<string> Halo3BetaFiles { get; set; }
        public List<string> Halo3BetaSharedFiles { get; set; }
        public List<string> Halo3RetailFiles { get; set; }
        public List<string> Halo3RetailSharedFiles { get; set; }
        public List<string> Halo3MythicRetailFiles { get; set; }
        public List<string> Halo3MythicRetailSharedFiles { get; set; }
        public List<string> Halo3ODSTFiles { get; set; }
        public List<string> Halo3ODSTSharedFiles { get; set; }
        public List<string> HaloReachFiles { get; set; }
        public List<string> HaloReachSharedFiles { get; set; }
        public List<string> HaloReach11883Files { get; set; }
        public List<string> HaloReach11883SharedFiles { get; set; }


        public static List<string> SharedFiles = new List<string>
        {
            "campaign.map",
            "shared.map"
        };

        public CacheGen3Resolver()
        {
            Halo3BetaFiles = new List<string>();
            Halo3BetaSharedFiles = new List<string>();
            Halo3RetailFiles = new List<string>();
            Halo3RetailSharedFiles = new List<string>();
            Halo3MythicRetailFiles = new List<string>();
            Halo3MythicRetailSharedFiles = new List<string>();
            Halo3ODSTFiles = new List<string>();
            Halo3ODSTSharedFiles = new List<string>();
            HaloReachFiles = new List<string>();
            HaloReachSharedFiles = new List<string>();
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
                            case CacheBuild.Halo3Beta:
                                if (mapFile.Header.GetBuild() == "09699.07.05.01.1534.delta")
                                {
                                    Halo3BetaFiles.Add(cacheFile);
                                    validFiles++;
                                }
                                else
                                {
                                    Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                    continue;
                                }
                                break;
                            case CacheBuild.Halo3Retail:
                                if (mapFile.Header.GetBuild() == "11855.07.08.20.2317.halo3_ship")
                                {
                                    Halo3RetailFiles.Add(cacheFile);
                                    validFiles++;
                                }
                                else
                                {
                                    Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                    continue;
                                }
                                break;
                            case CacheBuild.Halo3MythicRetail:
                                if (mapFile.Header.GetBuild() == "12065.08.08.26.0819.halo3_ship")
                                {
                                    Halo3MythicRetailFiles.Add(cacheFile);
                                    validFiles++;
                                }
                                else if (mapFile.Header.GetBuild() == "11855.07.08.20.2317.halo3_ship" && mapFile.Header.GetName() != "mainmenu")
                                {
                                    Halo3MythicRetailFiles.Add(cacheFile);
                                    validFiles++;
                                }
                                else
                                {
                                    Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                    continue;
                                }
                                break;
                            case CacheBuild.Halo3ODST:
                                if (mapFile.Header.GetBuild() == "13895.09.04.27.2201.atlas_relea")
                                {
                                    Halo3ODSTFiles.Add(cacheFile);
                                    validFiles++;
                                }
                                else
                                {
                                    Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                    continue;
                                }
                                break;
                            case CacheBuild.HaloReach:
                                if (mapFile.Header.GetBuild() == "11860.10.07.24.0147.omaha_relea")
                                {
                                    HaloReachFiles.Add(cacheFile);
                                    validFiles++;
                                }
                                else
                                {
                                    Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                    continue;
                                }
                                break;
                        }

                        cacheFileCount++;
                    }
                }
                else 
                {
                    switch (build.Build) 
                    {
                        case CacheBuild.Halo3Beta:
                            Halo3BetaSharedFiles.Add(cacheFile);
                            break;
                        case CacheBuild.Halo3Retail:
                            Halo3RetailSharedFiles.Add(cacheFile);
                            break;
                        case CacheBuild.Halo3MythicRetail:
                            Halo3MythicRetailSharedFiles.Add(cacheFile);
                            break;
                        case CacheBuild.Halo3ODST:
                            Halo3ODSTSharedFiles.Add(cacheFile);
                            break;
                        case CacheBuild.HaloReach:
                            HaloReachSharedFiles.Add(cacheFile);
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
