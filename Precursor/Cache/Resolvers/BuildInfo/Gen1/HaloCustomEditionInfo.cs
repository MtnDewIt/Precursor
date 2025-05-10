using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen1
{
    public class HaloCustomEditionInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.HaloCustomEdition;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "01.00.00.0609" 
        };

        public static readonly List<string> SharedFiles = new List<string> 
        {
            "bitmaps.map",
            "loc.map",
            "sounds.map"
        };

        public override CacheBuild GetBuild() => Build;
        public override List<string> GetBuildStrings() => BuildStrings;
        public override List<string> GetSharedFiles() => SharedFiles;
    }
}
