using Precursor.Cache.BuildInfo;
using Precursor.Cache.BuildInfo.Gen3;
using Precursor.Cache.BuildTable;

namespace Precursor.Cache.Resolvers.Cache
{
    public class CacheGenMonolithicResolver : CacheResolver
    {
        public override BuildInfoEntry VerifyBuild(BuildTableProperties.BuildTableEntry build)
        {
            BuildInfoEntry buildInfo = null;

            switch (build.Build)
            {
                case CacheBuild.HaloReach11883:
                    buildInfo = new HaloReach11883Info();
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
