using System.Collections.Generic;

namespace Precursor.Cache.BuildInfo.GenHaloOnline
{
    public class HaloOnline235640Info : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.HaloOnline235640;

        public static readonly CacheGeneration Generation = CacheGeneration.GenHaloOnline;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "1.235640 cert_ms25"
        };

        public static readonly List<string> CacheFiles = new List<string>
        {
            "tags.dat",
        };

        public static readonly List<string> SharedFiles = new List<string>
        {
            "audio.dat",
            "lightmaps.dat",
            "render_models.dat",
            "resources.dat",
            "string_ids.dat",
            "textures.dat",
            "textures_b.dat",
            "video.dat"
        };

        public List<string> CurrentMapFiles;
        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public HaloOnline235640Info()
        {
            CurrentMapFiles = new List<string>();
            CurrentCacheFiles = new List<string>();
            CurrentSharedFiles = new List<string>();
        }

        public override CacheBuild GetBuild() => Build;
        public override CacheGeneration GetGeneration() => Generation;

        public override List<string> GetBuildStrings() => BuildStrings;

        public override List<string> GetCacheFiles() => CacheFiles;
        public override List<string> GetSharedFiles() => SharedFiles;
        public override List<string> GetResourceFiles() => null;

        public override List<string> GetCurrentMapFiles() => CurrentMapFiles;
        public override List<string> GetCurrentCacheFiles() => CurrentCacheFiles;
        public override List<string> GetCurrentSharedFiles() => CurrentSharedFiles;
        public override List<string> GetCurrentResourceFiles() => null;
    }
}
