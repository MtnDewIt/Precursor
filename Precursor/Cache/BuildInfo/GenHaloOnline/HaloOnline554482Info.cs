using Precursor.Cache.BuildTable;
using Precursor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.Cache.HaloOnline;
using TagTool.IO;
using TagTool.Serialization;

namespace Precursor.Cache.BuildInfo.GenHaloOnline
{
    public class HaloOnline554482Info : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.HaloOnline554482;

        public static readonly CacheVersion Version = CacheVersion.HaloOnline554482;

        public static readonly CachePlatform Platform = CachePlatform.Original;

        public static readonly CacheGeneration Generation = CacheGeneration.GenHaloOnline;

        public static readonly List<string> BuildStrings = new List<string>
        {
            "11.1.554482 Live"
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

        public static readonly Dictionary<CacheResource, string> BuildDateTable = new Dictionary<CacheResource, string>
        {
            { CacheResource.Tags, "2015-09-29 10:14:31.9550501" },
            { CacheResource.Audio, "2015-09-29 10:14:32.2100756" },
            { CacheResource.Lightmaps, "2015-09-29 10:14:32.2100756" },
            { CacheResource.RenderModels, "2015-09-29 10:14:32.2100756" },
            { CacheResource.Resources, "2015-09-29 10:14:32.2090755" },
            { CacheResource.Textures, "2015-09-29 10:14:32.2100756" },
            { CacheResource.TexturesB, "2015-09-29 10:14:32.2100756" },
            { CacheResource.Video, "2015-09-29 10:14:32.2100756" },
        };

        public List<string> CurrentMapFiles;
        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public HaloOnline554482Info()
        {
            CurrentMapFiles = new List<string>();
            CurrentCacheFiles = new List<string>();
            CurrentSharedFiles = new List<string>();
        }

        public override bool VerifyBuildInfo(BuildTableProperties.BuildTableEntry build)
        {
            var files = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories).ToList();
            var sharedFiles = Directory.EnumerateFiles(build.Path, "*.dat", SearchOption.AllDirectories).ToList();

            if (!ParseFileCount(files.Count))
            {
                return false;
            }

            var totalFileCount = files.Count + sharedFiles.Count;
            var validFiles = 0;

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                using (var stream = fileInfo.OpenRead())
                using (var reader = new EndianReader(stream))
                {
                    var mapFile = new MapFile();

                    mapFile.Read(reader);

                    if (!mapFile.Header.IsValid())
                    {
                        new PrecursorWarning($"Invalid Map File: {Path.GetFileName(file)}");
                        continue;
                    }

                    if (BuildStrings.Contains(mapFile.Header.GetBuild()))
                    {
                        CurrentCacheFiles.Add(file);
                        validFiles++;
                    }
                    else
                    {
                        new PrecursorWarning($"Invalid Build String: {Path.GetFileName(file)} - {mapFile.Header.GetBuild()} != {BuildStrings.FirstOrDefault()}");
                        continue;
                    }
                }
            }

            foreach (var file in sharedFiles)
            {
                var fileInfo = new FileInfo(file);

                using (var stream = fileInfo.OpenRead())
                using (var reader = new EndianReader(stream))
                {
                    var dataContext = new DataSerializationContext(reader);
                    var deserializer = new TagDeserializer(Version, Platform);
                    var resourceType = GetResourceType(fileInfo.Name);

                    if (resourceType == CacheResource.None || resourceType != CacheResource.StringIds && !BuildDateTable.ContainsKey(resourceType))
                    {
                        new PrecursorWarning($"Invalid Cache File: {fileInfo.Name} - Unsupported or Invalid Cache Type");
                        continue;
                    }

                    if (resourceType == CacheResource.Tags)
                    {
                        TagCacheHaloOnlineHeader tagCacheHeader = null;

                        try
                        {
                            tagCacheHeader = deserializer.Deserialize<TagCacheHaloOnlineHeader>(dataContext);
                        }
                        catch
                        {
                            new PrecursorWarning($"Invalid Cache File: {fileInfo.Name} - Failed to deserialize tag cache header");
                            continue;
                        }

                        var tagCacheModificationDate = new LastModificationDate(tagCacheHeader.CreationTime);
                        var tagCacheDate = $"{tagCacheModificationDate.GetModificationDate():yyyy-MM-dd HH:mm:ss.FFFFFFF}";

                        if (BuildDateTable[resourceType] == tagCacheDate)
                        {
                            CurrentCacheFiles.Add(file);
                            validFiles++;
                        }
                        else
                        {
                            new PrecursorWarning($"Invalid Cache Build Date: {fileInfo.Name} - {tagCacheDate} != {BuildDateTable[resourceType]}");
                            continue;
                        }
                    }
                    else if (resourceType == CacheResource.StringIds)
                    {
                        CurrentSharedFiles.Add(file);
                        validFiles++;
                    }
                    else if (resourceType != CacheResource.None)
                    {
                        ResourceCacheHaloOnlineHeader resourceCacheHeader = null;

                        try
                        {
                            resourceCacheHeader = deserializer.Deserialize<ResourceCacheHaloOnlineHeader>(dataContext);
                        }
                        catch
                        {
                            new PrecursorWarning($"Invalid Cache File: {fileInfo.Name} - Failed to deserialize resource cache header");
                            continue;
                        }

                        var resourceCacheModificationDate = new LastModificationDate(resourceCacheHeader.CreationTime);
                        var resourceCacheDate = $"{resourceCacheModificationDate.GetModificationDate():yyyy-MM-dd HH:mm:ss.FFFFFFF}";

                        if (BuildDateTable[resourceType] == resourceCacheDate)
                        {
                            CurrentSharedFiles.Add(file);
                            validFiles++;
                        }
                        else
                        {
                            new PrecursorWarning($"Invalid Cache Build Date: {fileInfo.Name} - {resourceCacheDate} != {BuildDateTable[resourceType]}");
                            continue;
                        }
                    }
                }
            }

            ParseCacheFiles();
            ParseSharedFiles();

            Console.WriteLine($"Successfully Verified {validFiles}/{totalFileCount} Files\n");

            return true;
        }

        public override CacheBuild GetBuild() => Build;
        public override CacheVersion GetVersion() => Version;
        public override CachePlatform GetPlatform() => Platform;
        public override CacheGeneration GetGeneration() => Generation;

        public override string GetResourcePath() => null;

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
