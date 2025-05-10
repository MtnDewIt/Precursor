using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen2
{
    public class Halo2BetaInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo2Beta;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "02.06.28.07902" 
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "shared.map"
        };

        public override CacheBuild GetBuild() => Build;
        public override List<string> GetBuildStrings() => BuildStrings;
        public override List<string> GetSharedFiles() => SharedFiles;
    }
}
