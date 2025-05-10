using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen3
{
    public class Halo3ODSTInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo3ODST;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "13895.09.04.27.2201.atlas_relea"
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "campaign.map",
            "shared.map"
        };

        public override CacheBuild GetBuild() => Build;
        public override List<string> GetBuildStrings() => BuildStrings;
        public override List<string> GetSharedFiles() => SharedFiles;
    }
}
