using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.GenMCC
{
    public class Halo3ODSTMCCInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.MCCRetail;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "May 16 2023 11:44:41"
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
