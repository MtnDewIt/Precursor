using Precursor.Cache.BuildTable;
using Precursor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.IO;

namespace Precursor.Cache.BuildInfo.Gen1
{
    public class HaloCustomEditionInfo : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.HaloCustomEdition;

        public static readonly CacheGeneration Generation = CacheGeneration.Gen1;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "01.00.00.0609" 
        };

        public static readonly List<string> SharedFiles = new List<string> 
        {
            "bitmaps.map",
            "loc.map",
            "sounds.map"
        };

        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public HaloCustomEditionInfo()
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
                            new PrecursorWarning($"Invalid Build String: {Path.GetFileName(file)}");
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
        public override CacheGeneration GetGeneration() => Generation;

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
