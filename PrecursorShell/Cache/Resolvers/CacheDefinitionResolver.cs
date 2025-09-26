using Newtonsoft.Json;
using PrecursorShell.Cache.BuildInfo;
using PrecursorShell.Cache.Reports;
using PrecursorShell.Reports;
using PrecursorShell.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PrecursorShell.Cache.Resolvers
{
    public class CacheDefinitionResolver
    {
        private static readonly ParallelOptions Options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2
        };

        private static HashSet<string> GetBuildFiles(BuildTableEntry buildInfo) 
        {
            if (buildInfo.Generation == CacheGeneration.Eldorado)
            {
                return buildInfo.CurrentMapFiles;
            }

            // TODO: Merge shared maps for other cache generations
            return buildInfo.CurrentCacheFiles;
        }

        public static void ParseDefinitionsAsync(BuildTableEntry buildInfo) 
        {
            var files = GetBuildFiles(buildInfo);
            var build = buildInfo.Build;

            var cacheReport = new CacheDefinitionReport.CacheDefinitionReportBuild(build);

            var processedFiles = new ConcurrentBag<string>();
            var fileErrorCount = 0;

            Parallel.ForEach(files, Options, file =>
            {
                var result = ProcessFileAsync(buildInfo, file);

                if (result.HasErrors)
                {
                    Interlocked.Increment(ref fileErrorCount);
                }

                if (result.FilePath != null)
                {
                    processedFiles.Add(result.FilePath);
                }
            });

            cacheReport.ErrorLevel = ReportHelper.ParseErrorLevel(fileErrorCount, cacheReport.Files.Count);
            cacheReport.FileErrorCount = fileErrorCount;
            cacheReport.Files.AddRange(processedFiles);

            Program.CacheDefinitionReport.AddEntry(cacheReport);
        }

        private static FileProcessResult ProcessFileAsync(BuildTableEntry buildInfo, string file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var filePath = $"{buildInfo.Build}\\{fileName}\\{fileName}.json";
            var fileInfo = new FileInfo(file);
            var outputFileInfo = new FileInfo($"{Program.PrecursorDirectory}\\Reports\\CacheDefinitions\\{filePath}");

            if (!outputFileInfo.Directory.Exists)
            {
                outputFileInfo.Directory.Create();
            }

            var hasFileErrors = ProcessCacheFileAsync(buildInfo, outputFileInfo, fileName);

            return new FileProcessResult
            {
                FilePath = filePath,
                HasErrors = hasFileErrors
            };
        }

        private static bool ProcessCacheFileAsync(BuildTableEntry buildInfo, FileInfo outputFileInfo, string fileName)
        {
            using var fileStream = new StreamWriter(outputFileInfo.FullName);
            using var fileWriter = new JsonTextWriter(fileStream)
            {
                Formatting = Formatting.Indented,
            };

            fileWriter.WriteStartObject();
            fileWriter.WritePropertyName("FileName");
            fileWriter.WriteValue(fileName);

            fileWriter.WritePropertyName("Errors");
            fileWriter.WriteStartArray();

            var deserializer = new Deserializer(buildInfo.Version, buildInfo.Platform);



            if (deserializer.Problems.Count > 0)
            {
                foreach (var problem in deserializer.Problems)
                {
                    fileWriter.WriteValue(problem);
                }
            }

            fileWriter.WriteEndArray();

            fileWriter.WriteEndObject();

            return false;
        }

        private class FileProcessResult
        {
            public string FilePath { get; set; }
            public bool HasErrors { get; set; }
        }

        /*
        try
        {
            GenerateJSON(mapFile, fileName, ResourcePath);
        }
        catch (Exception ex)
        {
            return new FileValidationResult(false, $"Failed to serialize JSON \"{fileName}\": {ex.Message}");
        }

        public void GenerateJSON(MapFile mapFile, string fileName, string tempPath) 
        {
            var path = ResourcePath.Replace("Resources", "Temp");
            var mapName = Path.GetFileNameWithoutExtension(fileName);

            var mapObject = new MapObject()
            {
                MapName = mapName,
                Version = mapFile.Version,
                Platform = mapFile.Platform,
                Header = mapFile.Header,
                MapFileBlf = mapFile.MapFileBlf,
                Reports = mapFile.Reports,
            };

            var handler = new MapObjectHandler(Version, Platform);

            var jsonData = handler.Serialize(mapObject);

            var fileInfo = new FileInfo(Path.Combine($"{path}", "cache_files", $"{fileName}.json"));

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            File.WriteAllText(fileInfo.FullName, jsonData);
        }
        */
    }
}
