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
    public class HaloOnlineEDInfo : BuildTableEntry
    {
        public override CacheBuild Build => CacheBuild.HaloOnlineED;
        public override CacheVersion Version => CacheVersion.HaloOnlineED;
        public override CachePlatform Platform => CachePlatform.Original;
        public override CacheGeneration Generation => CacheGeneration.GenHaloOnline;

        public override string ResourcePath => @"Resources\GenHaloOnline\HaloOnlineED07";

        public override List<string> BuildStrings => new List<string>
        {
            "eldewrito"
        };

        public override List<string> CacheFiles => new List<string>
        {
            "tags.dat",
        };
        public override List<string> SharedFiles => new List<string>
        {
            "audio.dat",
            "resources.dat",
            "resources_b.dat",
            "string_ids.dat",
            "textures.dat",
            "textures_b.dat",
        };
        public override List<string> ResourceFiles => null;

        public static readonly Dictionary<CacheResource, string> BuildDateTable = new Dictionary<CacheResource, string>
        {
            { CacheResource.Tags, "2021-07-05 14:06:23.1101597" },
            { CacheResource.Audio, "2021-07-05 14:06:23.1101597" },
            { CacheResource.Resources, "2021-07-05 14:06:23.1101597" },
            { CacheResource.ResourcesB, "2021-07-05 14:06:23.1101597" },
            { CacheResource.Textures, "2021-07-05 14:06:23.1101597" },
            { CacheResource.TexturesB, "2021-07-05 14:06:23.1101597" },
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
