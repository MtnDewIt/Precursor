using Precursor.Cache.Objects;
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

namespace Precursor.Cache.Resolvers
{
    public class CacheGenHaloOnlineResolver : CacheResolver
    {
        public List<string> HaloOnlineEDMapFiles { get; set; } 
        public List<string> HaloOnlineEDCacheFiles { get; set; }
        public List<string> HaloOnline106708MapFiles { get; set;}
        public List<string> HaloOnline106708CacheFiles { get; set; }
        public List<string> HaloOnline235640MapFiles { get; set;}
        public List<string> HaloOnline235640CacheFiles { get; set; }
        public List<string> HaloOnline301003MapFiles { get; set; }
        public List<string> HaloOnline301003CacheFiles { get; set; }
        public List<string> HaloOnline327043MapFiles { get; set; }
        public List<string> HaloOnline327043CacheFiles { get; set; }
        public List<string> HaloOnline372731MapFiles { get; set; }
        public List<string> HaloOnline372731CacheFiles { get; set; }
        public List<string> HaloOnline416097MapFiles { get; set; }
        public List<string> HaloOnline416097CacheFiles { get; set; }
        public List<string> HaloOnline430475MapFiles { get; set; }
        public List<string> HaloOnline430475CacheFiles { get; set; }
        public List<string> HaloOnline454665MapFiles { get; set; }
        public List<string> HaloOnline454665CacheFiles { get; set; }
        public List<string> HaloOnline449175MapFiles { get; set; }
        public List<string> HaloOnline449175CacheFiles { get; set; }
        public List<string> HaloOnline498295MapFiles { get; set; }
        public List<string> HaloOnline498295CacheFiles { get; set; }
        public List<string> HaloOnline530605MapFiles { get; set; }
        public List<string> HaloOnline530605CacheFiles { get; set; }
        public List<string> HaloOnline532911MapFiles { get; set; }
        public List<string> HaloOnline532911CacheFiles { get; set; }
        public List<string> HaloOnline554482MapFiles { get; set; }
        public List<string> HaloOnline554482CacheFiles { get; set; }
        public List<string> HaloOnline571627MapFiles { get; set; }
        public List<string> HaloOnline571627CacheFiles { get; set; }
        public List<string> HaloOnline604673MapFiles { get; set; }
        public List<string> HaloOnline604673CacheFiles { get; set; }
        public List<string> HaloOnline700123MapFiles { get; set; }
        public List<string> HaloOnline700123CacheFiles { get; set; }

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

        public static Dictionary<long, CacheBuild> HaloOnlineTimestampMapping = new Dictionary<long, CacheBuild>
        {
            [132699675831101597] = CacheBuild.HaloOnlineED,
            [130713360241169179] = CacheBuild.HaloOnline106708,
            [130772932862346058] = CacheBuild.HaloOnline235640,
            [130785901486445524] = CacheBuild.HaloOnline301003,
            [130800445160458507] = CacheBuild.HaloOnline327043,
            [130814318396118255] = CacheBuild.HaloOnline372731,
            [130829123589114103] = CacheBuild.HaloOnline416097,
            [130834294034159845] = CacheBuild.HaloOnline430475,
            [130844512316254660] = CacheBuild.HaloOnline454665,
            [130851642645809862] = CacheBuild.HaloOnline449175,
            [130858473716879375] = CacheBuild.HaloOnline498295,
            [130868891945946004] = CacheBuild.HaloOnline530605,
            [130869644198634503] = CacheBuild.HaloOnline532911,
            [130879952719550501] = CacheBuild.HaloOnline554482,
            [130881889330693956] = CacheBuild.HaloOnline571627,
            [130893802351772672] = CacheBuild.HaloOnline604673,
            [130930071628935939] = CacheBuild.HaloOnline700123
        };

        public CacheGenHaloOnlineResolver()
        {
            HaloOnlineEDMapFiles = new List<string>();
            HaloOnlineEDCacheFiles = new List<string>();
            HaloOnline106708MapFiles = new List<string>();
            HaloOnline106708CacheFiles = new List<string>();
            HaloOnline235640MapFiles = new List<string>();
            HaloOnline235640CacheFiles = new List<string>();
            HaloOnline301003MapFiles = new List<string>();
            HaloOnline301003CacheFiles = new List<string>();
            HaloOnline327043MapFiles = new List<string>();
            HaloOnline327043CacheFiles = new List<string>();
            HaloOnline372731MapFiles = new List<string>();
            HaloOnline372731CacheFiles = new List<string>();
            HaloOnline416097MapFiles = new List<string>();
            HaloOnline416097CacheFiles = new List<string>();
            HaloOnline430475MapFiles = new List<string>();
            HaloOnline430475CacheFiles = new List<string>();
            HaloOnline454665MapFiles = new List<string>();
            HaloOnline454665CacheFiles = new List<string>();
            HaloOnline449175MapFiles = new List<string>();
            HaloOnline449175CacheFiles = new List<string>();
            HaloOnline498295MapFiles = new List<string>();
            HaloOnline498295CacheFiles = new List<string>();
            HaloOnline530605MapFiles = new List<string>();
            HaloOnline530605CacheFiles = new List<string>();
            HaloOnline532911MapFiles = new List<string>();
            HaloOnline532911CacheFiles = new List<string>();
            HaloOnline554482MapFiles = new List<string>();
            HaloOnline554482CacheFiles = new List<string>();
            HaloOnline571627MapFiles = new List<string>();
            HaloOnline571627CacheFiles = new List<string>();
            HaloOnline604673MapFiles = new List<string>();
            HaloOnline604673CacheFiles = new List<string>();
            HaloOnline700123MapFiles = new List<string>();
            HaloOnline700123CacheFiles = new List<string>();
        }

        public override void VerifyBuild(CacheObject.CacheBuildObject build)
        {
            if (string.IsNullOrEmpty(build.Path) || !Path.Exists(build.Path))
            {
                Console.WriteLine($"> Build Type: {build.Build} - Invalid or Missing Path, Skipping Verification...");
                return;
            }

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
                                HaloOnlineEDMapFiles.Add(file);
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
                                HaloOnline106708MapFiles.Add(file);
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
                                HaloOnline235640MapFiles.Add(file);
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
                                HaloOnline301003MapFiles.Add(file);
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
                                HaloOnline327043MapFiles.Add(file);
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
                                HaloOnline372731MapFiles.Add(file);
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
                                HaloOnline416097MapFiles.Add(file);
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
                                HaloOnline430475MapFiles.Add(file);
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
                                HaloOnline454665MapFiles.Add(file);
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
                                HaloOnline449175MapFiles.Add(file);
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
                                HaloOnline498295MapFiles.Add(file);
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
                                HaloOnline530605MapFiles.Add(file);
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
                                HaloOnline532911MapFiles.Add(file);
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
                                HaloOnline554482MapFiles.Add(file);
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
                                HaloOnline571627MapFiles.Add(file);
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
                                HaloOnline604673MapFiles.Add(file);
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
                                HaloOnline700123MapFiles.Add(file);
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

                        validFiles++;
                    }
                    else if (Path.GetFileName(cacheFile) == "string_ids.dat") 
                    {
                        validFiles++;
                    }
                    else
                    {
                        var header = deserializer.Deserialize<ResourceCacheHaloOnlineHeader>(dataContext);

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
