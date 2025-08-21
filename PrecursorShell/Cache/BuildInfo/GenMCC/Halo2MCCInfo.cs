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
    public class Halo2MCCInfo : BuildTableEntry
    {
        public override CacheBuild Build => CacheBuild.Halo2MCC;
        public override CacheVersion Version => CacheVersion.Halo2PC;
        public override CachePlatform Platform => CachePlatform.MCC;
        public override CacheGeneration Generation => CacheGeneration.GenMCC;

        public override string ResourcePath => @"Resources\GenMCC\Halo2MCC";

        public override List<string> BuildStrings => new List<string>
        {
            // No build string, srsly 343?
            ""
        };

        public override List<string> CacheFiles => null;
        public override List<string> SharedFiles => new List<string>
        {
            "shared.map",
            "single_player_shared.map"
        };
        public override List<string> ResourceFiles => new List<string> 
        {
            "sounds_cht.dat",
            "sounds_de.dat",
            "sounds_en.dat",
            "sounds_fr.dat",
            "sounds_it.dat",
            "sounds_jpn.dat",
            "sounds_kor.dat",
            "sounds_neutral.dat",
            "sounds_remastered.dat",
            "sounds_sp.dat",
            "textures.dat"
        };

        public override List<string> CurrentMapFiles => null;
        public override List<string> CurrentCacheFiles => new List<string>();
        public override List<string> CurrentSharedFiles => new List<string>();
        public override List<string> CurrentResourceFiles => new List<string>();

        public override bool VerifyBuildInfo(BuildTableConfig.BuildTableEntry build)
        {
            var files = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories).ToList();
            var resourceFiles = Directory.EnumerateFiles(build.Path, "*.dat", SearchOption.AllDirectories).ToList();

            if (!ParseFileCount(files.Count))
            {
                return false;
            }

            var totalFileCount = files.Count + resourceFiles.Count;
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

            foreach (var file in resourceFiles)
            {
                CurrentResourceFiles.Add(file);
                validFiles++;
            }

            ParseSharedFiles();
            ParseResourceFiles();

            Console.WriteLine($"Successfully Verified {validFiles}/{totalFileCount} Files\n");

            return true;
        }
    }
}
