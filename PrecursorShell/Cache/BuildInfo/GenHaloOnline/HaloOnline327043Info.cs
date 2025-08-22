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

namespace PrecursorShell.Cache.BuildInfo.GenHaloOnline
{
    public class HaloOnline327043Info : BuildTableEntry
    {
        public override CacheBuild Build => CacheBuild.HaloOnline327043;
        public override CacheVersion Version => CacheVersion.HaloOnline327043;
        public override CachePlatform Platform => CachePlatform.Original;
        public override CacheGeneration Generation => CacheGeneration.GenHaloOnline;

        public override string ResourcePath => @"Resources\GenHaloOnline\HaloOnline327043";

        public override List<string> BuildStrings => new List<string>
        {
            "0.4.1.327043 cert_MS26_new"
        };

        public override List<string> CacheFiles => new List<string>
        {
            "tags.dat",
        };
        public override List<string> SharedFiles => new List<string>
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
        public override List<string> ResourceFiles => null;

        public static readonly Dictionary<CacheResource, string> BuildDateTable = new Dictionary<CacheResource, string>
        {
            { CacheResource.Tags, "2015-06-29 09:41:56.0458507" },
            { CacheResource.Audio, "2015-06-29 09:41:56.2748736" },
            { CacheResource.Lightmaps, "2015-06-29 09:41:56.2748736" },
            { CacheResource.RenderModels, "2015-06-29 09:41:56.2748736" },
            { CacheResource.Resources, "2015-06-29 09:41:56.2738735" },
            { CacheResource.Textures, "2015-06-29 09:41:56.2738735" },
            { CacheResource.TexturesB, "2015-06-29 09:41:56.2748736" },
            { CacheResource.Video, "2015-06-29 09:41:56.2748736" },
        };

        public override bool VerifyBuildInfo(BuildTableConfig.BuildTableEntry build)
        {
            var files = Directory.EnumerateFiles(build.Path, "*.*", SearchOption.AllDirectories).Where(x => x.EndsWith(".map") || x.EndsWith(".dat"));

            if (!ParseFileCount(files.Count()))
            {
                return false;
            }

            var validFiles = 0;

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                using (var stream = fileInfo.OpenRead())
                using (var reader = new EndianReader(stream))
                {
                    if (!CacheFiles.Contains(fileInfo.Name) && !SharedFiles.Contains(fileInfo.Name))
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

                    if (CacheFiles.Contains(fileInfo.Name) || SharedFiles.Contains(fileInfo.Name))
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

                        if (resourceType == CacheResource.StringIds)
                        {
                            CurrentSharedFiles.Add(file);
                            validFiles++;
                        }
                    }
                }
            }

            ParseFiles(CacheFiles, CurrentCacheFiles);
            ParseFiles(SharedFiles, CurrentSharedFiles);

            Console.WriteLine($"Successfully Verified {validFiles}/{files.Count()} Files\n");

            return true;
        }
    }
}
