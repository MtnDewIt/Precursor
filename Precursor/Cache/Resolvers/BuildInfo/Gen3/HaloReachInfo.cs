using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen3
{
    public class HaloReachInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.HaloReach;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "11860.10.07.24.0147.omaha_relea" 
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
