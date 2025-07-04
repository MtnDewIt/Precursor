using Precursor.Cache.BuildTable;
using Precursor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.IO;

namespace Precursor.Cache.BuildInfo.Gen1
{
    public class HaloXboxInfo : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.HaloXbox;

        public static readonly CacheVersion Version = CacheVersion.HaloXbox;

        public static readonly CachePlatform Platform = CachePlatform.Original;

        public static readonly CacheGeneration Generation = CacheGeneration.Gen1;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "01.10.12.2276" 
        };

        public List<string> CurrentCacheFiles;

        public HaloXboxInfo()
        {
            CurrentCacheFiles = new List<string>();
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
        public override List<string> GetSharedFiles() => null;
        public override List<string> GetResourceFiles() => null;

        public override List<string> GetCurrentMapFiles() => null;
        public override List<string> GetCurrentCacheFiles() => CurrentCacheFiles;
        public override List<string> GetCurrentSharedFiles() => null;
        public override List<string> GetCurrentResourceFiles() => null;
    }
}
