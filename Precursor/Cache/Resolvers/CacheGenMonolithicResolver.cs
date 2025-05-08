using Precursor.Cache.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.IO;

namespace Precursor.Cache.Resolvers
{
    public class CacheGenMonolithicResolver
    {
        public List<string >HaloReach11883Files;
        public List<string >HaloReach11883BlobFiles;

        public CacheGenMonolithicResolver()
        {
            HaloReach11883Files = new List<string>();
            HaloReach11883BlobFiles = new List<string>();
        }

        public void VerifyBuild(CacheObject.CacheBuildObject build) 
        {
            if (string.IsNullOrEmpty(build.Path))
            {
                Console.WriteLine($"> Build Type: {build.Build} - Null or Empty Path Detected, Skipping Verification...");
                return;
            }
            else if (!Path.Exists(build.Path))
            {
                Console.WriteLine($"> Build Type: {build.Build} - Unable to Locate Directory, Skipping Verification...");
                return;
            }
            else 
            {
                var cacheFiles = Directory.EnumerateFiles(build.Path, "blob_index.dat", SearchOption.AllDirectories).ToList();
                var cacheFileBlobs = Directory.EnumerateFiles($@"{build.Path}\blobs", "*.", SearchOption.AllDirectories).ToList();

                if (cacheFiles.Count == 0)
                {
                    Console.WriteLine($"> Build Type: {build.Build} - No blob_index.dat Files Found in Directory, Skipping Verification...");
                    return;
                }

                var totalFileCount = cacheFiles.Count + cacheFileBlobs.Count;
                var validFiles = 0;

                foreach (var cacheFile in cacheFiles) 
                {
                    var fileInfo = new FileInfo(cacheFile);

                    using (var stream = fileInfo.OpenRead())
                    {
                        using (var reader = new EndianReader(stream))
                        {
                            var guid = new Guid(reader.ReadBytes(16));

                            if (guid.ToString() == "0237d057-1e3c-4390-9cfc-6108a911de01") 
                            {
                                HaloReach11883Files.Add(cacheFile);
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{guid}\"");
                                continue;
                            }
                        }
                    }
                }

                foreach (var blob in cacheFileBlobs) 
                {
                    if (Path.GetFileName(blob).StartsWith("cache_") || Path.GetFileName(blob).StartsWith("tags_") || Path.GetExtension(blob) == null)
                    {
                        HaloReach11883BlobFiles.Add(blob);
                        validFiles++;
                    }
                    else 
                    {
                        Console.WriteLine($"> Build Type: {build.Build} - Invalid Blob File \"{Path.GetFileName(blob)}\"");
                        continue;
                    }
                }

                Console.WriteLine($"> Build Type: {build.Build} - Successfully Verified {validFiles}/{totalFileCount} Files");
            }
        }
    }
}
