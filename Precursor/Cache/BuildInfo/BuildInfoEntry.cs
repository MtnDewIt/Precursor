using Precursor.Cache.BuildTable;
using Precursor.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public abstract bool VerifyBuildInfo(BuildTableProperties.BuildTableEntry build);

        public virtual void ParseCacheFiles()
        {
            foreach (var file in GetCacheFiles())
            {
                if (!GetCurrentCacheFiles().Any(x => Path.GetFileName(x) == file))
                {
                    new PrecursorWarning($"Missing Cache File: {file}");
                }
            }
        }

        public virtual void ParseSharedFiles()
        {
            foreach (var file in GetSharedFiles()) 
            {
                if (!GetCurrentSharedFiles().Any(x => Path.GetFileName(x) == file)) 
                {
                    new PrecursorWarning($"Missing Shared File: {file}");
                }
            }
        }

        public virtual void ParseResourceFiles()
        {
            foreach (var file in GetResourceFiles())
            {
                if (!GetCurrentResourceFiles().Any(x => Path.GetFileName(x) == file))
                {
                    new PrecursorWarning($"Missing Resource File: {file}");
                }
            }
        }

        public virtual bool ParseFileCount(int count) 
        {
            if (count == 0) 
            {
                new PrecursorWarning("No Valid Files Found in Directory, Skipping Verification...\n");
                return false;
            }

            return true;
        }
    }
}
