using System.Collections.Generic;

namespace Precursor.Cache.BuildInfo.Gen1
{
    public class HaloXboxInfo : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.HaloXbox;

        public static readonly CacheGeneration Generation = CacheGeneration.Gen1;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "01.10.12.2276" 
        };

        public List<string> CurrentCacheFiles;

        public HaloXboxInfo()
        {
            CurrentCacheFiles = new List<string>();
        }

        public override CacheBuild GetBuild() => Build;
        public override CacheGeneration GetGeneration() => Generation;

        public override List<string> GetBuildStrings() => BuildStrings;

        public override List<string> GetCacheFiles() => null;
        public override List<string> GetSharedFiles() => null;
        public override List<string> GetResourceFiles() => null;

        public override List<string> GetCurrentMapFiles() => null;
        public override List<string> GetCurrentCacheFiles() => CurrentCacheFiles;
        public override List<string> GetCurrentSharedFiles() => null;
        public override List<string> GetCurrentResourceFiles() => null;
    }
}
