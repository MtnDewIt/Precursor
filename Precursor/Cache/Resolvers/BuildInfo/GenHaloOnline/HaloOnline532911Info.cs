using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.GenHaloOnline
{
    public class HaloOnline532911Info : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.HaloOnline532911;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "11.1.532911 Live"
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "audio.dat",
            "lightmaps.dat",
            "render_models.dat",
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
