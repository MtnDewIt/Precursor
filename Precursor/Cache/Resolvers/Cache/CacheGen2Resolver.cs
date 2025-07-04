using Precursor.Cache.BuildInfo;
using Precursor.Cache.BuildInfo.Gen2;
using Precursor.Cache.BuildTable;

namespace Precursor.Cache.Resolvers.Cache
{
    public class CacheGen2Resolver : CacheResolver
    {
        public override BuildInfoEntry VerifyBuild(BuildTableProperties.BuildTableEntry build) 
        {
            BuildInfoEntry buildInfo = null;

            switch (build.Build) 
            {
                case CacheBuild.Halo2Alpha:
                    buildInfo = new Halo2AlphaInfo();
                    break;
                case CacheBuild.Halo2Beta:
                    buildInfo = new Halo2BetaInfo();
                    break;
                case CacheBuild.Halo2Xbox:
                    buildInfo = new Halo2XboxInfo();
                    break;
                case CacheBuild.Halo2Vista:
                    buildInfo = new Halo2VistaInfo();
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
