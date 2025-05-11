using System.Collections.Generic;

namespace Precursor.Cache.BuildInfo.GenMCC
{
    public class Halo4MCCInfo : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.Halo4MCC;

        public static readonly CacheGeneration Generation = CacheGeneration.GenMCC;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "Apr  1 2023 17:35:22"
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "campaign.map",
            "shared.map"
        };

        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public Halo4MCCInfo()
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
