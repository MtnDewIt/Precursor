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
    public class CacheGen3Resolver
    {
        public List<string> Halo3BetaFiles { get; set; }
        public List<string> Halo3RetailFiles { get; set; }
        public List<string> Halo3MythicRetailFiles { get; set; }
        public List<string> Halo3ODSTFiles { get; set; }
        public List<string> HaloReachFiles { get; set; }
        public List<string> HaloReach11883Files { get; set; }

        public CacheGen3Resolver()
        {
            Halo3BetaFiles = new List<string>();
            Halo3RetailFiles = new List<string>();
            Halo3MythicRetailFiles = new List<string>();
            Halo3ODSTFiles = new List<string>();
            HaloReachFiles = new List<string>();
            HaloReach11883Files = new List<string>();
        }

        public void VerifyBuild(CacheObject.CacheBuildObject build)
        {
            if (string.IsNullOrEmpty(build.Path))
            {
                Console.WriteLine($"> Cache Type: {build.Build} - Null or Empty Path Detected, Skipping Verification...");
                return;
            }
            else if (!Path.Exists(build.Path))
            {
                Console.WriteLine($"> Cache Type: {build.Build} - Unable to Locate Directory, Skipping Verification...");
                return;
            }
            else
            {
                var cacheFiles = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories).ToList();

                if (cacheFiles.Count == 0)
                {
                    Console.WriteLine($"> Cache Type: {build.Build} - No .Map Files Found in Directory, Skipping Verification...");
                    return;
                }

                var validFiles = 0;

                foreach (var cacheFile in cacheFiles)
                {
                    var fileInfo = new FileInfo(cacheFile);

                    using (var stream = fileInfo.OpenRead())
                    {
                        using (var reader = new EndianReader(stream))
                        {
                            var mapFile = new MapFile();

                            mapFile.Read(reader);

                            if (!mapFile.Header.IsValid())
                            {
                                Console.WriteLine($"> Cache Type: {build.Build} - Invalid Cache File");
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
                                        Console.WriteLine($"> Cache Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
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
                                        Console.WriteLine($"> Cache Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
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
                                        Console.WriteLine($"> Cache Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
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
                                        Console.WriteLine($"> Cache Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
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
                                        Console.WriteLine($"> Cache Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                // This might need to be handled differently
                                case CacheBuild.HaloReach11883:
                                    if (mapFile.Header.GetBuild() == "11883.10.10.25.1227.dlc_1_ship__tag_test")
                                    {
                                        HaloReach11883Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Cache Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                            }
                        }
                    }
                }

                Console.WriteLine($"> Cache Type: {build.Build} - Successfully Verified {validFiles}/{cacheFiles.Count} Files");
            }
        }
    }
}
