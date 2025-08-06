using Precursor.Cache;
using Precursor.Cache.BuildInfo;
using Precursor.Cache.BuildTable.Handlers;
using Precursor.Common;
using Precursor.Tags.Definitions.Reports;
using Precursor.Tags.Definitions.Reports.Handlers;
using SimpleJSON;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TagTool.Cache;
using TagTool.Cache.HaloOnline;
using TagTool.Cache.Monolithic;
using TagTool.Commands.Tags;

namespace Precursor.Tags.Definitions.Resolvers
{
    public class TagDefinitionResolver
    {
        public static void ParseDefinitions(BuildInfoEntry buildInfo)
        {
            var files = buildInfo.GetCurrentCacheFiles();

            var buildReport = new TagDefinitionReport.ReportBuild(buildInfo.GetBuild());

            foreach (var file in files) 
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileInfo = new FileInfo(file);

                GameCache cache = null;

                try
                {
                    cache = GameCache.Open(file);

                    switch (buildInfo.GetBuild()) 
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
                    continue;
                }

                var reportCacheFile = new TagDefinitionReportCacheFile(Path.GetFileName(file));

                using (var stream = cache.OpenCacheRead()) 
                {
                    // TODO: Update this to that it loops through each tag group in the target cache.
                    // This is only possible with Gen3 and Gen4 caches.
                    // TODO 2: Pull valid tag group tables for Gen1 and Gen2 caches.
                    // TODO 3: Somehow get HO tag groups. No clue if this differs between builds.

                    var tagGroups = cache.TagCache.NonNull().GroupBy(x => x.Group);

                    foreach (var group in tagGroups) 
                    {
                        if (group.Key == null || group.Key.Tag == "????")
                            continue; // TODO: Add better handling for this

                        if (cache.Version == CacheVersion.Halo4 && cache.Platform == CachePlatform.MCC && group.Key.Tag == "weap")
                            continue; // Skip until fixed

                        var tagGroup = $"{group.Key.Tag}";
                        var filteredGroup = Regex.Replace(tagGroup, @"[<>*\\ /:]", "_");

                        var reportTagGroup = new TagDefinitionReportTagGroup(tagGroup);

                        Debug.Write($"Parsing Tag Group {tagGroup}\n");

                        foreach (var tag in group.ToList()) 
                        {
                            var validator = new TagDefinitionValidator(cache, stream);

                            var reportTagInstance = new TagDefinitionReportTagInstance(tag.Name);

                            if (cache.TagCache.TagDefinitions == null || !cache.TagCache.TagDefinitions.TagDefinitionExists(group.Key))
                            {
                                reportTagInstance.Errors.Add($"Tag definition for tag group {group.Key.Tag} not implemented");
                                continue;
                            }

                            try 
                            {
                                validator.VerifyTag(tag);
                            }
                            catch (Exception ex) 
                            {
                                reportTagInstance.Errors.Add($"Failed to validate tag {tag}: {ex.Message}");
                                reportTagGroup.Tags.Add(reportTagInstance);
                                continue;
                            }

                            if (validator.Problems.Count > 0) 
                            {
                                reportTagInstance.Errors.AddRange(validator.Problems);
                            }

                            reportTagGroup.Tags.Add(reportTagInstance);
                        }

                        var groupPath = $"{buildInfo.GetBuild()}\\{fileName}\\{filteredGroup}\\{filteredGroup}.json";

                        reportCacheFile.Groups.Add(groupPath);
                        TagDefinitionReportTagGroup.GenerateReportTagGroup(reportTagGroup, groupPath);
                    }
                }

                var filePath = $"{buildInfo.GetBuild()}\\{fileName}\\{fileName}.json";

                buildReport.Files.Add(filePath);
                TagDefinitionReportCacheFile.GenerateReportCacheFiles(reportCacheFile, filePath);
            }

            Program.TagDefinitionReport.AddEntry(buildReport);
        }
    }
}
