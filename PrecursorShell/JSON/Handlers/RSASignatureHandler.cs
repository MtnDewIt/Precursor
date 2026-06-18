using Newtonsoft.Json;
using System;
using TagTool.Cache;

namespace PrecursorShell.JSON.Handlers
{
    public class RSASignatureHandler : JsonConverter<RSASignature>
    {
        public override void WriteJson(JsonWriter writer, RSASignature value, JsonSerializer serializer) 
        {
            var signatureString = string.Empty;

            if (!value.IsInvalid()) 
            {
                signatureString = value.ToString();
            }

            writer.WriteValue(signatureString);
        }

        public override RSASignature ReadJson(JsonReader reader, Type objectType, RSASignature existingValue, bool hasExistingValue, JsonSerializer serializer) 
        {
            var signatureString = reader.Value.ToString();

            var signature = new RSASignature();

            if (signatureString != string.Empty) 
            {
                signature.SetSignature(signatureString);
            }

            return signature;
        }
    }
}
