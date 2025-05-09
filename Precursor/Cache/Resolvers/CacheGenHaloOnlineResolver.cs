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
    public class CacheGenHaloOnlineResolver
    {
        public List<string> HaloOnlineEDFiles { get; set; } 
        public List<string> HaloOnline106708Files { get; set;}
        public List<string> HaloOnline235640Files { get; set;}
        public List<string> HaloOnline301003Files { get; set;}
        public List<string> HaloOnline327043Files { get; set;}
        public List<string> HaloOnline372731Files { get; set;}
        public List<string> HaloOnline416097Files { get; set;}
        public List<string> HaloOnline430475Files { get; set;}
        public List<string> HaloOnline454665Files { get; set;}
        public List<string> HaloOnline449175Files { get; set;}
        public List<string> HaloOnline498295Files { get; set;}
        public List<string> HaloOnline530605Files { get; set;}
        public List<string> HaloOnline532911Files { get; set;}
        public List<string> HaloOnline554482Files { get; set;}
        public List<string> HaloOnline571627Files { get; set;}
        public List<string> HaloOnline604673Files { get; set;}
        public List<string> HaloOnline700123Files { get; set;}

        public CacheGenHaloOnlineResolver()
        {
            HaloOnlineEDFiles = new List<string>();
            HaloOnline106708Files = new List<string>();
            HaloOnline235640Files = new List<string>();
            HaloOnline301003Files = new List<string>();
            HaloOnline327043Files = new List<string>();
            HaloOnline372731Files = new List<string>();
            HaloOnline416097Files = new List<string>();
            HaloOnline430475Files = new List<string>();
            HaloOnline454665Files = new List<string>();
            HaloOnline449175Files = new List<string>();
            HaloOnline498295Files = new List<string>();
            HaloOnline530605Files = new List<string>();
            HaloOnline532911Files = new List<string>();
            HaloOnline554482Files = new List<string>();
            HaloOnline571627Files = new List<string>();
            HaloOnline604673Files = new List<string>();
            HaloOnline700123Files = new List<string>();
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
                var cacheFiles = Directory.EnumerateFiles(build.Path, "*.map", SearchOption.AllDirectories).ToList();

                if (cacheFiles.Count == 0)
                {
                    Console.WriteLine($"> Build Type: {build.Build} - No .Map Files Found in Directory, Skipping Verification...");
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
                                Console.WriteLine($"> Build Type: {build.Build} - Invalid Cache File");
                                continue;
                            }

                            switch (build.Build)
                            {
                                case CacheBuild.HaloOnlineED:
                                    if (mapFile.Header.GetBuild() == "eldewrito")
                                    {
                                        HaloOnlineEDFiles.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline106708:
                                    if (mapFile.Header.GetBuild() == "1.106708 cert_ms23")
                                    {
                                        HaloOnline106708Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline235640:
                                    if (mapFile.Header.GetBuild() == "1.235640 cert_ms25")
                                    {
                                        HaloOnline235640Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline301003:
                                    if (mapFile.Header.GetBuild() == "Jun 12 2015 13:02:50")
                                    {
                                        HaloOnline301003Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline327043:
                                    if (mapFile.Header.GetBuild() == "0.4.1.327043 cert_MS26_new")
                                    {
                                        HaloOnline327043Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline372731:
                                    if (mapFile.Header.GetBuild() == "8.1.372731 Live")
                                    {
                                        HaloOnline372731Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline416097:
                                    if (mapFile.Header.GetBuild() == "0.0.416097 Live")
                                    {
                                        HaloOnline416097Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline430475:
                                    if (mapFile.Header.GetBuild() == "10.1.430475 Live")
                                    {
                                        HaloOnline430475Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline454665:
                                    if (mapFile.Header.GetBuild() == "10.1.454665 Live")
                                    {
                                        HaloOnline454665Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline449175:
                                    if (mapFile.Header.GetBuild() == "10.1.449175 Live")
                                    {
                                        HaloOnline449175Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline498295:
                                    if (mapFile.Header.GetBuild() == "11.1.498295 Live")
                                    {
                                        HaloOnline498295Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline530605:
                                    if (mapFile.Header.GetBuild() == "11.1.530605 Live")
                                    {
                                        HaloOnline530605Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline532911:
                                    if (mapFile.Header.GetBuild() == "11.1.532911 Live")
                                    {
                                        HaloOnline532911Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline554482:
                                    if (mapFile.Header.GetBuild() == "11.1.554482 Live")
                                    {
                                        HaloOnline554482Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline571627:
                                    if (mapFile.Header.GetBuild() == "11.1.571627 Live")
                                    {
                                        HaloOnline571627Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline604673:
                                    if (mapFile.Header.GetBuild() == "11.1.601838 Live")
                                    {
                                        HaloOnline604673Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                                case CacheBuild.HaloOnline700123:
                                    if (mapFile.Header.GetBuild() == "12.1.700123 cert_ms30_oct19")
                                    {
                                        HaloOnline700123Files.Add(cacheFile);
                                        validFiles++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"> Build Type: {build.Build} - \"{Path.GetFileName(cacheFile)}\" - Build String Does Not Match Specified Build - \"{mapFile.Header.GetBuild()}\"");
                                        continue;
                                    }
                                    break;
                            }
                        }
                    }
                }

                Console.WriteLine($"Successfully Verified {validFiles}/{cacheFiles.Count} Files\n");
            }
        }
    }
}
