using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.GenMCC
{
    public class HaloReachMCCInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo4MCC;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "Jun 21 2023 15:35:31"
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
