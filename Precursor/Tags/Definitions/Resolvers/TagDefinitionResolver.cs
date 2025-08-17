using Newtonsoft.Json;
using Precursor.Cache;
using Precursor.Cache.BuildInfo;
using Precursor.Common;
using Precursor.Reports;
using Precursor.Tags.Definitions.Reports;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TagTool.Cache;
using TagTool.Cache.HaloOnline;
using TagTool.Cache.Monolithic;
using TagTool.Tags;

namespace Precursor.Tags.Definitions.Resolvers
{
    public class TagDefinitionResolver
    {
        public static void ParseDefinitionsAsync(BuildInfoEntry buildInfo) 
        {
            var files = buildInfo.GetCurrentCacheFiles();
            var build = buildInfo.GetBuild();

            var buildReport = new TagDefinitionReport.TagDefinitionReportBuild(build);

            var processedFiles = new ConcurrentBag<string>();
            var fileErrorCount = 0;

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(files, parallelOptions, file =>
            {
                var result = ProcessFileAsync(file, build);

                if (result.HasErrors)
                {
                    Interlocked.Increment(ref fileErrorCount);
                }

                if (result.FilePath != null)
                {
                    processedFiles.Add(result.FilePath);
                }
            });

            buildReport.ErrorLevel = ParseErrorLevel(fileErrorCount, buildReport.Files.Count);
            buildReport.FileErrorCount = fileErrorCount;
            buildReport.Files.AddRange(processedFiles);

            Program.TagDefinitionReport.AddEntry(buildReport);
        }

        private static FileProcessResult ProcessFileAsync(string file, CacheBuild build) 
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var filePath = $"{build}\\{fileName}\\{fileName}.json";
            var fileInfo = new FileInfo(file);
            var outputFileInfo = new FileInfo($"{Program.PrecursorDirectory}\\Reports\\TagDefinitions\\{filePath}");

            if (!outputFileInfo.Directory.Exists)
            {
                outputFileInfo.Directory.Create();
            }

            GameCache cache = null;

            try
            {
                cache = GameCache.Open(file);

                switch (build)
                {
                    case CacheBuild.HaloXbox:
                    case CacheBuild.HaloPC:
                    case CacheBuild.HaloCustomEdition:
                    case CacheBuild.Halo1MCC:
                        var cacheGen1 = cache as GameCacheGen1;
                        break;
                    case CacheBuild.Halo2Alpha:
                    case CacheBuild.Halo2Beta:
                    case CacheBuild.Halo2Xbox:
                    case CacheBuild.Halo2Vista:
                    case CacheBuild.Halo2MCC:
                        var cacheGen2 = cache as GameCacheGen2;
                        break;
                    case CacheBuild.Halo3Beta:
                    case CacheBuild.Halo3Retail:
                    case CacheBuild.Halo3MythicRetail:
                    case CacheBuild.Halo3ODST:
                    case CacheBuild.HaloReach:
                    case CacheBuild.Halo3MCC:
                    case CacheBuild.Halo3ODSTMCC:
                    case CacheBuild.HaloReachMCC:
                        var cacheGen3 = cache as GameCacheGen3;
                        break;
                    case CacheBuild.HaloReach11883:
                        var cacheGenMonolithic = cache as GameCacheMonolithic;
                        break;
                    case CacheBuild.HaloOnlineED:
                    case CacheBuild.HaloOnline106708:
                    case CacheBuild.HaloOnline235640:
                    case CacheBuild.HaloOnline301003:
                    case CacheBuild.HaloOnline327043:
                    case CacheBuild.HaloOnline372731:
                    case CacheBuild.HaloOnline416097:
                    case CacheBuild.HaloOnline430475:
                    case CacheBuild.HaloOnline454665:
                    case CacheBuild.HaloOnline449175:
                    case CacheBuild.HaloOnline498295:
                    case CacheBuild.HaloOnline530605:
                    case CacheBuild.HaloOnline532911:
                    case CacheBuild.HaloOnline554482:
                    case CacheBuild.HaloOnline571627:
                    case CacheBuild.HaloOnline604673:
                    case CacheBuild.HaloOnline700123:
                        var cacheGenHaloOnline = cache as GameCacheHaloOnline;
                        break;
                    case CacheBuild.Halo4Retail:
                    case CacheBuild.Halo4MCC:
                    case CacheBuild.Halo2AMPMCC:
                        var cacheGen4 = cache as GameCacheGen4;
                        break;
                }
            }
            catch (Exception ex)
            {
                new PrecursorWarning($"Failed to open cache file \"{fileInfo.Name}\": {ex.Message}");
                return new FileProcessResult { HasErrors = true };
            }

            var hasGroupErrors = ProcessCacheFileAsync(cache, file, outputFileInfo, build, fileName);

            return new FileProcessResult
            {
                FilePath = filePath,
                HasErrors = hasGroupErrors
            };
        }

