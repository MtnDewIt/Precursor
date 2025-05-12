using Precursor.Cache.BuildTable;
using Precursor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.IO;

namespace Precursor.Cache.BuildInfo.Gen3
{
    public class HaloReach11883Info : BuildInfoEntry
    {
        public static readonly CacheBuild Build = CacheBuild.HaloReach11883;

        public static readonly CacheGeneration Generation = CacheGeneration.Gen3;

        public static readonly List<string> BuildStrings = new List<string> 
        { 
            "0237d057-1e3c-4390-9cfc-6108a911de01" 
        };

        public List<string> CurrentCacheFiles;

        public HaloReach11883Info()
        {
            CurrentCacheFiles = new List<string>();
        }

        public override bool VerifyBuildInfo(BuildTableProperties.BuildTableEntry build)
        {
            var files = Directory.EnumerateFiles(build.Path, "blob_index.dat", SearchOption.AllDirectories).ToList();
            var blobs = Directory.EnumerateFiles($@"{build.Path}\blobs", "*.", SearchOption.AllDirectories).ToList();

            if (!ParseFileCount(files.Count))
            {
                return false;
            }

            var totalFileCount = files.Count + blobs.Count;
            var validFiles = 0;

            foreach (var file in files) 
            {
                var fileInfo = new FileInfo(file);

                using (var stream = fileInfo.OpenRead())
                using (var reader = new EndianReader(stream))
                {
                    var guid = new Guid(reader.ReadBytes(16));

                    if (BuildStrings.Contains(guid.ToString()))
                    {
                        CurrentCacheFiles.Add(file);
                        validFiles++;
                    }
                    else
                    {
                        new PrecursorWarning($"Invalid Build String: {Path.GetFileName(file)}");
                        continue;
                    }
                }
            }

            foreach (var blob in blobs) 
            {
                if (Path.GetFileName(blob).StartsWith("cache_") || Path.GetFileName(blob).StartsWith("tags_") || Path.GetExtension(blob) == null) 
                {
                    CurrentCacheFiles.Add(blob);
                    validFiles++;
                }
                else
                {
                    new PrecursorWarning($"Invalid Blob File: {Path.GetFileName(blob)}");
                    continue;
                }
            }

            Console.WriteLine($"Successfully Verified {validFiles}/{totalFileCount} Files\n");

            return true;
        }

        public override CacheBuild GetBuild() => Build;
        public override CacheGeneration GetGeneration() => Generation;

        public override List<string> GetBuildStrings() => BuildStrings;

        public override List<string> GetCacheFiles() => null;
        public override List<string> GetSharedFiles() => null;
        public override List<string> GetResourceFiles() => null;

        public override List<string> GetCurrentMapFiles() => null;
        public override List<string> GetCurrentCacheFiles() => CurrentCacheFiles;
        public override List<string> GetCurrentSharedFiles() => null;
        public override List<string> GetCurrentResourceFiles() => null;
    }
}
