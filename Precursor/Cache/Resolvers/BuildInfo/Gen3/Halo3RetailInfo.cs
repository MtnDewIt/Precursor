using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen3
{
    public class Halo3RetailInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo3Retail;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "11855.07.08.20.2317.halo3_ship" 
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


