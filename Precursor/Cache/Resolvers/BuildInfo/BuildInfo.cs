using System.Collections.Generic;

namespace Precursor.Cache.Resolvers.BuildInfo
{
    public abstract class BuildInfo
    {
        public abstract CacheBuild GetBuild();
        public abstract List<string> GetBuildStrings();
        public abstract List<string> GetSharedFiles();
    }
}
