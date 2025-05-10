using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen3
{
    public class Halo3BetaInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo3Beta;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "09699.07.05.01.1534.delta" 
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
