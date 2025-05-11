using System.Collections.Generic;

namespace Precursor.Cache.BuildInfo.Gen2
{
    public class Halo2VistaInfo : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.Halo2Vista;

        public static readonly CacheGeneration Generation = CacheGeneration.Gen2;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "11081.07.04.30.0934.main" 
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "shared.map",
            "single_player_shared.map"
        };

        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public Halo2VistaInfo()
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
