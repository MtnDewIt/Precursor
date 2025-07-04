using Precursor.Cache.BuildTable;
using Precursor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.IO;

namespace Precursor.Cache.BuildInfo.GenHaloOnline
{
    public class HaloOnline571627Info : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.HaloOnline571627;

        public static readonly CacheVersion Version = CacheVersion.HaloOnline571627;

        public static readonly CachePlatform Platform = CachePlatform.Original;

        public static readonly CacheGeneration Generation = CacheGeneration.GenHaloOnline;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "11.1.571627 Live"
        };

        public static readonly List<string> CacheFiles = new List<string>
        {
            "tags.dat",
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "audio.dat",
            "lightmaps.dat",
            "render_models.dat",
            "resources.dat",
            "string_ids.dat",
            "textures.dat",
            "textures_b.dat",
            "video.dat"
        };

        public List<string> CurrentMapFiles;
        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public HaloOnline571627Info()
        {
            CurrentMapFiles = new List<string>();
            CurrentCacheFiles = new List<string>();
            CurrentSharedFiles = new List<string>();
        }

        public override bool VerifyBuildInfo(BuildTableProperties.BuildTableEntry build)
        {
            var files = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories).ToList();
            var sharedFiles = Directory.EnumerateFiles(build.Path, "*.dat", SearchOption.AllDirectories).ToList();

            if (!ParseFileCount(files.Count))
            {
                return false;
            }

            var totalFileCount = files.Count + sharedFiles.Count;
            var validFiles = 0;

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                using (var stream = fileInfo.OpenRead())
                using (var reader = new EndianReader(stream))
                {
                    var mapFile = new MapFile();

                    mapFile.Read(reader);

                    if (!mapFile.Header.IsValid())
                    {
                        new PrecursorWarning($"Invalid Cache File: {Path.GetFileName(file)}");
                        continue;
                    }

                    if (BuildStrings.Contains(mapFile.Header.GetBuild()))
                    {
                        CurrentCacheFiles.Add(file);
                        validFiles++;
                    }
                    else
                    {
                        new PrecursorWarning($"Invalid Build String: {Path.GetFileName(file)} - {mapFile.Header.GetBuild()} != {BuildStrings.FirstOrDefault()}");
                        continue;
                    }
                }
            }

            foreach (var file in sharedFiles)
            {
                var fileInfo = new FileInfo(file);

                using (var stream = fileInfo.OpenRead())
                using (var reader = new EndianReader(stream))
                {
                    if (Path.GetFileName(file) == "tags.dat")
                    {
                        //TODO: Verify Creation Date

                        CurrentCacheFiles.Add(file);
                        validFiles++;
                    }
                    else if (Path.GetFileName(file) == "string_ids.dat")
                    {
                        CurrentSharedFiles.Add(file);
                        validFiles++;
                    }
                    else
                    {
                        //TODO: Verify Creation Date

                        CurrentSharedFiles.Add(file);
                        validFiles++;
                    }
                }
            }

            ParseCacheFiles();
            ParseSharedFiles();

            Console.WriteLine($"Successfully Verified {validFiles}/{totalFileCount} Files\n");

            return true;
        }

        public override CacheBuild GetBuild() => Build;
        public override CacheVersion GetVersion() => Version;
        public override CachePlatform GetPlatform() => Platform;
        public override CacheGeneration GetGeneration() => Generation;

        public override string GetResourcePath() => null;

        public override List<string> GetBuildStrings() => BuildStrings;

        public override List<string> GetCacheFiles() => CacheFiles;
        public override List<string> GetSharedFiles() => SharedFiles;
        public override List<string> GetResourceFiles() => null;

        public override List<string> GetCurrentMapFiles() => CurrentMapFiles;
        public override List<string> GetCurrentCacheFiles() => CurrentCacheFiles;
        public override List<string> GetCurrentSharedFiles() => CurrentSharedFiles;
        public override List<string> GetCurrentResourceFiles() => null;
    }
}
