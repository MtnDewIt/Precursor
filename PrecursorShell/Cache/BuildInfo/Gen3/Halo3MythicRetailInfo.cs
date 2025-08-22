using PrecursorShell.Cache.BuildTable;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.IO;

namespace PrecursorShell.Cache.BuildInfo.Gen3
{
    public class Halo3MythicRetailInfo : BuildTableEntry
    {
        public override CacheBuild Build => CacheBuild.Halo3MythicRetail;
        public override CacheVersion Version => CacheVersion.Halo3Retail;
        public override CachePlatform Platform => CachePlatform.Original;
        public override CacheGeneration Generation => CacheGeneration.Gen3;

        public override string ResourcePath => @"Resources\Gen3\Halo3MythicRetail";

        public override List<string> BuildStrings => new List<string> 
        { 
            "12065.08.08.26.0819.halo3_ship",
            "11855.07.08.20.2317.halo3_ship"
        };

        public override List<string> CacheFiles => null;
        public override List<string> SharedFiles => new List<string>
        {
            "shared.map"
        };
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
                            new PrecursorWarning($"Invalid Build String: {fileInfo.Name} - {mapFile.Header.GetBuild()}\n" +
                                $"\nValid Build Strings:\n" +
                                $"{string.Join("\n", BuildStrings)}\n");
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

            ParseFiles(SharedFiles, CurrentSharedFiles);

            Console.WriteLine($"Successfully Verified {validFiles}/{files.Count()} Files\n");

            return true;
        }
    }
}
