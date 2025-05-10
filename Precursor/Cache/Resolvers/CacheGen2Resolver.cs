using Precursor.Cache.Objects;
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
        public List<string> Halo2AlphaFiles { get; set; }
        public List<string> Halo2AlphaSharedFiles { get; set; }
        public List<string> Halo2BetaFiles { get; set; }
        public List<string> Halo2BetaSharedFiles { get; set; }
        public List<string> Halo2XboxFiles { get; set; }
        public List<string> Halo2XboxSharedFiles { get; set; }
        public List<string> Halo2VistaFiles { get; set; }
        public List<string> Halo2VistaSharedFiles { get; set; }

        public static List<string> SharedFiles = new List<string>
        {
            "shared.map",
            "single_player_shared.map"
        };

        public CacheGen2Resolver()
        {
            Halo2AlphaFiles = new List<string>();
            Halo2AlphaSharedFiles = new List<string>();
            Halo2BetaFiles = new List<string>();
            Halo2BetaSharedFiles = new List<string>();
            Halo2XboxFiles = new List<string>();
            Halo2XboxSharedFiles = new List<string>();
            Halo2VistaFiles = new List<string>();
            Halo2VistaSharedFiles = new List<string>();
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
                            case CacheBuild.Halo2Alpha:
                                if (mapFile.Header.GetBuild() == "02.01.07.4998")
                                {
                                    Halo2AlphaFiles.Add(cacheFile);
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
                                    Halo2BetaFiles.Add(cacheFile);
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
                                    Halo2XboxFiles.Add(cacheFile);
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
                                    Halo2VistaFiles.Add(cacheFile);
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
                        case CacheBuild.Halo2Alpha:
                            Halo2AlphaSharedFiles.Add(cacheFile);
                            break;
                        case CacheBuild.Halo2Beta:
                            Halo2BetaSharedFiles.Add(cacheFile);
                            break;
                        case CacheBuild.Halo2Xbox:
                            Halo2XboxSharedFiles.Add(cacheFile);
                            break;
                        case CacheBuild.Halo2Vista:
                            Halo2VistaSharedFiles.Add(cacheFile);
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
