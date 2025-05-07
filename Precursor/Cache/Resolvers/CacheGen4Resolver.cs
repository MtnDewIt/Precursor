using Precursor.Cache.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagTool.BlamFile;
using TagTool.IO;

namespace Precursor.Cache.Resolvers
{
    public class CacheGen4Resolver
    {
        public List<string> Halo4RetailFiles { get; set; }

        public CacheGen4Resolver()
        {
            Halo4RetailFiles = new List<string>();
        }

        public void VerifyBuild(CacheObject.CacheBuildObject build)
        {
            if (string.IsNullOrEmpty(build.Path))
            {
                Console.WriteLine($"> Cache Type: {build.Build} - Null or Empty Path Detected, Skipping Verification...");
                return;
            }
            else if (!Path.Exists(build.Path))
            {
                Console.WriteLine($"> Cache Type: {build.Build} - Unable to Locate Directory, Skipping Verification...");
                return;
            }
            else
            {
                var cacheFiles = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories).ToList();

                if (cacheFiles.Count == 0)
                {
                    Console.WriteLine($"> Cache Type: {build.Build} - No .Map Files Found in Directory, Skipping Verification...");
                    return;
                }

                var validFiles = 0;

                foreach (var cacheFile in cacheFiles)
                {
                    var fileInfo = new FileInfo(cacheFile);

                    using (var stream = fileInfo.OpenRead())
                    {
                        using (var reader = new EndianReader(stream))
                        {
                            var mapFile = new MapFile();

                            mapFile.Read(reader);

                            if (!mapFile.Header.IsValid())
                            {
                                Console.WriteLine($"> Cache Type: {build.Build} - Invalid Cache File");
                                continue;
                            }

                            switch (build.Build)
                            {
                                case CacheBuild.Halo4Retail:
                                    if (mapFile.Header.GetBuild() == "")
                                    {
                                        Halo4RetailFiles.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Cache Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                            }
                        }
                    }
                }

                Console.WriteLine($"> Cache Type: {build.Build} - Successfully Verified {validFiles}/{cacheFiles.Count} Files");
            }
        }
    }
}
