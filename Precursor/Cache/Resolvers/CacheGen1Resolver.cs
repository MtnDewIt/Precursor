using Assimp;
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
    public class CacheGen1Resolver
    {
        public List<string> HaloXboxFiles { get; set; }
        public List<string> HaloPCFiles { get; set; }
        public List<string> HaloCustomEditionFiles { get; set; }

        public static List<string> ResourceFiles = new List<string>
        {
            "bitmaps.map",
            "loc.map",
            "sounds.map"
        };

        public CacheGen1Resolver()
        {
            HaloXboxFiles = new List<string>();
            HaloPCFiles = new List<string>();
            HaloCustomEditionFiles = new List<string>();
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

                var cacheFileCount = 0;
                var validFiles = 0;

                foreach (var cacheFile in cacheFiles)
                {
                    if (!ResourceFiles.Contains(Path.GetFileName(cacheFile))) 
                    {
                        var file = new FileInfo(cacheFile);

                        using (var stream = file.OpenRead())
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
                                case CacheBuild.HaloXbox:
                                    if (mapFile.Header.GetBuild() == "01.10.12.2276")
                                    {
                                        HaloXboxFiles.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Cache Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloPC:
                                    if (mapFile.Header.GetBuild() == "01.00.00.0564")
                                    {
                                        HaloPCFiles.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Cache Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloCustomEdition:
                                    if (mapFile.Header.GetBuild() == "01.00.00.0609")
                                    {
                                        HaloCustomEditionFiles.Add(cacheFile);
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

                        cacheFileCount++;
                    }
                }

                Console.WriteLine($"> Cache Type: {build.Build} - Successfully Verified {validFiles}/{cacheFileCount} Files");
            }
        }
    }
}
