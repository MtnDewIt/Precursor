using PrecursorShell.Cache.BuildInfo;
using PrecursorShell.Common;
using System;
using System.IO;
using System.Linq;
using TagTool.Bitmaps;
using TagTool.Cache;
using TagTool.Tags.Definitions;

namespace PrecursorShell.Bitmaps.Resolvers
{
    public class BitmapResolver
    {
        public static void ParseFiles(BuildTableEntry buildInfo) 
        {
            var files = buildInfo.CurrentCacheFiles;

            var bitmapCount = 0;

            // The idea here is to loop through all bitmap formats,
            // then on each format, loop through each bitmap in the cache,
            // collating each bitmap into groups, based on the current format.

            // That way, we can then perform conversion testing on each format in each map.
            // This should make data validation slightly easier.

            foreach (BitmapFormat currentFormat in Enum.GetValues(typeof(BitmapFormat))) 
            {
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
                        foreach (var tag in cache.TagCache.FindAllInGroup("bitm"))
                        {
                            var bitm = cache.Deserialize<Bitmap>(stream, tag);

                            object bitmapResourceDefinition = null;

                            if (bitm.Images.Any(x => x.Format != bitm.Images.FirstOrDefault().Format))
                            {
                                new PrecursorWarning($"Bitmap image sequence contains two or more formats");
                            }

                            for (int i = 0; i < bitm.Images.Count; i++) 
                            {
                                if (bitm.Images[i].XboxFlags.HasFlag(BitmapFlagsXbox.Xbox360UseInterleavedTextures))
                                    bitmapResourceDefinition = cache.ResourceCache.GetBitmapTextureInterleavedInteropResource(bitm.InterleavedHardwareTextures[bitm.Images[i].InterleavedInterop]);
                                else
                                    bitmapResourceDefinition = cache.ResourceCache.GetBitmapTextureInteropResource(bitm.HardwareTextures[i]);
                            }

                            // Get the most common format in the bitmap image sequence
                            // This ensures that even if an image sequence contains multiple formats
                            // we are using the most common format in the sequence.
                            var format = bitm.Images.GroupBy(x => x.Format).MaxBy(y => y.Count()).Key;

                            if (format == currentFormat)
                            {
                                Console.WriteLine($"{format} - {tag.Name}.bitmap");
                                bitmapCount++;

                                if (bitmapResourceDefinition == null)
                                {
                                    new PrecursorWarning($"Invalid Or Missing Bitmap Resource: {tag.Name}.bitmap");
                                }
                            }
                        }
                    }
                }
            } 

            Console.WriteLine($"Total Bitmap Count {bitmapCount}");
        }
    }
}
