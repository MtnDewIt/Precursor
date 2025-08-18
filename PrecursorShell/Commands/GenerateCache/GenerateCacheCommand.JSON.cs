using TagTool.JSON.Parsers;

namespace PrecursorShell.Commands.GenerateCache
{
    partial class GenerateCacheCommand : PrecursorCommand
    {
        private TagObjectParser TagParser;
        private BlfObjectParser BlfParser;

        public void ParseJSONData(string sourcePath, string scenarioPath)
        {
            TagParser = new TagObjectParser(Cache, CacheContext, CacheStream, sourcePath);
            BlfParser = new BlfObjectParser(Cache.Version, Cache.Platform, sourcePath, Cache.Directory.FullName);

            TagParser.ParseFile($@"global_tags.cache_file_global_tags");
            TagParser.ParseFile($@"{scenarioPath}.scenario");

            BlfParser.ParseFile($@"data\levels\halo3");
        }
    }
}
