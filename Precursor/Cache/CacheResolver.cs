using Newtonsoft.Json;
using Precursor.Cache.Handlers;
using Precursor.Cache.Objects;
using System;
using System.IO;

namespace Precursor.Cache
{
    public class CacheResolver
    {
        public static void GenerateCacheData(string outputPath)
        {
            var cacheObject = new CacheObject();

            foreach (CacheType cacheType in Enum.GetValues(typeof(CacheType)))
            {
                if (cacheType == CacheType.None)
                    continue;

                cacheObject.Caches.Add(new CacheObject.CacheTypeObject(cacheType, null));
            }

            var handler = new CacheObjectHandler();

            var outputObject = handler.Serialize(cacheObject);

            File.WriteAllText(outputPath, outputObject);
        }

        public static void ValidateCacheData(string inputPath)
        {
            if (!File.Exists(inputPath))
            {
                Console.WriteLine("Unable to locate Precursor.json\n");
                return;
            }

            var jsonData = File.ReadAllText(inputPath);

            var handler = new CacheObjectHandler();

            var cacheObject = handler.Deserialize(jsonData);

            foreach (var cache in cacheObject.Caches)
            {
                if (string.IsNullOrEmpty(cache.Path))
                {
                    Console.WriteLine($"> Cache Type: {cache.CacheType.ToString()} - Null or Empty Path Detected, Skipping Validation.");
                    continue;
                }
                else if (!Path.Exists(cache.Path))
                {
                    Console.WriteLine($"> Cache Type: {cache.CacheType.ToString()} - Unable to locate directory, Skipping Validation.");
                    continue;
                }
                else
                {
                    // TODO: Validate the cache itself

                    // Loop through each .map file in the specified cache directory
                    // Read the cache file header
                    // Check if the build string matches the associated cache type
                    // Build a file database for that specific cache type
                }
            }
        }
    }
}
