using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.Gen2
{
    public class Halo2VistaInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.Halo2Vista;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "11081.07.04.30.0934.main" 
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "shared.map",
            "single_player_shared.map"
        };

        public override CacheBuild GetBuild() => Build;
        public override List<string> GetBuildStrings() => BuildStrings;
        public override List<string> GetSharedFiles() => SharedFiles;
    }
}
