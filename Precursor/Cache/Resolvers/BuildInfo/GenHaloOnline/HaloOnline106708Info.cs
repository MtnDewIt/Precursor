using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.GenHaloOnline
{
    public class HaloOnline106708Info : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.HaloOnline106708;

        public static readonly List<string> BuildStrings = new List<string> 
        {
            "1.106708 cert_ms23"
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "audio.dat",
            "resources.dat",
            "string_ids.dat",
            "tags.dat",
            "textures.dat",
            "textures_b.dat",
            "video.dat"
        };

        public override CacheBuild GetBuild() => Build;
        public override List<string> GetBuildStrings() => BuildStrings;
        public override List<string> GetSharedFiles() => SharedFiles;
    }
}
