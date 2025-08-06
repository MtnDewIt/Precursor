using Newtonsoft.Json;
using Precursor.JSON.Handlers;
using System.Collections.Generic;

namespace Precursor.Tags.Definitions.Reports.Handlers
{
    public class TagDefinitionReportCacheFileHandler
    {
        private static List<JsonConverter> Converters { get; set; }

        public TagDefinitionReportCacheFileHandler()
        {
            Converters = new List<JsonConverter>
            {
                new EnumHandler()
            };
        }

        public string Serialize(TagDefinitionReportCacheFile input)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(input, settings);
        }

        public TagDefinitionReportCacheFile Deserialize(string input)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            };

            var cacheFile = JsonConvert.DeserializeObject<TagDefinitionReportCacheFile>(input, settings);

            return cacheFile;
        }
    }
}
