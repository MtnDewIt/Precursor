using Newtonsoft.Json;
using Precursor.JSON.Handlers;
using System.Collections.Generic;

namespace Precursor.Tags.Definitions.Reports.Handlers
{
    public class TagDefinitionReportTagGroupHandler
    {
        private static List<JsonConverter> Converters { get; set; }

        public TagDefinitionReportTagGroupHandler()
        {
            Converters = new List<JsonConverter>
            {
                new EnumHandler()
            };
        }

        public string Serialize(TagDefinitionReportTagGroup input)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(input, settings);
        }

        public TagDefinitionReportTagGroup Deserialize(string input)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            };

            var tagGroup = JsonConvert.DeserializeObject<TagDefinitionReportTagGroup>(input, settings);

            return tagGroup;
        }
    }
}
