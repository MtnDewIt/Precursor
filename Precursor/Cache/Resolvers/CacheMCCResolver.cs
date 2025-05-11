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
        public List<string> Halo1Files { get; set; }
        public List<string> Halo1SharedFiles { get; set; }
        public List<string> Halo2Files { get; set; }
        public List<string> Halo2SharedFiles { get; set; }
        public List<string> Halo3Files { get; set; }
        public List<string> Halo3SharedFiles { get; set; }
        public List<string> Halo3ODSTFiles { get; set; }
        public List<string> Halo3ODSTSharedFiles { get; set; }
        public List<string> HaloReachFiles { get; set; }
        public List<string> HaloReachSharedFiles { get; set; }
        public List<string> Halo4Files { get; set; }
        public List<string> Halo4SharedFiles { get; set; }
        public List<string> Halo2AMPFiles { get; set; }
        public List<string> Halo2AMPSharedFiles { get; set; }

        public CacheMCCResolver()
        {
            Halo1Files = new List<string>();
            Halo1SharedFiles = new List<string>();
            Halo2Files = new List<string>();
            Halo2SharedFiles = new List<string>();
            Halo3Files = new List<string>();
            Halo3SharedFiles = new List<string>();
            Halo3ODSTFiles = new List<string>();
            Halo3ODSTSharedFiles = new List<string>();
            HaloReachFiles = new List<string>();
            HaloReachSharedFiles = new List<string>();
            Halo4Files = new List<string>();
            Halo4SharedFiles = new List<string>();
            Halo2AMPFiles = new List<string>();
            Halo2AMPSharedFiles = new List<string>();
        }

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
            if (string.IsNullOrEmpty(build.Path) || !Path.Exists(build.Path))
            {
                Console.WriteLine($"> Build Type: {build.Build} - Invalid or Missing Path, Skipping Verification...");
                return;
            }

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
                            Halo1Files.Add(cacheFile);
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
                else
                {
                    Halo1SharedFiles.Add(cacheFile);
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
                            Halo2Files.Add(cacheFile);
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
                else
                {
                    Halo2SharedFiles.Add(cacheFile);
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
                            Halo3Files.Add(cacheFile);
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
                else
                {
                    Halo3SharedFiles.Add(cacheFile);
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
                            Halo3ODSTFiles.Add(cacheFile);
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
                else
                {
                    Halo3ODSTSharedFiles.Add(cacheFile);
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
                            HaloReachFiles.Add(cacheFile);
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
                else
                {
                    HaloReachSharedFiles.Add(cacheFile);
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
                            Halo4Files.Add(cacheFile);
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
                else
                {
                    Halo4SharedFiles.Add(cacheFile);
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
                            Halo2AMPFiles.Add(cacheFile);
                            validFiles++;
                        }
                        else
                        {
                            Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                            continue;
                        }
                    }
                }
                else
                {
                    Halo2AMPSharedFiles.Add(cacheFile);
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
