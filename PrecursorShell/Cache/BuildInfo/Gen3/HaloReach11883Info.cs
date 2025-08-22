using PrecursorShell.Cache.BuildTable;
using PrecursorShell.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TagTool.Cache;
using TagTool.IO;

namespace PrecursorShell.Cache.BuildInfo.Gen3
{
    public class HaloReach11883Info : BuildTableEntry
    {
        public override CacheBuild Build => CacheBuild.HaloReach11883;
        public override CacheVersion Version => CacheVersion.HaloReach11883;
        public override CachePlatform Platform => CachePlatform.Original;
        public override CacheGeneration Generation => CacheGeneration.Gen3;

        public override string ResourcePath => null;

        public override List<string> BuildStrings => new List<string> 
        { 
            "0237d057-1e3c-4390-9cfc-6108a911de01" 
        };

        public override List<string> CacheFiles => new List<string> 
        {
            "blob_index.dat"
        };
        public override List<string> SharedFiles => null;
        public override List<string> ResourceFiles => null;

        public override bool VerifyBuildInfo(BuildTableConfig.BuildTableEntry build)
        {
            var files = Directory.EnumerateFiles(build.Path, "*.*", SearchOption.AllDirectories).Where(x => x.EndsWith(".dat") || x.StartsWith($@"{build.Path}\blobs")).ToArray();

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

            Console.WriteLine($"Successfully Verified {validCount}/{files.Length} Files\n");

            return true;
        }

        private async Task<(int ValidCount, List<string> Errors)> VerifyFilesAsync(string[] files)
        {
            var validCacheFiles = new ConcurrentBag<string>();
            var validResourceFiles = new ConcurrentBag<string>();
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

            return ProcessResult(results, validCacheFiles, validResourceFiles, errors);
        }

        private FileValidationResult ValidateFileAsync(string filePath) 
        {
            var fileInfo = new FileInfo(filePath);

            using (var stream = fileInfo.OpenRead())
            using (var reader = new EndianReader(stream)) 
            {
                if (CacheFiles.Contains(fileInfo.Name))
                {
                    var guid = new Guid(reader.ReadBytes(16));

                    if (BuildStrings.Contains(guid.ToString()))
                    {
                        return new FileValidationResult(true, filePath, FileType.Cache);
                    }
                    else
                    {
                        return new FileValidationResult(false, $"Invalid Build String: {fileInfo.Name} - {guid} != {BuildStrings.FirstOrDefault()}");
                    }
                }
                else
                {
                    if (fileInfo.Name.StartsWith("cache_") || fileInfo.Name.StartsWith("tags_") || string.IsNullOrEmpty(fileInfo.Extension))
                    {
                        return new FileValidationResult(true, filePath, FileType.Resource);
                    }
                    else
                    {
                        return new FileValidationResult(false, $"Invalid Blob File: {fileInfo.Name}");
                    }
                }
            }
        }

        private (int ValidCount, List<string> Errors) ProcessResult(FileValidationResult[] results, ConcurrentBag<string> validCacheFiles, ConcurrentBag<string> validResourceFiles, ConcurrentBag<string> errors)
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
                        case FileType.Resource:
                            validResourceFiles.Add(result.FilePath);
                            break;
                    }
                }
                else if (result.ErrorMessage != null)
                {
                    errors.Add(result.ErrorMessage);
                }
            }

            UpdateFileTable(validCacheFiles, validResourceFiles);

            return (validCount, errors.ToList());
        }

        private void UpdateFileTable(ConcurrentBag<string> validCacheFiles, ConcurrentBag<string> validResourceFiles)
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

            if (!validResourceFiles.IsEmpty)
            {
                lock (CurrentResourceFiles)
                {
                    foreach (var file in validResourceFiles)
                    {
                        CurrentResourceFiles.Add(file);
                    }
                }
            }
        }
    }
}
