using System.Collections.Generic;

namespace Precursor.Cache.BuildInfo.Gen3
{
    public class Halo3BetaInfo : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.Halo3Beta;

        public static readonly CacheGeneration Generation = CacheGeneration.Gen3;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "09699.07.05.01.1534.delta" 
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "shared.map"
        };

        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public Halo3BetaInfo()
        {
            CurrentCacheFiles = new List<string>();
            CurrentSharedFiles = new List<string>();
        }

        public override CacheBuild GetBuild() => Build;
        public override CacheGeneration GetGeneration() => Generation;

        public override List<string> GetBuildStrings() => BuildStrings;

        public override List<string> GetCacheFiles() => null;
        public override List<string> GetSharedFiles() => SharedFiles;
        public override List<string> GetResourceFiles() => null;

        public override List<string> GetCurrentMapFiles() => null;
        public override List<string> GetCurrentCacheFiles() => CurrentCacheFiles;
        public override List<string> GetCurrentSharedFiles() => CurrentSharedFiles;
        public override List<string> GetCurrentResourceFiles() => null;
    }
}
