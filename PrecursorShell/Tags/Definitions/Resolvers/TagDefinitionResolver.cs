using Newtonsoft.Json;
using PrecursorShell.Cache;
using PrecursorShell.Cache.BuildInfo;
using PrecursorShell.Cache.BuildInfo.Eldorado.Groups;
using PrecursorShell.Cache.BuildInfo.Gen1.Groups;
using PrecursorShell.Cache.BuildInfo.Gen2.Groups;
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
using TagTool.Cache.Monolithic;
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

            if (buildInfo.Compressed)
            {
                foreach (var file in files) 
                {
                    var result = ProcessFileAsync(file, build);

                    if (result.HasErrors)
                    {
                        fileErrorCount++;
                    }

                    if (result.FilePath != null)
                    {
                        processedFiles.Add(result.FilePath);
                    }
                }
            }
            else 
            {
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
            }

            buildReport.ErrorLevel = ReportHelper.ParseErrorLevel(fileErrorCount, buildReport.Files.Count);
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
            fileWriter.WriteValue(ReportHelper.ParseErrorLevel(tagGroupErrorCount, tagGroupCount).ToString());

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

            var type = cache.TagCache.TagDefinitions.GetTagDefinitionType(group.Key);
            var isValidDefinition = cache.TagCache.TagDefinitions == null || !cache.TagCache.TagDefinitions.TagDefinitionExists(group.Key);

            foreach (var tag in tags)
            {
                var deserializer = new Deserializer(cache.Version, cache.Platform);

                var errorCount = 0;

                groupWriter.WriteStartObject();
                groupWriter.WritePropertyName("TagName");
                groupWriter.WriteValue(tag.Name ?? $"0x{tag.Index:X4}");

                groupWriter.WritePropertyName("Errors");
                groupWriter.WriteStartArray();

                if (isValidDefinition)
                {
                    groupWriter.WriteValue($"Tag definition for tag group {group.Key} not implemented");
                    groupWriter.WriteEndArray();
                    groupWriter.WriteEndObject();
                    tagErrorCount++;
                    continue;
                }

                try
                {
                    deserializer.DeserializeTagInstance(cache, stream, type, tag);
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
            groupWriter.WriteValue(ReportHelper.ParseErrorLevel(tagErrorCount, tags.Count()).ToString());

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
                CacheBuild.Halo1MCC => Gen1Groups.Groups,

                CacheBuild.Halo2Alpha or
                CacheBuild.Halo2Beta or
                CacheBuild.Halo2Xbox or
                CacheBuild.Halo2Vista or
                CacheBuild.Halo2MCC => Gen2Groups.Groups,

                CacheBuild.Halo3Beta or
                CacheBuild.Halo3Retail or
                CacheBuild.Halo3MythicRetail or
                CacheBuild.Halo3ODST or
                CacheBuild.HaloReach or
                CacheBuild.Halo3MCC or
                CacheBuild.Halo3ODSTMCC or
                CacheBuild.HaloReachMCC => (cache as GameCacheGen3).TagCacheGen3.Groups.ToDictionary(g => g.Tag, g => g.Name),

                CacheBuild.HaloReach11883 => GetMonolithicGroups(cache),

                CacheBuild.EldoradoED or
                CacheBuild.Eldorado106708 or
                CacheBuild.Eldorado155080 or
                CacheBuild.Eldorado171227 or
                CacheBuild.Eldorado177150 or
                CacheBuild.Eldorado235640 or
                CacheBuild.Eldorado301003 or
                CacheBuild.Eldorado332089 or
                CacheBuild.Eldorado373869 or
                CacheBuild.Eldorado416138 or
                CacheBuild.Eldorado430653 or
                CacheBuild.Eldorado454665 or
                CacheBuild.Eldorado479394 or
                CacheBuild.Eldorado498295 or
                CacheBuild.Eldorado530945 or
                CacheBuild.Eldorado533032 or
                CacheBuild.Eldorado554482 or
                CacheBuild.Eldorado571698 or
                CacheBuild.Eldorado604673 or
                CacheBuild.Eldorado700255 => EldoradoGroups.Groups,

                CacheBuild.Halo4Retail or
                CacheBuild.Halo4MCC or
                CacheBuild.Halo2AMPMCC => (cache as GameCacheGen4).TagCacheGen4.Groups.ToDictionary(g => g.Tag, g => g.Name),

                _ => null,
            };
        }

        public static Dictionary<Tag, string> GetMonolithicGroups(GameCache cache) 
        {
            Dictionary<Tag, string> groups = new Dictionary<Tag, string>();

            foreach (var group in (cache as GameCacheMonolithic).TagCacheMono.Tags.GroupBy(x => x.Group)) 
            {
                string name = (cache as GameCacheMonolithic).TagCacheMono.Tags.FirstOrDefault(x => x.Group == group.Key).ToString();

                if (!group.Key.Tag.Equals("????") && !name.Equals("????")) 
                {
                    groups.Add(group.Key.Tag, name.Split('.')[1]);
                }
            }

            return groups;
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
