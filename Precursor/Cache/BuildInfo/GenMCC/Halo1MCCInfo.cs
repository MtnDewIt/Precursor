using System.Collections.Generic;

namespace Precursor.Cache.BuildInfo.GenMCC
{
    public class Halo1MCCInfo : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.Halo1MCC;

        public static readonly CacheGeneration Generation = CacheGeneration.GenMCC;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "01.03.43.0000"
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "bitmaps.map"
        };

        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public Halo1MCCInfo()
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
