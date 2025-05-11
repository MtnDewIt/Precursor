using Precursor.Cache.BuildTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.IO;

namespace Precursor.Cache.Resolvers
{
    public class CacheGen2Resolver : CacheResolver
    {
        public static List<string> SharedFiles = new List<string>
        {
            "shared.map",
            "single_player_shared.map"
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
                            case CacheBuild.Halo2Alpha:
                                if (mapFile.Header.GetBuild() == "02.01.07.4998")
                                {
                                    validFiles++;
                                }
                                else
                                {
                                    Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                    continue;
                                }
                                break;
                            case CacheBuild.Halo2Beta:
                                if (mapFile.Header.GetBuild() == "02.06.28.07902")
                                {
                                    validFiles++;
                                }
                                else
                                {
                                    Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                    continue;
                                }
                                break;
                            case CacheBuild.Halo2Xbox:
                                if (mapFile.Header.GetBuild() == "02.09.27.09809")
                                {
                                    validFiles++;
                                }
                                else
                                {
                                    Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                    continue;
                                }
                                break;
                            case CacheBuild.Halo2Vista:
                                if (mapFile.Header.GetBuild() == "11081.07.04.30.0934.main")
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
