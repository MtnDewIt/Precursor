using Newtonsoft.Json;
using Precursor.Cache.Objects;
using System.Collections.Generic;

namespace Precursor.Cache.Handlers
{
    public class CacheObjectHandler
    {
        private static List<JsonConverter> Converters { get; set; }

        public CacheObjectHandler()
        {
            Converters = new List<JsonConverter>
            {
                new EnumHandler()
            };
        }

        public string Serialize(CacheObject input)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(input, settings);
        }

        public CacheObject Deserialize(string input)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            };

            return JsonConvert.DeserializeObject<CacheObject>(input, settings);
        }
    }
}
