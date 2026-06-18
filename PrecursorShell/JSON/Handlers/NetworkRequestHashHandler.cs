using Newtonsoft.Json;
using System;
using TagTool.Cache;

namespace PrecursorShell.JSON.Handlers
{
    public class NetworkRequestHashHandler : JsonConverter<NetworkRequestHash>
    {
        public override void WriteJson(JsonWriter writer, NetworkRequestHash value, JsonSerializer serializer)
        {
            var hashString = string.Empty;

            if (!value.IsInvalid()) 
            {
                hashString = value.ToString();
            }

            writer.WriteValue(hashString);
        }

        public override NetworkRequestHash ReadJson(JsonReader reader, Type objectType, NetworkRequestHash existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var hashString = reader.Value.ToString();

            var networkRequestHash = new NetworkRequestHash();

            if (hashString != string.Empty) 
            {
                networkRequestHash.SetHash(hashString);
            }

            return networkRequestHash;
        }
    }
}
