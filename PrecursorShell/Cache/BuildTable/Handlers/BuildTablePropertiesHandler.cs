using Newtonsoft.Json;
using PrecursorShell.JSON.Handlers;
using System.Collections.Generic;

namespace PrecursorShell.Cache.BuildTable.Handlers
{
    public class BuildTablePropertiesHandler
    {
        private static List<JsonConverter> Converters { get; set; }

        public BuildTablePropertiesHandler()
        {
            Converters = new List<JsonConverter>
            {
                new EnumHandler()
            };
        }

        public string Serialize(BuildTableProperties input)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(input, settings);
        }

        public BuildTableProperties Deserialize(string input)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = Converters,
                Formatting = Formatting.Indented
            };

            var mccCacheBuilds = new List<CacheBuild>
            {
                CacheBuild.Halo1MCC,
                CacheBuild.Halo2MCC,
                CacheBuild.Halo3MCC,
                CacheBuild.Halo3ODSTMCC,
                CacheBuild.HaloReachMCC,
                CacheBuild.Halo4MCC,
                CacheBuild.Halo2AMPMCC
            };

            var properties = JsonConvert.DeserializeObject<BuildTableProperties>(input, settings);

            var mccBuild = properties.Builds.Find(x => x.Build == CacheBuild.MCCRetail);

            if (mccBuild != null) 
            {
                foreach (var mccCacheBuild in mccCacheBuilds) 
                {
                    var build = new BuildTableProperties.BuildTableEntry(mccCacheBuild, null);

                    if (mccBuild.Path != null) 
                    {
                        switch (mccCacheBuild) 
                        {
                            case CacheBuild.Halo1MCC:
                                build.Path = $@"{mccBuild.Path}\halo1\maps";
                                break;
                            case CacheBuild.Halo2MCC:
                                build.Path = $@"{mccBuild.Path}\halo2\h2_maps_win64_dx11";
                                break;
                            case CacheBuild.Halo3MCC:
                                build.Path = $@"{mccBuild.Path}\halo3\maps";
                                break;
                            case CacheBuild.Halo3ODSTMCC:
                                build.Path = $@"{mccBuild.Path}\halo3odst\maps";
                                break;
                            case CacheBuild.HaloReachMCC:
                                build.Path = $@"{mccBuild.Path}\haloreach\maps";
                                break;
                            case CacheBuild.Halo4MCC:
                                build.Path = $@"{mccBuild.Path}\halo4\maps";
                                break;
                            case CacheBuild.Halo2AMPMCC:
                                build.Path = $@"{mccBuild.Path}\groundhog\maps";
                                break;
                        }
                    }

                    properties.Builds.Add(build);
                }

                properties.Builds.Remove(mccBuild);
            }

            return properties;
        }
    }
}
