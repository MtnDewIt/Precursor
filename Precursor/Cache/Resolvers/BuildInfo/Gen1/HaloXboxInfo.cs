using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen1
{
    public class HaloXboxInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.HaloXbox;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "01.10.12.2276" 
        };

        public override CacheBuild GetBuild() => Build;
        public override List<string> GetBuildStrings() => BuildStrings;
        public override List<string> GetSharedFiles() => null;
    }
}
