using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace PrecursorShell.Cache.BuildTable
{
    public class BuildTableConfig
    {
        public List<BuildTableEntry> Builds { get; set; }

        public BuildTableConfig()
        {
            Builds = [];
        }

        public class BuildTableEntry
        {
            public CacheBuild Build { get; set; }
            public string Path { get; set; }

            public BuildTableEntry(CacheBuild build, string path)
            {
                Build = build;
                Path = path;
            }
        }

        public static void GenerateEmptyConfig()
        {
            var filter = new List<CacheBuild>
            {
                CacheBuild.None,
                CacheBuild.Halo1MCC,
                CacheBuild.Halo2MCC,
                CacheBuild.Halo3MCC,
                CacheBuild.Halo3ODSTMCC,
                CacheBuild.HaloReachMCC,
                CacheBuild.Halo4MCC,
                CacheBuild.Halo2AMPMCC,
                CacheBuild.All,
            };

            var fileInfo = new FileInfo(Program.ConfigPath);

            using var sw = new StreamWriter(fileInfo.FullName);
            using var writer = new JsonTextWriter(sw)
            {
                Formatting = Formatting.Indented,
            };

            writer.WriteStartObject();
            writer.WritePropertyName("Builds");
            writer.WriteStartArray();

            foreach (CacheBuild build in Enum.GetValues(typeof(CacheBuild)))
            {
                if (filter.Contains(build))
                    continue;

                writer.WriteStartObject();
                writer.WritePropertyName("Build");
                writer.WriteValue(build.ToString());

                writer.WritePropertyName("Path");

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public void GenerateConfig() 
        {
            var fileInfo = new FileInfo(Program.ConfigPath);

            using var sw = new StreamWriter(fileInfo.FullName);
            using var writer = new JsonTextWriter(sw)
            {
                Formatting = Formatting.Indented,
            };

            writer.WriteStartObject();
            writer.WritePropertyName("Builds");
            writer.WriteStartArray();

            foreach (var build in Builds) 
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Build");
                writer.WriteValue(build.Build.ToString());

                writer.WritePropertyName("Path");
                writer.WriteValue(build.Path);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public static BuildTableConfig ParseConfig(string filePath = null)
        {
            var fileInfo = new FileInfo(filePath ?? Program.ConfigPath);

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

            using (var sr = new StreamReader(fileInfo.FullName))
            using (var reader = new JsonTextReader(sr))
            {
                var config = new BuildTableConfig();

                reader.Read();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndObject)
                        break;

                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString() == "Builds") 
                    {
                        reader.Read();

                        while (reader.Read()) 
                        {
                            if (reader.TokenType == JsonToken.EndArray)
                                break;

                            if (reader.TokenType == JsonToken.StartObject) 
                            {
                                CacheBuild build = CacheBuild.None;
                                string path = null;

                                while (reader.Read()) 
                                {
                                    if (reader.TokenType == JsonToken.EndObject)
                                        break;

                                    if (reader.TokenType == JsonToken.PropertyName)
                                    {
                                        string propertyName = reader.Value.ToString();

                                        reader.Read();

                                        switch (propertyName)
                                        {
                                            case "Build":
                                                build = Enum.Parse<CacheBuild>(reader.Value.ToString());
                                                break;
                                            case "Path":
                                                path = reader.Value?.ToString();
                                                break;
                                        }
                                    }
                                }

                                config.Builds.Add(new BuildTableEntry(build, path));
                            }
                        }
                    }
                }

                // TODO: Can definitely clean this up

                var mccBuild = config.Builds.Find(x => x.Build == CacheBuild.MCCRetail);

                if (mccBuild != null) 
                {
                    foreach (var mccCacheBuild in mccCacheBuilds)
                    {
                        var build = new BuildTableEntry(mccCacheBuild, null);

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

                        config.Builds.Add(build);
                    }

                    config.Builds.Remove(mccBuild);
                }

                return config;
            }
        }
    }
}
