using PrecursorShell.JSON.Handlers;
using PrecursorShell.JSON.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.Commands.Common;

namespace PrecursorShell.Commands.JSON
{
    public class GenerateMapObjectCommand : PrecursorCommand
    {
        private GameCache Cache;
        private string ExportPath = $@"maps";
        private string PathPrefix = null;

        private int MapCount = 0;
        private Stopwatch StopWatch = new Stopwatch();
        private List<string> ErrorLog = new List<string>();

        public GenerateMapObjectCommand(GameCache cache) : base
        (
            false,
            "GenerateMapObject",
            "Generates a JSON map object file based on the specified map file",

            "GenerateMapObject <Map_Path> [PathPrefix]",
            "Generates a JSON map object file based on the specified map file\n\n" + 

            "Optionally, instead of specifying a map file to convert you can\n" + 
            "use \"all\", which will convert all map files associated with\n" +
            "the current cache context"
        )
        {
            Cache = cache;
        }

        public override object Execute(List<string> args)
        {
            MapCount = 0;
            StopWatch.Reset();
            ErrorLog.Clear();

            if (args.Count > 2)
                return new TagToolError(CommandError.ArgCount);

            PathPrefix = args.Count == 2 ? args[1] : null;
            
            ExportPath = PathPrefix != null ? Path.Combine(PathPrefix, ExportPath) : ExportPath;

            ProcessDirectoryAsync(args[0]).GetAwaiter().GetResult();

            Console.WriteLine($"{MapCount - ErrorLog.Count}/{MapCount} Variants Converted Successfully in {StopWatch.ElapsedMilliseconds.FormatMilliseconds()} with {ErrorLog.Count} {(ErrorLog.Count == 1 ? "error" : "errors")}\n");

            if (ErrorLog.Count > 0)
            {
                ParseErrorLog();
            }

            PathPrefix = null;

            return true;
        }

        public async Task ProcessDirectoryAsync(string inputPath)
        {
            var files = new List<string>();
            var mapFiles = new List<MapFile>();

            if (Cache is GameCacheHaloOnlineBase hoCache) 
            {
                if (inputPath.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    mapFiles = hoCache.MapFiles.GetAll().ToList();
                }
                else
                {
                    var modFile = hoCache.MapFiles.FindByName(inputPath);
                    mapFiles.Add(modFile);
                }
            }

            MapCount = mapFiles.Count;

            StopWatch.Start();

            var tasks = mapFiles.Select(ConvertMapAsync);
            await Task.WhenAll(tasks);

            StopWatch.Start();
        }

        private async Task ConvertMapAsync(MapFile mapData) 
        {
            try
            {
                var mapObject = new MapObject()
                {
                    MapName = mapData.Header.GetName(),
                    Version = mapData.Version,
                    Platform = mapData.Platform,
                    Header = mapData.Header,
                    MapFileBlf = mapData.MapFileBlf,
                    Reports = mapData.Reports,
                };

                var handler = new MapObjectHandler(Cache.Version, Cache.Platform);

                var jsonData = handler.Serialize(mapObject);

                var fileInfo = new FileInfo(Path.Combine(ExportPath, $"{mapData.Header.GetName()}.json"));

                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }

                File.WriteAllText(fileInfo.FullName, jsonData);
            }
            catch (Exception e)
            {
                ErrorLog.Add($"Error converting \"{mapData.Header.GetName()}.map\" : {e.Message}");
            }
        }

        public void ParseErrorLog()
        {
            var time = DateTime.Now;
            var shortDateTime = $@"{time.ToShortDateString()}-{time.ToShortTimeString()}";

            var fileName = Regex.Replace($"hott_{shortDateTime}_map_errors.log", @"[<>:""/\|?*]", "_");
            var filePath = "logs";
            var fullPath = Path.Combine(DirectoryPaths.Base, filePath, fileName);

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            using (StreamWriter writer = new StreamWriter(File.Create(fullPath)))
            {
                foreach (var error in ErrorLog)
                {
                    writer.WriteLine(error);
                }
            }

            Console.WriteLine($"Check \"{fullPath}\" for details on errors");
        }
    }
}