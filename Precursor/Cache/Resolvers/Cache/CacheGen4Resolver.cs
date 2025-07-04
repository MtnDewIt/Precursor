using Precursor.Cache.BuildInfo;
using Precursor.Cache.BuildInfo.Gen4;
using Precursor.Cache.BuildTable;

namespace Precursor.Cache.Resolvers.Cache
{
    public class CacheGen4Resolver : CacheResolver
    {
        public override BuildInfoEntry VerifyBuild(BuildTableProperties.BuildTableEntry build)
        {
            BuildInfoEntry buildInfo = null;

            switch (build.Build)
            {
                case CacheBuild.Halo4Retail:
                    buildInfo = new Halo4RetailInfo();
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
