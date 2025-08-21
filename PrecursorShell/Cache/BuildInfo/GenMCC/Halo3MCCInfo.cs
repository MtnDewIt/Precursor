using PrecursorShell.Cache.BuildTable;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.IO;

namespace PrecursorShell.Cache.BuildInfo.GenMCC
{
    public class Halo3MCCInfo : BuildTableEntry
    {
        public override CacheBuild Build => CacheBuild.Halo3MCC;
        public override CacheVersion Version => CacheVersion.Halo3Retail;
        public override CachePlatform Platform => CachePlatform.MCC;
        public override CacheGeneration Generation => CacheGeneration.GenMCC;

        public override string ResourcePath => @"Resources\GenMCC\Halo3MCC";

        public override List<string> BuildStrings => new List<string>
        {
            "Dec 21 2023 22:31:37"
        };

        public override List<string> CacheFiles => null;
        public override List<string> SharedFiles => new List<string>
        {
            "campaign.map",
            "shared.map"
        };
        public override List<string> ResourceFiles => null;

        public override List<string> CurrentMapFiles { get; set; }
        public override List<string> CurrentCacheFiles { get; set; }
        public override List<string> CurrentSharedFiles { get; set; }
        public override List<string> CurrentResourceFiles { get; set; }

        public Halo3MCCInfo()
        {
            CurrentCacheFiles = [];
            CurrentSharedFiles = [];
        }

        public override bool VerifyBuildInfo(BuildTableConfig.BuildTableEntry build)
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
    }
}
