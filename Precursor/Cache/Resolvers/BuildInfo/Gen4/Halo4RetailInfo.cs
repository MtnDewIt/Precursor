using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen4
{
    public class Halo4RetailInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo4Retail;

        public static readonly List<string> BuildStrings = new List<string> 
        {
            "20810.12.09.22.1647.main",
            "21122.12.11.21.0101.main",
            "21165.12.12.12.0112.main",
            "21339.13.02.05.0117.main",
            "21391.13.03.13.1711.main"
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
