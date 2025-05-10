using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.GenMCC
{
    public class Halo4MCCInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.HaloReachMCC;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "Apr  1 2023 17:35:22"
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
