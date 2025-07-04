using Precursor.Cache.BuildTable;
using Precursor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.IO;

namespace Precursor.Cache.BuildInfo.Gen2
{
    public class Halo2AlphaInfo : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.Halo2Alpha;

        public static readonly CacheVersion Version = CacheVersion.Halo2Alpha;

        public static readonly CachePlatform Platform = CachePlatform.Original;

        public static readonly CacheGeneration Generation = CacheGeneration.Gen2;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "02.01.07.4998" 
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "shared.map"
        };

        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public Halo2AlphaInfo()
        {
            CurrentCacheFiles = new List<string>();
            CurrentSharedFiles = new List<string>();
        }

        public override bool VerifyBuildInfo(BuildTableProperties.BuildTableEntry build) 
        {
            var files = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories).ToList();

            if (!ParseFileCount(files.Count))
            {
                return false;
            }

            var validFiles = 0;

            foreach (var file in files) 
            {
                if (!SharedFiles.Contains(Path.GetFileName(file)))
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

                if (SharedFiles.Contains(Path.GetFileName(file)))
                {
                    CurrentSharedFiles.Add(file);
                    validFiles++;
                }
            }

            ParseSharedFiles();

            Console.WriteLine($"Successfully Verified {validFiles}/{files.Count} Files\n");

            return true;
        }

        public override CacheBuild GetBuild() => Build;
        public override CacheVersion GetVersion() => Version;
        public override CachePlatform GetPlatform() => Platform;
        public override CacheGeneration GetGeneration() => Generation;

        public override string GetResourcePath() => null;

        public override List<string> GetBuildStrings() => BuildStrings;

        public override List<string> GetCacheFiles() => null;
        public override List<string> GetSharedFiles() => SharedFiles;
        public override List<string> GetResourceFiles() => null;

        public override List<string> GetCurrentMapFiles() => null;
        public override List<string> GetCurrentCacheFiles() => CurrentCacheFiles;
        public override List<string> GetCurrentSharedFiles() => CurrentSharedFiles;
        public override List<string> GetCurrentResourceFiles() => null;
    }
}
