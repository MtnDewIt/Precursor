using Newtonsoft.Json;
using TagTool.Cache;
using System;

namespace PrecursorShell.JSON.Handlers
{
    public class FileCreatorHandler : JsonConverter<FileCreator>
    {
        public override void WriteJson(JsonWriter writer, FileCreator value, JsonSerializer serializer)
        {
            var creatorString = string.Empty;

            if (!value.IsInvalid())
            {
                creatorString = value.ToString();
            }

            writer.WriteValue(creatorString);
        }

        public override FileCreator ReadJson(JsonReader reader, Type objectType, FileCreator existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var creatorData = Array.Empty<byte>();
            var creatorString = reader.Value.ToString();

            var fileCreator = new FileCreator()
            {
                Data = creatorData,
            };

            if (creatorString != string.Empty)
            {
                fileCreator.SetCreator(creatorString);
            }

            return fileCreator;
        }
    }
}
