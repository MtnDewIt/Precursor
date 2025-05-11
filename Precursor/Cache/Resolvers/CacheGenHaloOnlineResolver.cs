using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagTool.BlamFile;
using TagTool.Cache.HaloOnline;
using TagTool.Cache;
using TagTool.IO;
using TagTool.Serialization;
using Precursor.Cache.BuildTable;

namespace Precursor.Cache.Resolvers
{
    public class CacheGenHaloOnlineResolver : CacheResolver
    {
        public List<string> CacheFiles = new List<string> 
        {
            "audio.dat",
            "lightmaps.dat",
            "render_models.dat",
            "resources.dat",
            "resources_b.dat",
            "string_ids.dat",
            "tags.dat",
            "textures.dat",
            "textures_b.dat",
            "video.dat"
        };

        public override void VerifyBuild(BuildTableProperties.BuildTableEntry build)
        {
            var mapFiles = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories).ToList();
            var cacheFiles = Directory.EnumerateFiles(build.Path, "*.dat", SearchOption.AllDirectories).ToList();

            if (cacheFiles.Count == 0)
            {
                Console.WriteLine($"> Build Type: {build.Build} - No .Map Files Found in Directory, Skipping Verification...");
                return;
            }

            var validFiles = 0;

            foreach (var file in mapFiles)
            {
                var fileInfo = new FileInfo(file);

                using (var stream = fileInfo.OpenRead())
                using (var reader = new EndianReader(stream))
                {
                    var mapFile = new MapFile();

                    mapFile.Read(reader);

                    if (!mapFile.Header.IsValid())
                    {
                        Console.WriteLine($"> Build Type: {build.Build} - Invalid Map File");
                        continue;
                    }

                    switch (build.Build)
                    {
                        case CacheBuild.HaloOnlineED:
                            if (mapFile.Header.GetBuild() == "eldewrito")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline106708:
                            if (mapFile.Header.GetBuild() == "1.106708 cert_ms23")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline235640:
                            if (mapFile.Header.GetBuild() == "1.235640 cert_ms25")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline301003:
                            if (mapFile.Header.GetBuild() == "Jun 12 2015 13:02:50")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline327043:
                            if (mapFile.Header.GetBuild() == "0.4.1.327043 cert_MS26_new")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline372731:
                            if (mapFile.Header.GetBuild() == "8.1.372731 Live")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline416097:
                            if (mapFile.Header.GetBuild() == "0.0.416097 Live")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline430475:
                            if (mapFile.Header.GetBuild() == "10.1.430475 Live")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline454665:
                            if (mapFile.Header.GetBuild() == "10.1.454665 Live")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline449175:
                            if (mapFile.Header.GetBuild() == "10.1.449175 Live")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline498295:
                            if (mapFile.Header.GetBuild() == "11.1.498295 Live")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline530605:
                            if (mapFile.Header.GetBuild() == "11.1.530605 Live")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline532911:
                            if (mapFile.Header.GetBuild() == "11.1.532911 Live")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline554482:
                            if (mapFile.Header.GetBuild() == "11.1.554482 Live")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline571627:
                            if (mapFile.Header.GetBuild() == "11.1.571627 Live")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline604673:
                            if (mapFile.Header.GetBuild() == "11.1.601838 Live")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                        case CacheBuild.HaloOnline700123:
                            if (mapFile.Header.GetBuild() == "12.1.700123 cert_ms30_oct19")
                            {
                                validFiles++;
                            }
                            else
                            {
                                Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(file)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                continue;
                            }
                            break;
                    }
                }
            }

            foreach (var cacheFile in cacheFiles) 
            {
                var fileInfo = new FileInfo(cacheFile);

                using (var stream = fileInfo.OpenRead())
                using (var reader = new EndianReader(stream)) 
                {
                    if (!CacheFiles.Contains(Path.GetFileName(cacheFile))) 
                    {
                        Console.WriteLine($"> Build Type: {build.Build} - Invalid Cache File");
                        continue;
                    }

                    reader.SeekTo(0);
                    var dataContext = new DataSerializationContext(reader);
                    var deserializer = new TagDeserializer(CacheVersion.Unknown, CachePlatform.All);

                    if (Path.GetFileName(cacheFile) == "tags.dat")
                    {
                        var header = deserializer.Deserialize<TagCacheHaloOnlineHeader>(dataContext);

                        // TODO: Validate Tag Cache

                        validFiles++;
                    }
                    else if (Path.GetFileName(cacheFile) == "string_ids.dat") 
                    {
                        validFiles++;
                    }
                    else
                    {
                        var header = deserializer.Deserialize<ResourceCacheHaloOnlineHeader>(dataContext);

                        // TODO: Validate Resource Cache

                        validFiles++;
                    }
                }
            }

            Console.WriteLine($"Successfully Verified {validFiles}/{mapFiles.Count + cacheFiles.Count} Files\n");
        }

        public override bool IsValidCacheFile()
        {
            return false;
        }
    }
}
