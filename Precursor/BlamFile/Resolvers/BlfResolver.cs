using Precursor.Cache.BuildInfo;
using Precursor.Common;
using System;
using System.IO;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.IO;
using TagTool.JSON.Handlers;
using TagTool.JSON.Objects;

namespace Precursor.BlamFile.Resolvers
{
    public class BlfResolver
    {
        public static void ParseFiles(BuildInfoEntry buildInfo)
        {
            var path = buildInfo.GetResourcePath();
            var version = buildInfo.GetVersion();
            var platform = buildInfo.GetPlatform();
            var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);

            foreach (var filePath in files)
            {
                Console.WriteLine($"Parsing file \"{filePath}\"...");

                var input = new FileInfo(filePath);

                var tempPath = input.DirectoryName.Replace("Resources", "Temp");
                var fileName = Path.GetFileNameWithoutExtension(input.Name);
                var fileExtension = input.Extension.TrimStart('.');

                var blf = new Blf(CacheVersion.Halo3Retail, CachePlatform.Original);

                var output = new FileInfo(Path.Combine(tempPath, $"{fileName}.{fileExtension}"));

                try
                {
                    using (var inStream = input.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var reader = new EndianReader(inStream);

                        blf.Read(reader);
                    }
                }
                catch (Exception e)
                {
                    new PrecursorError($"Failed to parse file \"{filePath}\": {e.Message}");
                }

                ParseVersion(blf, version, platform, filePath);

                GenerateJSON(blf, version, platform, fileName, fileExtension, tempPath);

                try
                {
                    using (var outStream = output.Create())
                    {
                        var writer = new EndianWriter(outStream);

                        blf.Write(writer);
                    }
                }
                catch (Exception e)
                {
                    new PrecursorError($"Failed to write blf data \"{filePath}\": {e.Message}");
                }

                // 4: Diff Against Original File

                // I don't know :/
                // Binary Diff???
                // Dump the data from the saved file to JSON
                // Diff the JSON???
                // BOTH??????????
            }
        }

        public static void GenerateJSON(Blf blf, CacheVersion version, CachePlatform platform, string fileName, string fileExtension, string filePath)
        {
            var handler = new BlfObjectHandler(version, platform);

            var blfObject = new BlfObject()
            {
                FileName = fileName,
                FileType = fileExtension,
                Blf = blf,
            };

            var jsonData = handler.Serialize(blfObject);

            var fileInfo = new FileInfo(Path.Combine(filePath, $"{fileName}.json"));

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            File.WriteAllText(fileInfo.FullName, jsonData);
        }

        public static void ParseVersion(Blf blf, CacheVersion version, CachePlatform platform, string filePath)
        {
            if (blf.Version != version || blf.CachePlatform != platform)
            {
                // We force Halo3Retail:Original for map images as the format doesn't change from Halo3Retail to Halo4
                bool isMapImage = blf.ContentFlags.HasFlag(Blf.BlfFileContentFlags.MapImage);

                // We force Halo3Retail:Original for campaign files up to reach, as the format doesn't change from Halo3Retail to HaloReach
                bool isGen3Campaign = blf.ContentFlags.HasFlag(Blf.BlfFileContentFlags.Campaign) && CacheVersionDetection.IsBetween(version, CacheVersion.Halo3Retail, CacheVersion.HaloReach);

                // We force Halo4:Original for campaign files up to h2amp, as the format doesn't change from Halo4 to Halo2AMP
                bool isGen4Campaign = blf.ContentFlags.HasFlag(Blf.BlfFileContentFlags.Campaign) && CacheVersionDetection.IsBetween(version, CacheVersion.Halo4, CacheVersion.Halo2AMP);

                // We force Halo4:Original for map info files up to h2amp, as the format doesn't change from Halo4 to Halo2AMP
                bool isGen4MapInfo = blf.ContentFlags.HasFlag(Blf.BlfFileContentFlags.Scenario) && CacheVersionDetection.IsBetween(version, CacheVersion.Halo4, CacheVersion.Halo2AMP);

                // We force X:Original for all map info files as the format does not change from X:Original to X:MCC 
                bool isMCCMapInfo = blf.ContentFlags.HasFlag(Blf.BlfFileContentFlags.Scenario) && platform == CachePlatform.MCC;

                if (!isMapImage && !isGen3Campaign && !isGen4Campaign && !isMCCMapInfo && !isGen4MapInfo)
                {
                    new PrecursorError($"Version Mismatch \"{filePath}\": {blf.Version}:{blf.CachePlatform} != {version}:{platform}");
                }
            }
        }
    }
}
