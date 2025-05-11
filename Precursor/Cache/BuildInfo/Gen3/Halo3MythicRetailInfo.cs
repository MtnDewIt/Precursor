using System.Collections.Generic;

namespace Precursor.Cache.BuildInfo.Gen3
{
    public class Halo3MythicRetailInfo : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.Halo3MythicRetail;

        public static readonly CacheGeneration Generation = CacheGeneration.Gen3;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "12065.08.08.26.0819.halo3_ship" 
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "campaign.map",
            "shared.map"
        };

        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public Halo3MythicRetailInfo()
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
