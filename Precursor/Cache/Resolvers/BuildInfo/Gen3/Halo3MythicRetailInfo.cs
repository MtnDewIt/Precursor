using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen3
{
    public class Halo3MythicRetailInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo3MythicRetail;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "12065.08.08.26.0819.halo3_ship" 
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
