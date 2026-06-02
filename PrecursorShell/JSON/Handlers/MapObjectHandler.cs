using Newtonsoft.Json;
using System.Collections.Generic;
using TagTool.Cache;
using PrecursorShell.JSON.Objects;

namespace PrecursorShell.JSON.Handlers
{
    public class MapObjectHandler
    {
        private CacheVersion Version { get; set; }
        private CachePlatform Platform { get; set; }

        private List<JsonConverter> Converters;

        public MapObjectHandler(CacheVersion version, CachePlatform platform)
        {
            Version = version;
            Platform = platform;

            Converters = new List<JsonConverter>
            {
                new TagStructureHandler(Version, Platform),

                // I really need to merge all these into a single handler which just takes a generic type as an input :/
                new AngleHandler(),
                new ArgbColorHandler(),
                new BoundsAngleHandler(),
                new BoundsByteHandler(),
                new BoundsFloatHandler(),
                new BoundsIntHandler(),
                new BoundsLongHandler(),
                new BoundsSByteHandler(),
                new BoundsShortHandler(),
                new BoundsUIntHandler(),
                new BoundsULongHandler(),
                new BoundsUShortHandler(),
                new CacheAddressHandler(),
                new DatumHandleHandler(),
                new EnumHandler(),
                new FileCreatorHandler(),
                new Int16Point2dHandler(),
                new LastModificationDateHandler(),
                new NetworkRequestHashHandler(),
                new PlatformSignedValueHandler(Platform),
                new PlatformUnsignedValueHandler(Platform),
                new RealArgbColorHandler(),
                new RealBoundingBoxHandler(),
                new RealEulerAngles2dHandler(),
                new RealEulerAngles3dHandler(),
                new RealMatrix4x3Handler(),
                new RealPlane2dHandler(),
                new RealPlane3dHandler(),
                new RealPoint2dHandler(),
                new RealPoint3dHandler(),
                new RealQuaternionHandler(),
                new RealRectangle3dHandler(),
                new RealRgbColorHandler(),
                new RealVector2dHandler(),
                new RealVector3dHandler(),
                new Rectangle2dHandler(),
                new ResourceCRCHandler(),
                new RSASignatureHandler(),
                new SHA256HashHandler(),
                new TagHandler(),
            };
        }

        public string Serialize(MapObject input)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(input, settings);
        }

        public MapObject Deserialize(JsonReader reader)
        {
            var serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            });

            return serializer.Deserialize<MapObject>(reader);
        }
    }
}
