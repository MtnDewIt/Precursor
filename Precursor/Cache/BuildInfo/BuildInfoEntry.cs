using System.Collections.Generic;

namespace Precursor.Cache.BuildInfo
{
    public abstract class BuildInfoEntry
    {
        public abstract CacheBuild GetBuild();
        public abstract CacheGeneration GetGeneration();

        public abstract List<string> GetBuildStrings();

        public abstract List<string> GetCacheFiles();
        public abstract List<string> GetSharedFiles();
        public abstract List<string> GetResourceFiles();

        public abstract List<string> GetCurrentMapFiles();
        public abstract List<string> GetCurrentCacheFiles();
        public abstract List<string> GetCurrentSharedFiles();
        public abstract List<string> GetCurrentResourceFiles();
    }
}
