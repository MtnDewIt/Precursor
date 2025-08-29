using PrecursorShell.Cache.BuildTable;
using PrecursorShell.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.Cache.HaloOnline;
using TagTool.IO;

namespace PrecursorShell.Cache.BuildInfo.GenHaloOnline
{
    public class HaloOnline177150Info : BuildTableEntry
    {
        public override CacheBuild Build => CacheBuild.HaloOnline177150;
        public override CacheVersion Version => CacheVersion.HaloOnline155080;
        public override CachePlatform Platform => CachePlatform.Original;
        public override CacheGeneration Generation => CacheGeneration.GenHaloOnline;

        public override string ResourcePath => @"Resources\GenHaloOnline\HaloOnline177150";

        public override List<string> BuildStrings => new List<string>
        {
            "1.155080 cert_ms23"
        };

        public override List<string> CacheFiles => new List<string>
        {
            "tags.dat",
        };
        public override List<string> SharedFiles => new List<string>
        {
            "audio.dat",
            "resources.dat",
            "string_ids.dat",
            "textures.dat",
            "textures_b.dat",
            "video.dat"
        };
        public override List<string> ResourceFiles => null;

        public static readonly Dictionary<CacheResource, string> BuildDateTable = new Dictionary<CacheResource, string>
        {
            { CacheResource.Tags, "2015-04-10 11:37:39.234805" },
            { CacheResource.Audio, "2015-04-10 11:37:39.397805" },
            { CacheResource.Resources, "2015-04-10 11:37:39.396805" },
            { CacheResource.Textures, "2015-04-10 11:37:39.396805" },
            { CacheResource.TexturesB, "2015-04-10 11:37:39.396805" },
            { CacheResource.Video, "2015-04-10 11:37:39.397805" },
        };

        public override bool VerifyBuildInfo(BuildTableConfig.BuildTableEntry build)
        {
            var files = Directory.EnumerateFiles(build.Path, "*.*", SearchOption.AllDirectories).Where(x => x.EndsWith(".map") || x.EndsWith(".dat")).ToArray();

            if (!ParseFileCount(files.Length))
            {
                return false;
            }

            var (validCount, errors) = Task.Run(async () => await VerifyFilesAsync(files)).GetAwaiter().GetResult();

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    new PrecursorWarning(error);
                }

                return false;
            }

            ParseFiles(CacheFiles, CurrentCacheFiles);
            ParseFiles(SharedFiles, CurrentSharedFiles);

            Console.WriteLine($"Successfully Verified {validCount}/{files.Length} Files\n");

