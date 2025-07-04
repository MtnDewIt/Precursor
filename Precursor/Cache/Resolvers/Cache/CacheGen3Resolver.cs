using Precursor.Cache.BuildInfo;
using Precursor.Cache.BuildInfo.Gen3;
using Precursor.Cache.BuildTable;

namespace Precursor.Cache.Resolvers.Cache
{
    public class CacheGen3Resolver : CacheResolver
    {
        public override BuildInfoEntry VerifyBuild(BuildTableProperties.BuildTableEntry build)
        {
            BuildInfoEntry buildInfo = null;

            switch (build.Build)
            {
                case CacheBuild.Halo3Beta:
                    buildInfo = new Halo3BetaInfo();
                    break;
                case CacheBuild.Halo3Retail:
                    buildInfo = new Halo3RetailInfo();
                    break;
                case CacheBuild.Halo3MythicRetail:
                    buildInfo = new Halo3MythicRetailInfo();
                    break;
                case CacheBuild.Halo3ODST:
                    buildInfo = new Halo3ODSTInfo();
                    break;
                case CacheBuild.HaloReach:
                    buildInfo = new HaloReachInfo();
                    break;
            }

            if (buildInfo != null)
            {
                if (buildInfo.VerifyBuildInfo(build))
                {
                    return buildInfo;
                }
            }

            return null;
        }
    }
}
