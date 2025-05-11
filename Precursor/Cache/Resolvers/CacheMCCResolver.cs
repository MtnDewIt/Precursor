using Precursor.Cache.BuildTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.IO;

namespace Precursor.Cache.Resolvers
{
    public class CacheMCCResolver : CacheResolver
    {
        public static List<string> SharedFiles = new List<string>
        {
            "bitmaps.map",
            "loc.map",
            "sounds.map",
            "campaign.map",
            "shared.map",
            "single_player_shared.map"
        };

        public override void VerifyBuild(BuildTableProperties.BuildTableEntry build)
        {
            var halo1CacheFiles = Directory.EnumerateFiles($@"{build.Path}\halo1\maps", "*.map", SearchOption.AllDirectories).ToList();
            var halo2CacheFiles = Directory.EnumerateFiles($@"{build.Path}\halo2\h2_maps_win64_dx11", "*.map", SearchOption.AllDirectories).ToList();
            var halo3CacheFiles = Directory.EnumerateFiles($@"{build.Path}\halo3\maps", "*.map", SearchOption.AllDirectories).ToList();
            var halo3OdstCacheFiles = Directory.EnumerateFiles($@"{build.Path}\halo3odst\maps", "*.map", SearchOption.AllDirectories).ToList();
            var haloReachCacheFiles = Directory.EnumerateFiles($@"{build.Path}\haloreach\maps", "*.map", SearchOption.AllDirectories).ToList();
            var halo4CacheFiles = Directory.EnumerateFiles($@"{build.Path}\halo4\maps", "*.map", SearchOption.AllDirectories).ToList();
            var halo2AMPCacheFiles = Directory.EnumerateFiles($@"{build.Path}\groundhog\maps", "*.map", SearchOption.AllDirectories).ToList();

            var totalFileCount = 0;
            var validFiles = 0;

            foreach (var cacheFile in halo1CacheFiles)
            {
                if (!SharedFiles.Contains(Path.GetFileName(cacheFile)))
                {
                    totalFileCount++;

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

                        if (mapFile.Header.GetBuild() == "01.03.43.0000")
                        {
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
            }

            foreach (var cacheFile in halo2CacheFiles)
            {
                if (!SharedFiles.Contains(Path.GetFileName(cacheFile)))
                {
                    totalFileCount++;

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

                        if (mapFile.Header.GetBuild() == "")
                        {
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
            }

            foreach (var cacheFile in halo3CacheFiles)
            {
                if (!SharedFiles.Contains(Path.GetFileName(cacheFile)))
                {
                    totalFileCount++;

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

                        if (mapFile.Header.GetBuild() == "Dec 21 2023 22:31:37")
                        {
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
            }

            foreach (var cacheFile in halo3OdstCacheFiles)
            {
                if (!SharedFiles.Contains(Path.GetFileName(cacheFile)))
                {
                    totalFileCount++;

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

                        if (mapFile.Header.GetBuild() == "May 16 2023 11:44:41")
                        {
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
            }

            foreach (var cacheFile in haloReachCacheFiles)
            {
                if (!SharedFiles.Contains(Path.GetFileName(cacheFile)))
                {
                    totalFileCount++;

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

                        if (mapFile.Header.GetBuild() == "Jun 21 2023 15:35:31")
                        {
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
            }

            foreach (var cacheFile in halo4CacheFiles)
            {
                if (!SharedFiles.Contains(Path.GetFileName(cacheFile)))
                {
                    totalFileCount++;

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

                        if (mapFile.Header.GetBuild() == "Apr  1 2023 17:35:22")
                        {
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
            }

            foreach (var cacheFile in halo2AMPCacheFiles)
            {
                if (!SharedFiles.Contains(Path.GetFileName(cacheFile)))
                {
                    totalFileCount++;

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

                        if (mapFile.Header.GetBuild() == "Jun 13 2023 20:21:18")
                        {
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
            }

            Console.WriteLine($"Successfully Verified {validFiles}/{totalFileCount} Files\n");
        }

        public override bool IsValidCacheFile()
        {
            return false;
        }
    }
}
