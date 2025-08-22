using PrecursorShell.Cache.BuildTable;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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
            var files = Directory.EnumerateFiles(build.Path, "*.*", SearchOption.AllDirectories).Where(x => x.EndsWith(".dat") || x.StartsWith($@"{build.Path}\blobs"));

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
                    if (CacheFiles.Contains(fileInfo.Name))
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
                    else 
                    {
                        if (fileInfo.Name.StartsWith("cache_") || fileInfo.Name.StartsWith("tags_") || string.IsNullOrEmpty(fileInfo.Extension))
                        {
                            CurrentResourceFiles.Add(file);
                            validFiles++;
                        }
                        else
                        {
                            new PrecursorWarning($"Invalid Blob File: {fileInfo.Name}");
                            continue;
                        }
                    }
                }
            }

            Console.WriteLine($"Successfully Verified {validFiles}/{files.Count()} Files\n");

            return true;
        }
    }
}
