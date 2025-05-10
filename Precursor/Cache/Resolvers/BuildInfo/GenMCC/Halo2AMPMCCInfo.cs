using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.GenMCC
{
    public class Halo2AMPMCCInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo2AMPMCC;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "Jun 13 2023 20:21:18"
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
