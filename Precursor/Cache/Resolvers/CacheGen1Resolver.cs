using Precursor.Cache.BuildInfo;
using Precursor.Cache.BuildInfo.Gen1;
using Precursor.Cache.BuildTable;

namespace Precursor.Cache.Resolvers
{
    public class CacheGen1Resolver : CacheResolver
    {
        public override BuildInfoEntry VerifyBuild(BuildTableProperties.BuildTableEntry build) 
        {
            BuildInfoEntry buildInfo = null;

            switch (build.Build)
            {
                case CacheBuild.HaloXbox:
                    buildInfo = new HaloXboxInfo();
                    break;
                case CacheBuild.HaloPC:
                    buildInfo = new HaloPCInfo();
                    break;
                case CacheBuild.HaloCustomEdition:
                    buildInfo = new HaloCustomEditionInfo();
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
