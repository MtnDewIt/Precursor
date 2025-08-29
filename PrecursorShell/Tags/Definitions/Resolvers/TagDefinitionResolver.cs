using Newtonsoft.Json;
using PrecursorShell.Cache;
using PrecursorShell.Cache.BuildInfo;
using PrecursorShell.Reports;
using PrecursorShell.Serialization;
using PrecursorShell.Tags.Definitions.Reports;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TagTool.Cache;
using TagTool.Common;

namespace PrecursorShell.Tags.Definitions.Resolvers
{
    public class TagDefinitionResolver
    {
        private static readonly ParallelOptions Options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2
        };

        public static void ParseDefinitionsAsync(BuildTableEntry buildInfo) 
        {
            var files = buildInfo.CurrentCacheFiles;
            var build = buildInfo.Build;

            var buildReport = new TagDefinitionReport.TagDefinitionReportBuild(build);

            var processedFiles = new ConcurrentBag<string>();
            var fileErrorCount = 0;

            Parallel.ForEach(files, Options, file =>
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

            GameCache cache;

            try
            {
                cache = GameCache.Open(fileInfo);
            }
            catch (Exception)
            {
                return new FileProcessResult 
                { 
                    HasErrors = true 
                };
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
            var tagGroups = GetTagGroups(cache, build);

            // TODO: Resolve Gen 1, Gen 2, Monolithic and Halo Online tag groups.
            if (tagGroups == null)
                return true;

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

            var tagGroupCount = tagGroups.Count;
            var tagGroupErrorCount = 0;
            var groupPaths = new List<string>();

            Parallel.ForEach(tagGroups, Options, group => 
            {
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

        private static TagGroupProcessResult ProcessTagGroupAsync(GameCache cache, Stream stream, KeyValuePair<Tag, string> group, CacheBuild build, string fileName) 
        {
            var filteredGroup = Regex.Replace($"{group.Key}", @"[<>*\\ /:]", "_");
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
            groupWriter.WriteValue($"{group.Key}");

            groupWriter.WritePropertyName("GroupName");
            groupWriter.WriteValue($"{group.Value}");

            groupWriter.WritePropertyName("Tags");
            groupWriter.WriteStartArray();

            var tags = cache.TagCache.FindAllInGroup(group.Key);

            foreach (var tag in tags)
            {
                var deserializer = new Deserializer(cache.Version, cache.Platform);

                var errorCount = 0;

                groupWriter.WriteStartObject();
                groupWriter.WritePropertyName("TagName");
                groupWriter.WriteValue(tag.Name);

                groupWriter.WritePropertyName("Errors");
                groupWriter.WriteStartArray();

                if (cache.TagCache.TagDefinitions == null || !cache.TagCache.TagDefinitions.TagDefinitionExists(group.Key))
                {
                    groupWriter.WriteValue($"Tag definition for tag group {group.Key} not implemented");
                    groupWriter.WriteEndArray();
                    groupWriter.WriteEndObject();
                    tagErrorCount++;
                    continue;
                }

                try
                {
                    deserializer.DeserializeTagInstance(cache, stream, tag);
                }
                catch (Exception ex)
                {
                    groupWriter.WriteValue($"Failed to validate tag {tag}: {ex.Message}");
                    groupWriter.WriteEndArray();
                    groupWriter.WriteEndObject();
                    tagErrorCount++;
                    continue;
                }

                if (deserializer.Problems.Count > 0)
                {
                    foreach (var problem in deserializer.Problems)
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
            groupWriter.WriteValue(ParseErrorLevel(tagErrorCount, tags.Count()).ToString());

            groupWriter.WritePropertyName("TagErrorCount");
            groupWriter.WriteValue(tagErrorCount);

            groupWriter.WriteEndObject();

            return new TagGroupProcessResult
            {
                GroupPath = groupPath,
                HasErrors = tagErrorCount > 0
            };
        }

        public static Dictionary<Tag, string> GetTagGroups(GameCache cache, CacheBuild build) 
        {
            return build switch
            {
                CacheBuild.HaloXbox or
                CacheBuild.HaloPC or
                CacheBuild.HaloCustomEdition or
                CacheBuild.Halo1MCC => null,

                CacheBuild.Halo2Alpha or
                CacheBuild.Halo2Beta or
                CacheBuild.Halo2Xbox or
                CacheBuild.Halo2Vista or
                CacheBuild.Halo2MCC => null,

                CacheBuild.Halo3Beta or
                CacheBuild.Halo3Retail or
                CacheBuild.Halo3MythicRetail or
                CacheBuild.Halo3ODST or
                CacheBuild.HaloReach or
                CacheBuild.Halo3MCC or
                CacheBuild.Halo3ODSTMCC or
                CacheBuild.HaloReachMCC => (cache as GameCacheGen3).TagCacheGen3.Groups.ToDictionary(g => g.Tag, g => g.Name),

                CacheBuild.HaloReach11883 => null,

                CacheBuild.HaloOnlineED or
                CacheBuild.HaloOnline106708 or
                CacheBuild.HaloOnline155080 or
                CacheBuild.HaloOnline171227 or
                CacheBuild.HaloOnline177150 or
                CacheBuild.HaloOnline235640 or
                CacheBuild.HaloOnline301003 or
                CacheBuild.HaloOnline327043 or
                CacheBuild.HaloOnline332089 or
                CacheBuild.HaloOnline372731 or
                CacheBuild.HaloOnline373869 or
                CacheBuild.HaloOnline416097 or
                CacheBuild.HaloOnline416138 or
                CacheBuild.HaloOnline430475 or
                CacheBuild.HaloOnline430653 or
                CacheBuild.HaloOnline449175 or
                CacheBuild.HaloOnline454665 or
                CacheBuild.HaloOnline479394 or
                CacheBuild.HaloOnline498295 or
                CacheBuild.HaloOnline530945 or
                CacheBuild.HaloOnline533032 or
                CacheBuild.HaloOnline554482 or
                CacheBuild.HaloOnline571698 or
                CacheBuild.HaloOnline604673 or
                CacheBuild.HaloOnline700255 => null,

                CacheBuild.Halo4Retail or
                CacheBuild.Halo4MCC or
                CacheBuild.Halo2AMPMCC => (cache as GameCacheGen4).TagCacheGen4.Groups.ToDictionary(g => g.Tag, g => g.Name),

                _ => null,
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
