using Precursor.Cache.Handlers;
using Precursor.Common;
using System;
using System.IO;

namespace Precursor.Cache.Resolvers
{
    public class CacheResolver
    {
        public static void VerifyBuilds(string inputPath)
        {
            if (!File.Exists(inputPath))
            {
                new PrecursorWarning("Unable to locate Precursor.json");
                return;
            }

            var jsonData = File.ReadAllText(inputPath);

            var handler = new CacheObjectHandler();

            var cacheObject = handler.Deserialize(jsonData);

            foreach (var build in cacheObject.Builds)
            {
                Console.WriteLine($"Verifying {build.Build} Cache Files...");

                switch (build.Build) 
                {
                    case CacheBuild.HaloXbox:
                    case CacheBuild.HaloPC:
                    case CacheBuild.HaloCustomEdition:
                        var resolverGen1 = new CacheGen1Resolver();
                        resolverGen1.VerifyBuild(build);
                        break;
                    case CacheBuild.Halo2Alpha:
                    case CacheBuild.Halo2Beta:
                    case CacheBuild.Halo2Xbox:
                    case CacheBuild.Halo2Vista:
                        var resolverGen2 = new CacheGen2Resolver();
                        resolverGen2.VerifyBuild(build);
                        break;
                    case CacheBuild.Halo3Beta:
                    case CacheBuild.Halo3Retail:
                    case CacheBuild.Halo3MythicRetail:
                    case CacheBuild.Halo3ODST:
                    case CacheBuild.HaloReach:
                        var resolverGen3 = new CacheGen3Resolver();
                        resolverGen3.VerifyBuild(build);
                        break;
                    case CacheBuild.HaloReach11883:
                        var resolverGenMonolithic = new CacheGenMonolithicResolver();
                        resolverGenMonolithic.VerifyBuild(build);
                        break;
                    case CacheBuild.Halo4Retail:
                        var resolverGen4 = new CacheGen4Resolver();
                        resolverGen4.VerifyBuild(build);
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
                        var resolverGenHaloOnline = new CacheGenHaloOnlineResolver();
                        resolverGenHaloOnline.VerifyBuild(build);
                        break;
                    case CacheBuild.MCCRetail:
                        var resolverMCC = new CacheMCCResolver();
                        resolverMCC.VerifyBuild(build);
                        break;
                }
            }
        }
    }
}
