using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.GenMCC
{
    public class Halo1MCCInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo1MCC;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "01.03.43.0000"
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "bitmaps.map"
        };

        public override CacheBuild GetBuild() => Build;
        public override List<string> GetBuildStrings() => BuildStrings;
        public override List<string> GetSharedFiles() => SharedFiles;
    }
}
