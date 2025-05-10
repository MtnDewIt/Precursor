using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.GenMCC
{
    public class Halo2MCCInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo2MCC;

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

        public override CacheBuild GetBuild() => Build;
        public override List<string> GetBuildStrings() => BuildStrings;
        public override List<string> GetSharedFiles() => SharedFiles;
    }
}
