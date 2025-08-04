using Newtonsoft.Json;
using Precursor.Cache;
using Precursor.Cache.BuildTable;
using Precursor.JSON.Handlers;
using Precursor.Tags.Definitions.Reports;
using System.Collections.Generic;

namespace Precursor.Tags.Definitions.Reports.Handlers
{
    public class TagDefinitionReportPropertiesHandler
    {
        private static List<JsonConverter> Converters { get; set; }

        public TagDefinitionReportPropertiesHandler()
        {
            Converters = new List<JsonConverter>
            {
                new EnumHandler()
            };
        }

        public string Serialize(TagDefinitionReportProperties input)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(input, settings);
        }

        public TagDefinitionReportProperties Deserialize(string input)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            };

            var properties = JsonConvert.DeserializeObject<TagDefinitionReportProperties>(input, settings);

            return properties;
        }
    }
}
