using Precursor.Cache;
using Precursor.Cache.BuildInfo;
using Precursor.Common;
using System;
using System.IO;
using System.Linq;
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

            foreach (var file in files) 
            {
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

                using (var stream = cache.OpenCacheRead()) 
                {
                    // TODO: Update this to that it loops through each tag group in the target cache.
                    // This is only possible with Gen3 and Gen4 caches.
                    // TODO 2: Pull valid tag group tables for Gen1 and Gen2 caches.
                    // TODO 3: Somehow get HO tag groups. No clue if this differs between builds.

                    foreach (var group in cache.TagCache.TagTable.GroupBy(x => x.Group)) 
                    {
                        if (cache.TagCache.TagDefinitions == null || !cache.TagCache.TagDefinitions.TagDefinitionExists(group.Key))
                            new PrecursorWarning($"Tag definition for tag group {group.Key.Tag} not implemented");

                        foreach (var tag in group.ToList()) 
                        {
                            var validator = new TagDataValidiator(cache, stream);

                            try 
                            {
                                validator.VerifyTag(tag);
                            }
                            catch (Exception ex) 
                            {
                                new PrecursorWarning($"Failed to validate tag {tag}: {ex.Message}");
                                continue;
                            }

                            if (validator.Problems.Count > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"{tag}:");
                                foreach (var problem in validator.Problems)
                                    Console.WriteLine($"  {problem}");
                                Console.ResetColor();
                                Console.WriteLine();
                            }
                        }
                    }
                }
            }
        }
    }
}
