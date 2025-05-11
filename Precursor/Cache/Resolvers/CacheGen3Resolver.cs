using Precursor.Cache.BuildTable;
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
        public static List<string> SharedFiles = new List<string>
        {
            "campaign.map",
            "shared.map"
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
                                    validFiles++;
                                }
                                else if (mapFile.Header.GetBuild() == "11855.07.08.20.2317.halo3_ship" && mapFile.Header.GetName() != "mainmenu")
                                {
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
            }

            Console.WriteLine($"Successfully Verified {validFiles}/{cacheFileCount} Files\n");
        }

        public override bool IsValidCacheFile()
        {
            return false;
        }
    }
}