        private static bool ProcessCacheFileAsync(GameCache cache, string file, FileInfo outputFileInfo, CacheBuild build, string fileName) 
        {
            using var fileStream = new StreamWriter(outputFileInfo.FullName);
            using var fileWriter = new JsonTextWriter(fileStream)
            {
                Formatting = Formatting.Indented,
            };

            fileWriter.WriteStartObject();
            fileWriter.WritePropertyName("FileName");
            fileWriter.WriteValue(Path.GetFileName(file));

            fileWriter.WritePropertyName("Groups");
            fileWriter.WriteStartArray();

            var tagGroups = cache.TagCache.NonNull().GroupBy(x => x.Group);
            var tagGroupCount = tagGroups.Count();
            var tagGroupErrorCount = 0;
            var groupPaths = new List<string>();

            // TODO: Update this to that it loops through each tag group in the target cache.
            // This is only possible with Gen3 and Gen4 caches.
            // TODO 2: Pull valid tag group tables for Gen1 and Gen2 caches.
            // TODO 3: Somehow get HO tag groups. No clue if this differs between builds.

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(tagGroups, parallelOptions, group => 
            {
                if (group.Key == null || group.Key.Tag == "????")
                    return; // TODO: Add better handling for this

                if (cache.Version == CacheVersion.Halo4 && cache.Platform == CachePlatform.MCC && group.Key.Tag == "weap")
                    return; // Skip until fixed

                using (var stream = cache.OpenCacheRead())
                {
                    var result = ProcessTagGroupAsync(cache, stream, group, build, fileName);

                    if (result != null)
                    {
                        groupPaths.Add(result.GroupPath);

                        if (result.HasErrors)
                        {
                            Interlocked.Increment(ref tagGroupErrorCount);
                        }
                    }
                }
            });

            foreach (var groupPath in groupPaths.OrderBy(p => p))
            {
                fileWriter.WriteValue(groupPath);
            }

            fileWriter.WriteEndArray();

            fileWriter.WritePropertyName("ErrorLevel");
            fileWriter.WriteValue(ParseErrorLevel(tagGroupErrorCount, tagGroupCount).ToString());

            fileWriter.WritePropertyName("GroupErrorCount");
            fileWriter.WriteValue(tagGroupErrorCount);

            fileWriter.WriteEndObject();

            return tagGroupErrorCount > 0;
        }

        private static TagGroupProcessResult ProcessTagGroupAsync(GameCache cache, Stream stream, IGrouping<TagGroup, CachedTag> group, CacheBuild build, string fileName) 
        {
            var tagGroup = $"{group.Key.Tag}";
            var filteredGroup = Regex.Replace(tagGroup, @"[<>*\\ /:]", "_");
            var tagCount = group.Count();
            var tagErrorCount = 0;
            var groupPath = $"{build}\\{fileName}\\{filteredGroup}\\{filteredGroup}.json";
            var groupOutputInfo = new FileInfo($"{Program.PrecursorDirectory}\\Reports\\TagDefinitions\\{groupPath}");

            if (!groupOutputInfo.Directory.Exists)
            {
                groupOutputInfo.Directory.Create();
            }

            using var groupStream = new StreamWriter(groupOutputInfo.FullName);
            using var groupWriter = new JsonTextWriter(groupStream)
            {
                Formatting = Formatting.Indented,
            };

            groupWriter.WriteStartObject();
            groupWriter.WritePropertyName("TagGroup");
            groupWriter.WriteValue(tagGroup);

            groupWriter.WritePropertyName("GroupName");
            groupWriter.WriteValue("");

            groupWriter.WritePropertyName("Tags");
            groupWriter.WriteStartArray();

            foreach (var tag in group.ToList())
            {
                var validator = new TagDefinitionValidator(cache, stream);

                var errorCount = 0;

                groupWriter.WriteStartObject();
                groupWriter.WritePropertyName("TagName");
                groupWriter.WriteValue(tag.Name);

                groupWriter.WritePropertyName("Errors");
                groupWriter.WriteStartArray();

                if (cache.TagCache.TagDefinitions == null || !cache.TagCache.TagDefinitions.TagDefinitionExists(group.Key))
                {
                    groupWriter.WriteValue($"Tag definition for tag group {group.Key.Tag} not implemented");
                    groupWriter.WriteEndArray();
                    groupWriter.WriteEndObject();
                    tagErrorCount++;
                    continue;
                }

                try
                {
                    validator.VerifyTag(tag);
                }
                catch (Exception ex)
                {
                    groupWriter.WriteValue($"Failed to validate tag {tag}: {ex.Message}");
                    groupWriter.WriteEndArray();
                    groupWriter.WriteEndObject();
                    tagErrorCount++;
                    continue;
                }

                if (validator.Problems.Count > 0)
                {
                    foreach (var problem in validator.Problems)
                    {
                        groupWriter.WriteValue(problem);
                        errorCount++;
                    }
                }

                groupWriter.WriteEndArray();

                groupWriter.WriteEndObject();

                if (errorCount > 0)
                {
                    tagErrorCount++;
                }
            }

            groupWriter.WriteEndArray();

            groupWriter.WritePropertyName("ErrorLevel");
            groupWriter.WriteValue(ParseErrorLevel(tagErrorCount, tagCount).ToString());

            groupWriter.WritePropertyName("TagErrorCount");
            groupWriter.WriteValue(tagErrorCount);

            groupWriter.WriteEndObject();

            return new TagGroupProcessResult
            {
                GroupPath = groupPath,
                HasErrors = tagErrorCount > 0
            };
        }

        public static ReportErrorLevel ParseErrorLevel(int currentCount, int totalCount)
        {
            if (currentCount == 0)
                return ReportErrorLevel.None;

            if (currentCount > 0 && currentCount < totalCount)
                return ReportErrorLevel.Intermediate;

            if (currentCount == totalCount)
                return ReportErrorLevel.All;

            return ReportErrorLevel.All;
        }

        private class FileProcessResult
        {
            public string FilePath { get; set; }
            public bool HasErrors { get; set; }
        }

        private class TagGroupProcessResult
        {
            public string GroupPath { get; set; }
            public bool HasErrors { get; set; }
        }
    }
}
