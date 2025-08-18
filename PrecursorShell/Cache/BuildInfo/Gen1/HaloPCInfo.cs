using PrecursorShell.Cache.BuildTable;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.IO;

namespace PrecursorShell.Cache.BuildInfo.Gen1
{
    public class HaloPCInfo : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.HaloPC;

        public static readonly CacheVersion Version = CacheVersion.HaloPC;

        public static readonly CachePlatform Platform = CachePlatform.Original;

        public static readonly CacheGeneration Generation = CacheGeneration.Gen1;
        
        public static readonly string ResourcePath = @"Resources\Gen1\HaloPC";

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "01.00.00.0564" 
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "bitmaps.map",
            "sounds.map"
        };

        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public HaloPCInfo()
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
                var fileInfo = new FileInfo(file);

                if (!SharedFiles.Contains(fileInfo.Name))
                {
                    using (var stream = fileInfo.OpenRead())
                    using (var reader = new EndianReader(stream))
                    {
                        var mapFile = new MapFile();

                        try
                        {
                            mapFile.Read(reader);
                        }
                        catch (Exception ex)
                        {
                            new PrecursorWarning($"Failed to parse file \"{fileInfo.Name}\": {ex.Message}");
                            continue;
                        }

                        if (!mapFile.Header.IsValid())
                        {
                            new PrecursorWarning($"Invalid Cache File: {fileInfo.Name}");
                            continue;
                        }

                        if (BuildStrings.Contains(mapFile.Header.GetBuild()))
                        {
                            try
                            {
                                GenerateJSON(mapFile, fileInfo.Name, ResourcePath);
                            }
                            catch (Exception ex)
                            {
                                new PrecursorWarning($"Failed to serialize JSON \"{fileInfo.Name}\": {ex.Message}");
                                continue;
                            }

                            CurrentCacheFiles.Add(file);
                            validFiles++;
                        }
                        else
                        {
                            new PrecursorWarning($"Invalid Build String: {fileInfo.Name} - {mapFile.Header.GetBuild()} != {BuildStrings.FirstOrDefault()}");
                            continue;
                        }
                    }
                }

                if (SharedFiles.Contains(fileInfo.Name)) 
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

        public override string GetResourcePath() => ResourcePath;

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
