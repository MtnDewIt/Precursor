using Precursor.Cache.BuildTable;
using Precursor.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.Cache;

namespace Precursor.Cache.BuildInfo
{
    public abstract class BuildInfoEntry
    {
        public abstract CacheBuild GetBuild();
        public abstract CacheVersion GetVersion();
        public abstract CachePlatform GetPlatform();
        public abstract CacheGeneration GetGeneration();

        public abstract string GetResourcePath();

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

        public virtual CacheResource GetResourceType(string fileName)
        {
            switch (fileName)
            {
                case "tags.dat":
                    return CacheResource.Tags;
                case "string_ids.dat":
                    return CacheResource.StringIds;
                case "audio.dat":
                    return CacheResource.Audio;
                case "lightmaps.dat":
                    return CacheResource.Lightmaps;
                case "render_models.dat":
                    return CacheResource.RenderModels;
                case "resources.dat":
                    return CacheResource.Resources;
                case "resources_b.dat":
                    return CacheResource.ResourcesB;
                case "textures.dat":
                    return CacheResource.Textures;
                case "textures_b.dat":
                    return CacheResource.TexturesB;
                case "video.dat":
                    return CacheResource.Video;
                default:
                    return CacheResource.None;
            }
        }
    }
}