            return true;
        }

        private async Task<(int ValidCount, List<string> Errors)> VerifyFilesAsync(string[] files)
        {
            var validMapFiles = new ConcurrentBag<string>();
            var validCacheFiles = new ConcurrentBag<string>();
            var validSharedFiles = new ConcurrentBag<string>();
            var errors = new ConcurrentBag<string>();

            using var semaphore = new SemaphoreSlim(MaxConcurrency);

            var tasks = files.Select(async file =>
            {
                await semaphore.WaitAsync().ConfigureAwait(false);

                try
                {
                    return ValidateFileAsync(file);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            return ProcessResult(results, validMapFiles, validCacheFiles, validSharedFiles, errors);
        }

        private FileValidationResult ValidateFileAsync(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var fileName = fileInfo.Name;

            using (var stream = fileInfo.OpenRead())
            using (var reader = new EndianReader(stream))
            {
                if (CacheFiles.Contains(fileName) || SharedFiles.Contains(fileName))
                {
                    return ValidateCacheFileAsync(reader, fileName, filePath);
                }
                else
                {
                    return ValidateMapFileAsync(reader, fileName, filePath);
                }
            }
        }

        private FileValidationResult ValidateMapFileAsync(EndianReader reader, string fileName, string filePath)
        {
            var mapFile = new MapFile();

            try
            {
                mapFile.Read(reader);
            }
            catch (Exception ex)
            {
                return new FileValidationResult(false, $"Failed to parse file \"{fileName}\": {ex.Message}");
            }

            if (!mapFile.Header.IsValid())
            {
                return new FileValidationResult(false, $"Invalid Map File: {fileName}");
            }

            var buildString = mapFile.Header.GetBuild();

            if (!BuildStrings.Contains(buildString))
            {
                return new FileValidationResult(false, $"Invalid Build String: {fileName} - {buildString} != {BuildStrings.FirstOrDefault()}");
            }

            try
            {
                GenerateJSON(mapFile, fileName, ResourcePath);
            }
            catch (Exception ex)
            {
                return new FileValidationResult(false, $"Failed to serialize JSON \"{fileName}\": {ex.Message}");
            }

            return new FileValidationResult(true, filePath, FileType.Map);
        }

        private FileValidationResult ValidateCacheFileAsync(EndianReader reader, string fileName, string filePath)
        {
            var resourceType = GetResourceType(fileName);

            if (resourceType == CacheResource.StringIds)
            {
                return new FileValidationResult(true, filePath, FileType.Shared);
            }

            if (resourceType == CacheResource.None || !BuildDateTable.TryGetValue(resourceType, out string buildDate))
            {
                return new FileValidationResult(false, $"Invalid File: {fileName} - Unsupported or invalid resource type");
            }

            CacheFileSectionHeader header;

            try
            {
                header = CacheFileSectionHeader.ReadHeader(reader, Version, Platform);
            }
            catch
            {
                return new FileValidationResult(false, $"Invalid File: {fileName} - Failed to deserialize file section header");
            }

            var timestamp = LastModificationDate.GetTimestamp(header.CreationDate);

            if (BuildDateTable.TryGetValue(resourceType, out var expectedTimestamp) && expectedTimestamp != timestamp)
            {
                return new FileValidationResult(false, $"Invalid Build Date: {fileName} - {timestamp} != {buildDate}");
            }

            var category = resourceType == CacheResource.Tags ? FileType.Cache : FileType.Shared;

            return new FileValidationResult(true, filePath, category);
        }

        private (int ValidCount, List<string> Errors) ProcessResult(FileValidationResult[] results, ConcurrentBag<string> validMapFiles, ConcurrentBag<string> validCacheFiles, ConcurrentBag<string> validSharedFiles, ConcurrentBag<string> errors)
        {
            var validCount = 0;

            foreach (var result in results)
            {
                if (result.IsValid)
                {
                    validCount++;

                    switch (result.Type)
                    {
                        case FileType.Map:
                            validMapFiles.Add(result.FilePath);
                            break;
                        case FileType.Cache:
                            validCacheFiles.Add(result.FilePath);
                            break;
                        case FileType.Shared:
                            validSharedFiles.Add(result.FilePath);
                            break;
                    }
                }
                else if (result.ErrorMessage != null)
                {
                    errors.Add(result.ErrorMessage);
                }
            }

            UpdateFileTable(validMapFiles, validCacheFiles, validSharedFiles);

            return (validCount, errors.ToList());
        }

        private void UpdateFileTable(ConcurrentBag<string> validMapFiles, ConcurrentBag<string> validCacheFiles, ConcurrentBag<string> validSharedFiles)
        {
            if (!validMapFiles.IsEmpty)
            {
                lock (CurrentMapFiles)
                {
                    foreach (var file in validMapFiles)
                    {
                        CurrentMapFiles.Add(file);
                    }
                }
            }

            if (!validCacheFiles.IsEmpty)
            {
                lock (CurrentCacheFiles)
                {
                    foreach (var file in validCacheFiles)
                    {
                        CurrentCacheFiles.Add(file);
                    }
                }
            }

            if (!validSharedFiles.IsEmpty)
            {
                lock (CurrentSharedFiles)
                {
                    foreach (var file in validSharedFiles)
                    {
                        CurrentSharedFiles.Add(file);
                    }
                }
            }
        }
    }
}
