using Precursor.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TagTool.Cache;
using TagTool.Cache.HaloOnline;
using TagTool.Commands.Common;

namespace Precursor.Commands.GenerateCache
{
    partial class GenerateCacheCommand : PrecursorCommand
    {
        public GameCache Cache { get; set; }
        public GameCacheHaloOnline CacheContext { get; set; }
        public Stream CacheStream { get; set; }
        public static DirectoryInfo SourceDirectoryInfo { get; set; }
        public static DirectoryInfo OutputDirectoryInfo { get; set; }
        public Stopwatch StopWatch { get; set; }

        public GenerateCacheCommand(GameCache cache) : base
        (
            true,
            "GenerateCache",
            "Generates a new cache for use with ElDewrito 0.7.1",
            "GenerateCache <Source Path> <Output Path> <Scenario Path>",

            // TODO: Redo help text :/
            "It's similar to the MCC Tools, but don't try and use MCC loose tags" 
        )
        {
            Cache = cache;
            StopWatch = new Stopwatch();
        }

        public override object Execute(List<string> args) 
        {
            StopWatch.Reset();

            if (args.Count > 3)
                return new PrecursorError($"Incorrect amount of arguments supplied");

            SourceDirectoryInfo = new DirectoryInfo(args[0]);
            OutputDirectoryInfo = new DirectoryInfo(args[1]);

            if (!SourceDirectoryInfo.Exists)
                return new PrecursorError("Source data path does not exist, or could not be found");

            if (!OutputDirectoryInfo.Exists)
                return new PrecursorError("Output path does not exist, or could not be found");

            if (OutputDirectoryInfo.FullName == Cache.Directory.FullName)
                return new PrecursorError("Output path cannot be the same as the current working directory");

            StopWatch.Start();

            RebuildCache(OutputDirectoryInfo.FullName);
            RetargetCache(OutputDirectoryInfo.FullName);

            using (CacheStream = Cache.OpenCacheReadWrite()) 
            {
                ParseJSONData(SourceDirectoryInfo.FullName, args[2]);
            }

            StopWatch.Stop();

            var output = StopWatch.ElapsedMilliseconds.FormatMilliseconds();
            var startColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Cache Generated Sucessfully. Time Taken: " + output);
            Console.ForegroundColor = startColor;

            return true;
        }
    }
}