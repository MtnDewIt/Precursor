using PrecursorShell.Cache.BuildTable;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public override List<string> CacheFiles => null;
        public override List<string> SharedFiles => null;
        public override List<string> ResourceFiles => null;

        public override List<string> CurrentMapFiles => null;
        public override List<string> CurrentCacheFiles => new List<string>();
        public override List<string> CurrentSharedFiles => null;
        public override List<string> CurrentResourceFiles => new List<string>();

        public override bool VerifyBuildInfo(BuildTableConfig.BuildTableEntry build)
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
                        new PrecursorWarning($"Invalid Build String: {fileInfo.Name} - {guid.ToString()} != {BuildStrings.FirstOrDefault()}");
                        continue;
                    }
                }
            }

            foreach (var blob in blobs) 
            {
                var fileInfo = new FileInfo(blob);

                if (fileInfo.Name.StartsWith("cache_") || fileInfo.Name.StartsWith("tags_") || string.IsNullOrEmpty(fileInfo.Extension)) 
                {
                    CurrentResourceFiles.Add(blob);
                    validFiles++;
                }
                else
                {
                    new PrecursorWarning($"Invalid Blob File: {fileInfo.Name}");
                    continue;
                }
            }

            Console.WriteLine($"Successfully Verified {validFiles}/{totalFileCount} Files\n");

            return true;
        }
    }
}
