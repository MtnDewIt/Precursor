using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.GenMCC
{
    public class Halo3MCCInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo3MCC;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "Dec 21 2023 22:31:37"
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
