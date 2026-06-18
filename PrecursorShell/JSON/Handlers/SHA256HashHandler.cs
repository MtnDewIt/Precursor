using Newtonsoft.Json;
using System;
using TagTool.Cache;

namespace PrecursorShell.JSON.Handlers
{
    public class SHA256HashHandler : JsonConverter<SHA256Hash>
    {
        public override void WriteJson(JsonWriter writer, SHA256Hash value, JsonSerializer serializer)
        {
            var signatureString = string.Empty;

            if (!value.IsInvalid())
            {
                signatureString = value.ToString();
            }

            writer.WriteValue(signatureString);
        }

        public override SHA256Hash ReadJson(JsonReader reader, Type objectType, SHA256Hash existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var signatureString = reader.Value.ToString();

            var signature = new SHA256Hash();

            if (signatureString != string.Empty)
            {
                signature.SetHash(signatureString);
            }

            return signature;
        }
    }
}
