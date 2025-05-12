using Precursor.Cache.BuildTable;
using Precursor.Cache.BuildInfo.GenHaloOnline;
using Precursor.Cache.BuildInfo;

namespace Precursor.Cache.Resolvers
{
    public class CacheGenHaloOnlineResolver : CacheResolver
    {
        public override BuildInfoEntry VerifyBuild(BuildTableProperties.BuildTableEntry build)
        {
            BuildInfoEntry buildInfo = null;

            switch (build.Build)
            {
                case CacheBuild.HaloOnlineED:
                    buildInfo = new HaloOnlineEDInfo();
                    break;
                case CacheBuild.HaloOnline106708:
                    buildInfo = new HaloOnline106708Info();
                    break;
                case CacheBuild.HaloOnline235640:
                    buildInfo = new HaloOnline235640Info();
                    break;
                case CacheBuild.HaloOnline301003:
                    buildInfo = new HaloOnline301003Info();
                    break;
                case CacheBuild.HaloOnline327043:
                    buildInfo = new HaloOnline327043Info();
                    break;
                case CacheBuild.HaloOnline372731:
                    buildInfo = new HaloOnline372731Info();
                    break;
                case CacheBuild.HaloOnline416097:
                    buildInfo = new HaloOnline416097Info();
                    break;
                case CacheBuild.HaloOnline430475:
                    buildInfo = new HaloOnline430475Info();
                    break;
                case CacheBuild.HaloOnline454665:
                    buildInfo = new HaloOnline454665Info();
                    break;
                case CacheBuild.HaloOnline449175:
                    buildInfo = new HaloOnline449175Info();
                    break;
                case CacheBuild.HaloOnline498295:
                    buildInfo = new HaloOnline498295Info();
                    break;
                case CacheBuild.HaloOnline530605:
                    buildInfo = new HaloOnline530605Info();
                    break;
                case CacheBuild.HaloOnline532911:
                    buildInfo = new HaloOnline532911Info();
                    break;
                case CacheBuild.HaloOnline554482:
                    buildInfo = new HaloOnline554482Info();
                    break;
                case CacheBuild.HaloOnline571627:
                    buildInfo = new HaloOnline571627Info();
                    break;
                case CacheBuild.HaloOnline604673:
                    buildInfo = new HaloOnline604673Info();
                    break;
                case CacheBuild.HaloOnline700123:
                    buildInfo = new HaloOnline700123Info();
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
