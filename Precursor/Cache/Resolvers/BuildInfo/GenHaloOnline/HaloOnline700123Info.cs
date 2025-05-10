using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.GenHaloOnline
{
    public class HaloOnline700123Info : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.HaloOnline700123;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "12.1.700123 cert_ms30_oct19"
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
