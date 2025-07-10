using Precursor.Cache.BuildInfo;
using Precursor.Common;
using System;
using System.IO;
using System.Linq;
using TagTool.Bitmaps;
using TagTool.Cache;
using TagTool.Tags.Definitions;

namespace Precursor.Bitmaps.Resolvers
{
    public class BitmapResolver
    {
        public static void ParseFiles(BuildInfoEntry buildInfo) 
        {
            var files = buildInfo.GetCacheFiles();

            foreach (var file in files) 
            {
                var fileInfo = new FileInfo(file);

                // TODO: Add versioning
                GameCache cache = null;

                try 
                {
                    cache = GameCache.Open(fileInfo);
                }
                catch (Exception ex) 
                {
                    new PrecursorWarning($"Failed to open cache file \"{fileInfo.Name}\": {ex.Message}");
                    continue;
                }

                using (var stream = cache.OpenCacheRead()) 
                {
                    // The idea here is to loop through all bitmap formats,
                    // then on each format, loop through each bitmap in the cache,
                    // collating each bitmap into groups, based on the current format.

                    // That way, we can then perform conversion testing on each format in each map.
                    // This should make data validation slightly easier.

                    foreach (var tag in cache.TagCache.FindAllInGroup("bitm"))
                    {
                        var bitm = cache.Deserialize<Bitmap>(stream, tag);

                        if (!bitm.Images.Any(x => x.Format != bitm.Images.FirstOrDefault().Format)) 
                        {
                            new PrecursorWarning($"Bitmap image sequence contains two or more formats");
                        }

                        // TODO: Ensure that this works :/
                        // Get the most common format in the bitmap image sequence
                        // This ensures that even if an image sequence contains multiple formats
                        // we are using the most common format in the sequence.
                        var format = bitm.Images.GroupBy(x => x.Format).MaxBy(y => y.Count()).Key;

                        // TODO: Add proper format testing
                        Console.WriteLine($"{format} - {tag.Name}.bitmap");
                    }
                }
            }
        }
    }
}
