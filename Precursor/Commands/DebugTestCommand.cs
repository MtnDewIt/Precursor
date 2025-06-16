using Precursor.Commands.Context;
using Precursor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.IO;
using TagTool.JSON.Handlers;
using TagTool.JSON.Objects;

namespace Precursor.Commands
{
    class DebugTestCommand : PrecursorCommand
    {
        public GameCache Cache { get; set; }
        public GameCacheHaloOnlineBase CacheContext { get; set; }
        public PrecursorContextStack ContextStack { get; set; }

        public DebugTestCommand(GameCache cache, GameCacheHaloOnlineBase cacheContext, PrecursorContextStack contextStack) : base
        (
            false,
            "DebugTest",
            "Self Explanatory",

            "DebugTest",
            "Self Explanatory"
        )
        {
            Cache = cache;
            CacheContext = cacheContext;
            ContextStack = contextStack;
        }

        public override object Execute(List<string> args)
        {
            var eldewritoGameVariants = $@"Resources\GenHaloOnline\HaloOnlineED07\game_variants";
            ParseFiles(eldewritoGameVariants, CacheVersion.HaloOnlineED, CachePlatform.Original);

            var eldewritoMapVariants = $@"Resources\GenHaloOnline\HaloOnlineED07\map_variants";
            ParseFiles(eldewritoMapVariants, CacheVersion.HaloOnlineED, CachePlatform.Original);

            var halo3GameVariants = $@"Resources\Gen3\Halo3MythicRetail\game_variants";
            ParseFiles(halo3GameVariants, CacheVersion.Halo3Retail, CachePlatform.Original);

            var halo3MapVariants = $@"Resources\Gen3\Halo3MythicRetail\map_variants";
            ParseFiles(halo3MapVariants, CacheVersion.Halo3Retail, CachePlatform.Original);

            var haloReachGameVariants = $@"Resources\Gen3\HaloReach\game_variants";
            ParseFiles(haloReachGameVariants, CacheVersion.HaloReach, CachePlatform.Original);

            var haloReachMapVariants = $@"Resources\Gen3\HaloReach\map_variants";
            ParseFiles(haloReachMapVariants, CacheVersion.HaloReach, CachePlatform.Original);

            var halo3MCCGameVariants = $@"Resources\GenMCC\Halo3MCC\game_variants";
            ParseFiles(halo3MCCGameVariants, CacheVersion.Halo3Retail, CachePlatform.MCC);

            var halo3MCCMapVariants = $@"Resources\GenMCC\Halo3MCC\map_variants";
            ParseFiles(halo3MCCMapVariants, CacheVersion.Halo3Retail, CachePlatform.MCC);

            var haloReachMCCGameVariants = $@"Resources\GenMCC\HaloReachMCC\game_variants";
            ParseFiles(haloReachMCCGameVariants, CacheVersion.HaloReach, CachePlatform.MCC);

            var haloReachMCCMapVariants = $@"Resources\GenMCC\HaloReachMCC\map_variants";
            ParseFiles(haloReachMCCMapVariants, CacheVersion.HaloReach, CachePlatform.MCC);

            return true;
        }

        public static void ParseFiles(string path, CacheVersion version, CachePlatform platform) 
        {
            var files = Directory.EnumerateFiles(path);

            foreach (var filePath in files) 
            {
                Console.WriteLine($"Parsing file \"{filePath}\"...");

                var input = new FileInfo(filePath);

                try
                {
                    using (var stream = input.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var reader = new EndianReader(stream);

                        var blf = new Blf(version, platform);
                        blf.Read(reader);

                        var fileName = Path.GetFileNameWithoutExtension(input.Name);

                        var fileExtension = input.Extension.TrimStart('.');

                        var handler = new BlfObjectHandler(version, platform);

                        var blfObject = new BlfObject()
                        {
                            FileName = fileName,
                            FileType = fileExtension,
                            Blf = blf,
                        };

                        var jsonData = handler.Serialize(blfObject);

                        var fileInfo = new FileInfo(Path.Combine(input.DirectoryName, $"{fileName}.json"));

                        if (!fileInfo.Directory.Exists)
                        {
                            fileInfo.Directory.Create();
                        }

                        File.WriteAllText(fileInfo.FullName, jsonData);
                    }
                }
                catch (Exception e) 
                {
                    new PrecursorError($"Failed to parse file \"{filePath}\": {e.Message}");
                }
            }
        }
    }
}
