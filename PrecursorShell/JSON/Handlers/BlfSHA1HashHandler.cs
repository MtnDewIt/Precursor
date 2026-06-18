using Newtonsoft.Json;
using System;
using System.Globalization;
using TagTool.BlamFile.Chunks;

namespace PrecursorShell.JSON.Handlers
{
    public class BlfSHA1HashHandler : JsonConverter<BlfEndOfFileSHA1.BlfSHA1Hash>
    {
        public override void WriteJson(JsonWriter writer, BlfEndOfFileSHA1.BlfSHA1Hash value, JsonSerializer serializer)
        {
            var hashString = string.Empty;

            if (!Array.TrueForAll(value.Hash, b => b == 0))
            {
                hashString = Convert.ToHexString(value.Hash);
            }

            writer.WriteValue(hashString);
        }

        public override BlfEndOfFileSHA1.BlfSHA1Hash ReadJson(JsonReader reader, Type objectType, BlfEndOfFileSHA1.BlfSHA1Hash existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var hashString = reader.Value.ToString();

            byte[] result = new byte[256];

            if (hashString != string.Empty)
            {
                var chunkSize = 2;

                for (int i = 0; i < 256; i++)
                {
                    int start = i * chunkSize;
                    int length = Math.Min(chunkSize, hashString.Length - start);
                    result[i] = byte.Parse(hashString.Substring(start, length), NumberStyles.HexNumber);
                }
            }

            return new BlfEndOfFileSHA1.BlfSHA1Hash
            {
                Hash = result,
            };
        }
    }
}
