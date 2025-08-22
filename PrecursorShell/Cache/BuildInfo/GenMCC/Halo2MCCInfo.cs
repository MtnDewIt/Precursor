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

namespace PrecursorShell.Cache.BuildInfo.GenMCC
{
    public class Halo2MCCInfo : BuildTableEntry
    {
        public override CacheBuild Build => CacheBuild.Halo2MCC;
        public override CacheVersion Version => CacheVersion.Halo2PC;
        public override CachePlatform Platform => CachePlatform.MCC;
        public override CacheGeneration Generation => CacheGeneration.GenMCC;

        public override string ResourcePath => @"Resources\GenMCC\Halo2MCC";

        public override List<string> BuildStrings => new List<string>
        {
            // No build string, srsly 343?
            ""
        };

        public override List<string> CacheFiles => null;
        public override List<string> SharedFiles => new List<string>
        {
            "shared.map",
            "single_player_shared.map"
        };
        public override List<string> ResourceFiles => new List<string> 
        {
            "sounds_cht.dat",
            "sounds_de.dat",
            "sounds_en.dat",
            "sounds_fr.dat",
            "sounds_it.dat",
            "sounds_jpn.dat",
            "sounds_kor.dat",
            "sounds_neutral.dat",
            "sounds_remastered.dat",
            "sounds_sp.dat",
            "textures.dat"
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

            ParseFiles(SharedFiles, CurrentSharedFiles);
            ParseFiles(ResourceFiles, CurrentResourceFiles);

            Console.WriteLine($"Successfully Verified {validCount}/{files.Length} Files\n");

            return true;
        }

        private async Task<(int ValidCount, List<string> Errors)> VerifyFilesAsync(string[] files)
        {
            var validCacheFiles = new ConcurrentBag<string>();
            var validSharedFiles = new ConcurrentBag<string>();
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

            return ProcessResult(results, validCacheFiles, validSharedFiles, validResourceFiles, errors);
        }

        private FileValidationResult ValidateFileAsync(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var fileName = fileInfo.Name;

            using (var stream = fileInfo.OpenRead())
            using (var reader = new EndianReader(stream))
            {
                if (!SharedFiles.Contains(fileInfo.Name) && !ResourceFiles.Contains(fileInfo.Name)) 
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

                if (ResourceFiles.Contains(fileInfo.Name))
                {
                    return new FileValidationResult(true, filePath, FileType.Resource);
                }

                if (SharedFiles.Contains(fileInfo.Name))
                {
                    return new FileValidationResult(true, filePath, FileType.Shared);
                }
            }

            return new FileValidationResult(false);
        }

        private (int ValidCount, List<string> Errors) ProcessResult(FileValidationResult[] results, ConcurrentBag<string> validCacheFiles, ConcurrentBag<string> validSharedFiles, ConcurrentBag<string> validResourceFiles, ConcurrentBag<string> errors)
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
                        case FileType.Shared:
                            validSharedFiles.Add(result.FilePath);
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

            UpdateFileTable(validCacheFiles, validSharedFiles, validResourceFiles);

            return (validCount, errors.ToList());
        }

        private void UpdateFileTable(ConcurrentBag<string> validCacheFiles, ConcurrentBag<string> validSharedFiles, ConcurrentBag<string> validResourceFiles)
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
