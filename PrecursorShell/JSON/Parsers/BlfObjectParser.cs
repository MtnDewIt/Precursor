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
            var jsonData = File.ReadAllText($@"{InputPath}\{filePath}.json");
            var blfObject = Handler.Deserialize(jsonData);

            var blfFile = new FileInfo($@"{OutputPath}\{blfObject.FileName}.{blfObject.FileType}");

            using (var stream = blfFile.Create())
            using (var writer = new EndianWriter(stream))
            {
                blfObject.Blf.Write(writer);
            }
        }
    }
}
