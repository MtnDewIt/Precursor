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
    public class HaloXboxInfo : BuildTableEntry
    {
        public override CacheBuild Build => CacheBuild.HaloXbox;
        public override CacheVersion Version => CacheVersion.HaloXbox;
        public override CachePlatform Platform => CachePlatform.Original;
        public override CacheGeneration Generation => CacheGeneration.Gen1;

        public override string ResourcePath => @"Resources\Gen1\HaloXbox";

        public override List<string> BuildStrings => new List<string> 
        { 
            "01.10.12.2276" 
        };

        public override List<string> CacheFiles => null;
        public override List<string> SharedFiles => null;
        public override List<string> ResourceFiles => null;

        public override bool VerifyBuildInfo(BuildTableConfig.BuildTableEntry build)
        {
            var files = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories);

            if (!ParseFileCount(files.Count()))
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

            Console.WriteLine($"Successfully Verified {validFiles}/{files.Count()} Files\n");

            return true;
        }
    }
}
