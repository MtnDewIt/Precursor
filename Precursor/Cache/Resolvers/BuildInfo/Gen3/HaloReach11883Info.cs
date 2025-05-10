using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen3
{
    public class HaloReach11883Info : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.HaloReach11883;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "0237d057-1e3c-4390-9cfc-6108a911de01" 
        };

        public override CacheBuild GetBuild() => Build;
        public override List<string> GetBuildStrings() => BuildStrings;
        public override List<string> GetSharedFiles() => null;
    }
}
