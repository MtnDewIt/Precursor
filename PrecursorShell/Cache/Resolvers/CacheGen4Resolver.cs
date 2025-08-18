using PrecursorShell.Cache.BuildInfo;
using PrecursorShell.Cache.BuildInfo.Gen4;
using PrecursorShell.Cache.BuildTable;

namespace PrecursorShell.Cache.Resolvers
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
