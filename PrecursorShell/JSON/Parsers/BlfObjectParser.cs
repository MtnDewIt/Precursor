using Newtonsoft.Json;
using PrecursorShell.JSON.Handlers;
using System.IO;
using TagTool.Cache;
using TagTool.IO;

namespace PrecursorShell.JSON.Parsers
{
    public class BlfObjectParser
    {
        private BlfObjectHandler Handler;
        private string InputPath;
        private string OutputPath;

        public BlfObjectParser(CacheVersion version, CachePlatform platform, string inputPath, string outputPath = null)
        {
            Handler = new BlfObjectHandler(version, platform);
            InputPath = inputPath;
            OutputPath = outputPath;
        }

        public void ParseFile(string filePath)
        {
            using var fileStream = File.OpenRead($@"{InputPath}\{filePath}.json");
            using var streamReader = new StreamReader(fileStream);
            using var jsonReader = new JsonTextReader(streamReader);
            var blfObject = Handler.Deserialize(jsonReader);

            var blfFile = new FileInfo($@"{OutputPath}\{blfObject.FileName}.{blfObject.FileType}");

            using (var stream = blfFile.Create())
            using (var writer = new EndianWriter(stream))
            {
                blfObject.Blf.Write(writer);
            }
        }
    }
}
