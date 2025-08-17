using Precursor.Serialization;
using System.Collections.Generic;
using System.IO;
using TagTool.Cache;

namespace Precursor.Tags.Definitions
{
    public class TagDefinitionValidator
    {
        private readonly GameCache Cache;
        private readonly Stream Stream;
        public List<string> Problems = new List<string>();

        public TagDefinitionValidator(GameCache cache, Stream stream)
        {
            Cache = cache;
            Stream = stream;
        }

        public void VerifyTag(CachedTag tag)
        {
            var deserializer = new Deserializer(Cache.Version, Cache.Platform);

            object data = deserializer.DeserializeTagInstance(Cache, Stream, tag);

            Problems.AddRange(deserializer.Problems);
        }
    }
}
