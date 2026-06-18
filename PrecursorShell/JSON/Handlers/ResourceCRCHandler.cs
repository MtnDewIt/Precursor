using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Collections.Generic;
using TagTool.Cache;

namespace PrecursorShell.JSON.Handlers
{
    public class ResourceCRCHandler : JsonConverter<ResourceCRC>
    {
        public override void WriteJson(JsonWriter writer, ResourceCRC value, JsonSerializer serializer)
        {
            var signatureString = string.Empty;

            if (!Array.TrueForAll(value.Data, b => b == 0))
            {
                signatureString = value.GetCRC();
            }

            writer.WriteValue(signatureString);
        }

        public override ResourceCRC ReadJson(JsonReader reader, Type objectType, ResourceCRC existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var signatureString = reader.Value.ToString();

            var signature = new ResourceCRC();

            if (signatureString != string.Empty)
            {
                signature.SetCRC(signatureString);
            }

            return signature;
        }
    }
}
