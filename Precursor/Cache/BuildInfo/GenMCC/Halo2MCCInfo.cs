using System.Collections.Generic;

namespace Precursor.Cache.BuildInfo.GenMCC
{
    public class Halo2MCCInfo : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.Halo2MCC;

        public static readonly CacheGeneration Generation = CacheGeneration.GenMCC;

        public static readonly List<string> BuildStrings = new List<string>
        {
            // No build string, srsly 343?
            ""
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "shared.map",
            "single_player_shared.map"
        };

        public static readonly List<string> ResourceFiles = new List<string> 
        {
            "sounds_cht.dat",
            "sounds_de.dat",
            "sounds_en.dat",
            "sounds_fr.dat",
            "sounds_it.dat",
            "sounds_jpn.dat",
            "sounds_kor.dat",
            "sounds_neutral.dat",
            "sounds_remastered.dat",
            "sounds_sp.dat",
            "textures.dat"
        };

        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;
        public List<string> CurrentResourceFiles;

        public Halo2MCCInfo()
        {
            CurrentCacheFiles = new List<string>();
            CurrentSharedFiles = new List<string>();
            CurrentResourceFiles = new List<string>();
        }

        public override CacheBuild GetBuild() => Build;
        public override CacheGeneration GetGeneration() => Generation;

        public override List<string> GetBuildStrings() => BuildStrings;

        public override List<string> GetCacheFiles() => null;
        public override List<string> GetSharedFiles() => SharedFiles;
        public override List<string> GetResourceFiles() => ResourceFiles;

        public override List<string> GetCurrentMapFiles() => null;
        public override List<string> GetCurrentCacheFiles() => CurrentCacheFiles;
        public override List<string> GetCurrentSharedFiles() => CurrentSharedFiles;
        public override List<string> GetCurrentResourceFiles() => CurrentResourceFiles;
    }
}
