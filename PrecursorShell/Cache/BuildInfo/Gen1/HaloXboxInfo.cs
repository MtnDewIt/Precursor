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
using TagTool.IO;

namespace PrecursorShell.Cache.BuildInfo.Gen1
{
    public class HaloXboxInfo : BuildTableEntry
    {
        public override CacheBuild Build => CacheBuild.HaloXbox;
        public override CacheVersion Version => CacheVersion.HaloXbox;
        public override CachePlatform Platform => CachePlatform.Original;
        public override CacheGeneration Generation => CacheGeneration.Gen1;

        public override string ResourcePath => @"Resources\Gen1\HaloXbox";

        public override List<string> BuildStrings => new List<string> 
        { 
            "01.10.12.2276" 
        };

        public override List<string> CacheFiles => null;
        public override List<string> SharedFiles => null;
        public override List<string> ResourceFiles => null;

        public override bool VerifyBuildInfo(BuildTableConfig.BuildTableEntry build)
        {
            var files = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories).ToArray();

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

            Console.WriteLine($"Successfully Verified {validCount}/{files.Length} Files\n");

            return true;
        }

        private async Task<(int ValidCount, List<string> Errors)> VerifyFilesAsync(string[] files)
        {
            var validCacheFiles = new ConcurrentBag<string>();
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

            return ProcessResult(results, validCacheFiles, errors);
        }

        private FileValidationResult ValidateFileAsync(string filePath) 
        {
            var fileInfo = new FileInfo(filePath);

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
                    return new FileValidationResult(false, $"Failed to parse file \"{fileInfo.Name}\": {ex.Message}");
                }

                if (!mapFile.Header.IsValid())
                {
                    return new FileValidationResult(false, $"Invalid Cache File: {fileInfo.Name}");
                }

                if (BuildStrings.Contains(mapFile.Header.GetBuild()))
                {
                    try
                    {
                        GenerateJSON(mapFile, fileInfo.Name, ResourcePath);
                    }
                    catch (Exception ex)
                    {
                        return new FileValidationResult(false, $"Failed to serialize JSON \"{fileInfo.Name}\": {ex.Message}");
                    }

                    return new FileValidationResult(true, filePath, FileType.Cache);
                }
                else
                {
                    return new FileValidationResult(false, $"Invalid Build String: {fileInfo.Name} - {mapFile.Header.GetBuild()} != {BuildStrings.FirstOrDefault()}");
                }
            }
        }

        private (int ValidCount, List<string> Errors) ProcessResult(FileValidationResult[] results, ConcurrentBag<string> validCacheFiles, ConcurrentBag<string> errors)
        {
            var validCount = 0;

            foreach (var result in results)
            {
                if (result.IsValid)
                {
                    validCount++;

                    switch (result.Type)
                    {
                        case FileType.Cache:
                            validCacheFiles.Add(result.FilePath);
                            break;
                    }
                }
                else if (result.ErrorMessage != null)
                {
                    errors.Add(result.ErrorMessage);
                }
            }

            UpdateFileTable(validCacheFiles);

            return (validCount, errors.ToList());
        }

        private void UpdateFileTable(ConcurrentBag<string> validCacheFiles)
        {
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
        }
    }
}
