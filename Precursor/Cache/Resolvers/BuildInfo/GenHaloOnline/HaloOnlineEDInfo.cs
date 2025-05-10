using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo.GenHaloOnline
{
    public class HaloOnlineEDInfo : BuildInfo
    {
        public static readonly CacheBuild Build = CacheBuild.HaloOnlineED;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "eldewrito" 
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "audio.dat",
            "resources.dat",
            "resources_b.dat",
            "string_ids.dat",
            "tags.dat",
            "textures.dat",
            "textures_b.dat",
        };

        public override CacheBuild GetBuild() => Build;
        public override List<string> GetBuildStrings() => BuildStrings;
        public override List<string> GetSharedFiles() => SharedFiles;
    }
}
