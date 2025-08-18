using PrecursorShell.Cache.BuildInfo;
using PrecursorShell.Cache.BuildInfo.GenMCC;
using PrecursorShell.Cache.BuildTable;

namespace PrecursorShell.Cache.Resolvers
{
    public class CacheMCCResolver : CacheResolver
    {
        public override BuildInfoEntry VerifyBuild(BuildTableProperties.BuildTableEntry build)
        {
            BuildInfoEntry buildInfo = null;

            switch (build.Build)
            {
                case CacheBuild.Halo1MCC:
                    buildInfo = new Halo1MCCInfo();
                    break;
                case CacheBuild.Halo2MCC:
                    buildInfo = new Halo2MCCInfo();
                    break;
                case CacheBuild.Halo3MCC:
                    buildInfo = new Halo3MCCInfo();
                    break;
                case CacheBuild.Halo3ODSTMCC:
                    buildInfo = new Halo3ODSTMCCInfo();
                    break;
                case CacheBuild.HaloReachMCC:
                    buildInfo = new HaloReachMCCInfo();
                    break;
                case CacheBuild.Halo4MCC:
                    buildInfo = new Halo4MCCInfo();
                    break;
                case CacheBuild.Halo2AMPMCC:
                    buildInfo = new Halo2AMPMCCInfo();
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
