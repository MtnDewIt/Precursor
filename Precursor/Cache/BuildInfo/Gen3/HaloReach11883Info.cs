using System.Collections.Generic;

namespace Precursor.Cache.BuildInfo.Gen3
{
    public class HaloReach11883Info : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.HaloReach11883;

        public static readonly CacheGeneration Generation = CacheGeneration.Gen3;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "0237d057-1e3c-4390-9cfc-6108a911de01" 
        };

        public override CacheBuild GetBuild() => Build;
        public override CacheGeneration GetGeneration() => Generation;

        public override List<string> GetBuildStrings() => BuildStrings;

        public override List<string> GetCacheFiles() => null;
        public override List<string> GetSharedFiles() => null;
        public override List<string> GetResourceFiles() => null;

        public override List<string> GetCurrentMapFiles() => null;
        public override List<string> GetCurrentCacheFiles() => null;
        public override List<string> GetCurrentSharedFiles() => null;
        public override List<string> GetCurrentResourceFiles() => null;
    }
}
