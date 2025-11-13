using Newtonsoft.Json;
using PrecursorShell.Cache.BuildInfo;
using PrecursorShell.Cache.Reports;
using PrecursorShell.Reports;
using PrecursorShell.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TagTool.Cache;
using PrecursorShell.JSON.Handlers;
using PrecursorShell.JSON.Objects;

namespace PrecursorShell.Cache.Resolvers
{
    public class CacheDefinitionResolver
    {
        private static readonly ParallelOptions Options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2
        };

        public static void ParseDefinitionsAsync(BuildTableEntry buildInfo) 
        {
            if (buildInfo.Build == CacheBuild.HaloReach11883) 
                return;

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
            var outputFileInfo = new FileInfo($"{DirectoryPaths.Base}\\Reports\\CacheDefinitions\\{filePath}");

            if (!outputFileInfo.Directory.Exists)
            {
                outputFileInfo.Directory.Create();
            }

            var hasFileErrors = ProcessCacheFileAsync(buildInfo, fileInfo, outputFileInfo, fileName);

            return new FileProcessResult
            {
                FilePath = filePath,
                HasErrors = hasFileErrors
            };
        }

        private static bool ProcessCacheFileAsync(BuildTableEntry buildInfo, FileInfo fileInfo, FileInfo outputFileInfo, string fileName)
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
            var headerType = CacheFileHeader.GetHeaderType(buildInfo.Version, buildInfo.Platform);

            using (var stream = fileInfo.OpenRead()) 
            {
                try 
                {
                    ProcessCacheFile(buildInfo, deserializer, stream, headerType, fileName);
                }
                catch (Exception ex) 
                {
                    fileWriter.WriteValue($"Failed to deserialize header \"{fileName}\": {ex.Message}");
                    fileWriter.WriteEndArray();
                    fileWriter.WriteEndObject();
                    return true;
                }
            }

            if (deserializer.Problems.Count > 0)
            {
                foreach (var problem in deserializer.Problems)
                {
                    fileWriter.WriteValue(problem);
                }
            }

            fileWriter.WriteEndArray();
            fileWriter.WriteEndObject();

            return deserializer.Problems.Count > 0; 
        }

        private static void ProcessCacheFile(BuildTableEntry buildInfo, Deserializer deserializer, Stream stream, Type headerType, string fileName) 
        {
            var header = deserializer.DeserializeStructure(stream, headerType);
            var blf = deserializer.DeserializeBlf(stream);
            var reports = deserializer.DeserializeCacheFileReports(stream, header as CacheFileHeader);

            var mapObject = new MapObject()
            {
                MapName = fileName,
                Version = buildInfo.Version,
                Platform = buildInfo.Platform,
                Header = header as CacheFileHeader,
                MapFileBlf = blf,
                Reports = reports,
            };

            var handler = new MapObjectHandler(buildInfo.Version, buildInfo.Platform);

            var jsonData = handler.Serialize(mapObject);

            var fileInfo = new FileInfo($"{DirectoryPaths.Base}\\Reports\\CacheDefinitions\\{buildInfo.Build}\\{fileName}\\{fileName}_header.json");

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            File.WriteAllText(fileInfo.FullName, jsonData);
        }

        private static HashSet<string> GetBuildFiles(BuildTableEntry buildInfo)
        {
            return buildInfo.Build switch
            {
                CacheBuild.Halo2Alpha or
                CacheBuild.Halo2Beta or
                CacheBuild.Halo2Xbox or
                CacheBuild.Halo2Vista or
                CacheBuild.Halo3Beta or
                CacheBuild.Halo3Retail or
                CacheBuild.Halo3MythicRetail or
                CacheBuild.Halo3ODST or
                CacheBuild.HaloReach or
                CacheBuild.Halo4Retail or
                CacheBuild.Halo2MCC or
                CacheBuild.Halo3MCC or
                CacheBuild.Halo3ODSTMCC or
                CacheBuild.HaloReachMCC or
                CacheBuild.Halo4MCC or
                CacheBuild.Halo2AMPMCC => buildInfo.CurrentCacheFiles.Union(buildInfo.CurrentSharedFiles).ToHashSet(),

                CacheBuild.HaloOnlineED or
                CacheBuild.HaloOnline106708 or
                CacheBuild.HaloOnline155080 or
                CacheBuild.HaloOnline171227 or
                CacheBuild.HaloOnline177150 or
                CacheBuild.HaloOnline235640 or
                CacheBuild.HaloOnline301003 or
                CacheBuild.HaloOnline332089 or
                CacheBuild.HaloOnline373869 or
                CacheBuild.HaloOnline416138 or
                CacheBuild.HaloOnline430653 or
                CacheBuild.HaloOnline454665 or
                CacheBuild.HaloOnline479394 or
                CacheBuild.HaloOnline498295 or
                CacheBuild.HaloOnline530945 or
                CacheBuild.HaloOnline533032 or
                CacheBuild.HaloOnline554482 or
                CacheBuild.HaloOnline571698 or
                CacheBuild.HaloOnline604673 or
                CacheBuild.HaloOnline700255 => buildInfo.CurrentMapFiles,

                _ => buildInfo.CurrentCacheFiles,
            };
        }

        private class FileProcessResult
        {
            public string FilePath { get; set; }
            public bool HasErrors { get; set; }
        }
    }
}
