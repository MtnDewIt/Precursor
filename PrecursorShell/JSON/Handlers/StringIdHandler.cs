using Newtonsoft.Json;
using System;
using TagTool.Cache;
using TagTool.Common;

namespace PrecursorShell.JSON.Handlers
{
    public class StringIdHandler : JsonConverter<StringId>
    {
        private GameCache Cache;

        public StringIdHandler(GameCache cache)
        {
            Cache = cache;
        }

        public override void WriteJson(JsonWriter writer, StringId value, JsonSerializer serializer)
        {
            writer.WriteValue(Cache.StringTable.GetString(value));
        }

        public override StringId ReadJson(JsonReader reader, Type objectType, StringId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string stringId = reader.Value.ToString();

            return stringId == $@"invalid" ? StringId.Invalid : Cache.StringTable.GetOrAddString(stringId);
        }
    }
}