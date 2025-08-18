using PrecursorShell.Cache.BuildTable;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.Cache.HaloOnline;
using TagTool.IO;
using TagTool.Serialization;

namespace PrecursorShell.Cache.BuildInfo.GenHaloOnline
{
    public class HaloOnline498295Info : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.HaloOnline498295;

        public static readonly CacheVersion Version = CacheVersion.HaloOnline498295;

        public static readonly CachePlatform Platform = CachePlatform.Original;

        public static readonly CacheGeneration Generation = CacheGeneration.GenHaloOnline;

        public static readonly string ResourcePath = @"Resources\GenHaloOnline\HaloOnline498295";

        public static readonly List<string> BuildStrings = new List<string>
        {
            "11.1.498295 Live"
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
            { CacheResource.Tags, "2015-09-04 13:36:11.6879375" },
            { CacheResource.Audio, "2015-09-04 13:36:11.9149602" },
            { CacheResource.Lightmaps, "2015-09-04 13:36:11.9149602" },
            { CacheResource.RenderModels, "2015-09-04 13:36:11.9149602" },
            { CacheResource.Resources, "2015-09-04 13:36:11.9139601" },
            { CacheResource.Textures, "2015-09-04 13:36:11.9139601" },
            { CacheResource.TexturesB, "2015-09-04 13:36:11.9149602" },
            { CacheResource.Video, "2015-09-04 13:36:11.9149602" },
        };

        public List<string> CurrentMapFiles;
        public List<string> CurrentCacheFiles;
        public List<string> CurrentSharedFiles;

        public HaloOnline498295Info()
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

                    try
                    {
                        mapFile.Read(reader);
                    }
                    catch (Exception ex)
                    {
                        new PrecursorWarning($"Failed to parse file \"{fileInfo.Name}\": {ex.Message}");
                        continue;
                    }

                    if (!mapFile.Header.IsValid())
                    {
                        new PrecursorWarning($"Invalid Map File: {fileInfo.Name}");
                        continue;
                    }

                    if (BuildStrings.Contains(mapFile.Header.GetBuild()))
                    {
                        try
                        {
                            GenerateJSON(mapFile, fileInfo.Name, ResourcePath);
                        }
                        catch (Exception ex)
                        {
                            new PrecursorWarning($"Failed to serialize JSON \"{fileInfo.Name}\": {ex.Message}");
                            continue;
                        }

                        CurrentMapFiles.Add(file);
                        validFiles++;
                    }
                    else
                    {
                        new PrecursorWarning($"Invalid Build String: {fileInfo.Name} - {mapFile.Header.GetBuild()} != {BuildStrings.FirstOrDefault()}");
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
                    var resourceType = GetResourceType(fileInfo.Name);

                    if (resourceType == CacheResource.None || resourceType != CacheResource.StringIds && !BuildDateTable.ContainsKey(resourceType))
                    {
                        new PrecursorWarning($"Invalid File: {fileInfo.Name} - Unsupported or invalid resource type");
                        continue;
                    }

                    if (resourceType != CacheResource.None && resourceType != CacheResource.StringIds)
                    {
                        CacheFileSectionHeader header = null;

                        try
                        {
                            header = CacheFileSectionHeader.ReadHeader(reader, Version, Platform);
                        }
                        catch
                        {
                            new PrecursorWarning($"Invalid File: {fileInfo.Name} - Failed to deserialize file section header");
                            continue;
                        }

                        var timestamp = LastModificationDate.GetTimestamp(header.CreationDate);

                        if (BuildDateTable[resourceType] == timestamp)
                        {
                            if (resourceType == CacheResource.Tags)
                            {
                                CurrentCacheFiles.Add(file);
                                validFiles++;
                            }
                            else
                            {
                                CurrentSharedFiles.Add(file);
                                validFiles++;
                            }
                        }
                        else
                        {
                            new PrecursorWarning($"Invalid Build Date: {fileInfo.Name} - {timestamp} != {BuildDateTable[resourceType]}");
                            continue;
                        }
                    }
                    else if (resourceType == CacheResource.StringIds)
                    {
                        CurrentSharedFiles.Add(file);
                        validFiles++;
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

        public override string GetResourcePath() => ResourcePath;

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
