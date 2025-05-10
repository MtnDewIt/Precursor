using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen1
{
    public class HaloPCInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.HaloPC;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "01.00.00.0564" 
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "bitmaps.map",
            "sounds.map"
        };

        public override CacheBuild GetBuild() => Build;
        public override List<string> GetBuildStrings() => BuildStrings;
        public override List<string> GetSharedFiles() => SharedFiles;
    }
}
