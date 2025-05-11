using Precursor.Cache.BuildTable;
using Precursor.Common;
using System.IO;

namespace Precursor.Cache.Resolvers
{
    public abstract class CacheResolver
    {
        public static CacheResolver GetResolver(BuildTableProperties.BuildTableEntry build) 
        {
            if (string.IsNullOrEmpty(build.Path) || !Path.Exists(build.Path))
            {
                new PrecursorWarning("Invalid or Missing Path, Skipping Verification...");
                return null;
            }

            switch (build.Build)
            {
                case CacheBuild.HaloXbox:
                case CacheBuild.HaloPC:
                case CacheBuild.HaloCustomEdition:
                    return new CacheGen1Resolver();
                case CacheBuild.Halo2Alpha:
                case CacheBuild.Halo2Beta:
                case CacheBuild.Halo2Xbox:
                case CacheBuild.Halo2Vista:
                    return new CacheGen2Resolver();
                case CacheBuild.Halo3Beta:
                case CacheBuild.Halo3Retail:
                case CacheBuild.Halo3MythicRetail:
                case CacheBuild.Halo3ODST:
                case CacheBuild.HaloReach:
                    return new CacheGen3Resolver();
                case CacheBuild.HaloReach11883:
                    return new CacheGenMonolithicResolver();
                case CacheBuild.Halo4Retail:
                    return new CacheGen4Resolver();
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
                    return new CacheGenHaloOnlineResolver();
                case CacheBuild.Halo1MCC:
                case CacheBuild.Halo2MCC:
                case CacheBuild.Halo3MCC:
                case CacheBuild.Halo3ODSTMCC:
                case CacheBuild.HaloReachMCC:
                case CacheBuild.Halo4MCC:
                case CacheBuild.Halo2AMPMCC:
                    return new CacheMCCResolver();
            }

            return null;
        }

        public abstract void VerifyBuild(BuildTableProperties.BuildTableEntry build);
        public abstract bool IsValidCacheFile();
    }
}
