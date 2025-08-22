using PrecursorShell.Cache.BuildInfo.Gen1;
using PrecursorShell.Cache.BuildInfo.Gen2;
using PrecursorShell.Cache.BuildInfo.Gen3;
using PrecursorShell.Cache.BuildInfo.Gen4;
using PrecursorShell.Cache.BuildInfo.GenHaloOnline;
using PrecursorShell.Cache.BuildInfo.GenMCC;
using PrecursorShell.Cache.BuildTable;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.JSON.Handlers;
using TagTool.JSON.Objects;

namespace PrecursorShell.Cache.BuildInfo
{
    public abstract class BuildTableEntry
    {
        public static readonly int MaxConcurrency = Environment.ProcessorCount * 2;

        public abstract CacheBuild Build { get; }
        public abstract CacheVersion Version { get; }
        public abstract CachePlatform Platform { get; }
        public abstract CacheGeneration Generation { get; }

        public abstract string ResourcePath { get; }

        public abstract IReadOnlyList<string> BuildStrings { get; }

        public abstract IReadOnlyList<string> CacheFiles { get; }
        public abstract IReadOnlyList<string> SharedFiles { get; }
        public abstract IReadOnlyList<string> ResourceFiles { get; }

        public HashSet<string> CurrentMapFiles { get; set; } = [];
        public HashSet<string> CurrentCacheFiles { get; set; } = [];
        public HashSet<string> CurrentSharedFiles { get; set; } = [];
        public HashSet<string> CurrentResourceFiles { get; set; } = [];

        public abstract bool VerifyBuildInfo(BuildTableConfig.BuildTableEntry build);

        public static void ParseFiles(IReadOnlyList<string> mask, HashSet<string> files)
        {
            var currentFiles = files.Select(Path.GetFileName);

            foreach (var file in mask.Where(file => !currentFiles.Contains(file)))
            {
                new PrecursorWarning($"Missing Shared File: {file}");
            }
        }

        public static bool ParseFileCount(int count) 
        {
            if (count == 0) 
            {
                new PrecursorWarning("No Valid Files Found in Directory, Skipping Verification...\n");
                return false;
            }

            return true;
        }

        public void GenerateJSON(MapFile mapFile, string fileName, string tempPath) 
        {
            var path = ResourcePath.Replace("Resources", "Temp");
            var mapName = Path.GetFileNameWithoutExtension(fileName);

            var mapObject = new MapObject()
            {
                MapName = mapName,
                MapVersion = mapFile.Version,
                Header = mapFile.Header,
                MapFileBlf = mapFile.MapFileBlf,
                Reports = mapFile.Reports,
            };

            var handler = new MapObjectHandler(Version, Platform);

            var jsonData = handler.Serialize(mapObject);

            var fileInfo = new FileInfo(Path.Combine($"{path}", "cache_files", $"{fileName}.json"));

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            File.WriteAllText(fileInfo.FullName, jsonData);
        }

        public static CacheResource GetResourceType(ReadOnlySpan<char> fileName)
        {
            return fileName switch
            {
                "tags.dat" => CacheResource.Tags,
                "string_ids.dat" => CacheResource.StringIds,
                "audio.dat" => CacheResource.Audio,
                "lightmaps.dat" => CacheResource.Lightmaps,
                "render_models.dat" => CacheResource.RenderModels,
                "resources.dat" => CacheResource.Resources,
                "resources_b.dat" => CacheResource.ResourcesB,
                "textures.dat" => CacheResource.Textures,
                "textures_b.dat" => CacheResource.TexturesB,
                "video.dat" => CacheResource.Video,
                _ => CacheResource.None,
            };
        }

        // TODO: MAKE NOT ASS
        // TODO: MOVE SOMEWHERE ELSE
        public static BuildTableEntry GetBuildEntry(BuildTableConfig.BuildTableEntry build)
        {
            if (string.IsNullOrEmpty(build.Path) || !Path.Exists(build.Path))
            {
                new PrecursorWarning("Invalid or Missing Path, Skipping Verification...\n");
                return null;
            }

            BuildTableEntry buildInfo = null;

            // TODO: Maybe rework the base BuildInfoEntry class so this isn't necessary
            switch (build.Build)
            {
                case CacheBuild.HaloXbox:
                    buildInfo = new HaloXboxInfo();
                    break;
                case CacheBuild.HaloPC:
                    buildInfo = new HaloPCInfo();
                    break;
                case CacheBuild.HaloCustomEdition:
                    buildInfo = new HaloCustomEditionInfo();
                    break;
                case CacheBuild.Halo2Alpha:
                    buildInfo = new Halo2AlphaInfo();
                    break;
                case CacheBuild.Halo2Beta:
                    buildInfo = new Halo2BetaInfo();
                    break;
                case CacheBuild.Halo2Xbox:
                    buildInfo = new Halo2XboxInfo();
                    break;
                case CacheBuild.Halo2Vista:
                    buildInfo = new Halo2VistaInfo();
                    break;
                case CacheBuild.Halo3Beta:
                    buildInfo = new Halo3BetaInfo();
                    break;
                case CacheBuild.Halo3Retail:
                    buildInfo = new Halo3RetailInfo();
                    break;
                case CacheBuild.Halo3MythicRetail:
                    buildInfo = new Halo3MythicRetailInfo();
                    break;
                case CacheBuild.Halo3ODST:
                    buildInfo = new Halo3ODSTInfo();
                    break;
                case CacheBuild.HaloReach:
                    buildInfo = new HaloReachInfo();
                    break;
                case CacheBuild.HaloReach11883:
                    buildInfo = new HaloReach11883Info();
                    break;
                case CacheBuild.Halo4Retail:
                    buildInfo = new Halo4RetailInfo();
                    break;
                case CacheBuild.HaloOnlineED:
                    buildInfo = new HaloOnlineEDInfo();
                    break;
                case CacheBuild.HaloOnline106708:
                    buildInfo = new HaloOnline106708Info();
                    break;
                case CacheBuild.HaloOnline235640:
                    buildInfo = new HaloOnline235640Info();
                    break;
                case CacheBuild.HaloOnline301003:
                    buildInfo = new HaloOnline301003Info();
                    break;
                case CacheBuild.HaloOnline327043:
                    buildInfo = new HaloOnline327043Info();
                    break;
                case CacheBuild.HaloOnline372731:
                    buildInfo = new HaloOnline372731Info();
                    break;
                case CacheBuild.HaloOnline416097:
                    buildInfo = new HaloOnline416097Info();
                    break;
                case CacheBuild.HaloOnline430475:
                    buildInfo = new HaloOnline430475Info();
                    break;
                case CacheBuild.HaloOnline454665:
                    buildInfo = new HaloOnline454665Info();
                    break;
                case CacheBuild.HaloOnline449175:
                    buildInfo = new HaloOnline449175Info();
                    break;
                case CacheBuild.HaloOnline498295:
                    buildInfo = new HaloOnline498295Info();
                    break;
                case CacheBuild.HaloOnline530605:
                    buildInfo = new HaloOnline530605Info();
                    break;
                case CacheBuild.HaloOnline532911:
                    buildInfo = new HaloOnline532911Info();
                    break;
                case CacheBuild.HaloOnline554482:
                    buildInfo = new HaloOnline554482Info();
                    break;
                case CacheBuild.HaloOnline571627:
                    buildInfo = new HaloOnline571627Info();
                    break;
                case CacheBuild.HaloOnline604673:
                    buildInfo = new HaloOnline604673Info();
                    break;
                case CacheBuild.HaloOnline700123:
                    buildInfo = new HaloOnline700123Info();
                    break;
                case CacheBuild.Halo1MCC:
                    buildInfo = new Halo1MCCInfo();
                    break;
                case CacheBuild.Halo2MCC:
                    buildInfo = new Halo2MCCInfo();
                    break;
                case CacheBuild.Halo3MCC:
                    buildInfo = new Halo3MCCInfo();
                    break;
                case CacheBuild.Halo3ODSTMCC:
                    buildInfo = new Halo3ODSTMCCInfo();
                    break;
                case CacheBuild.HaloReachMCC:
                    buildInfo = new HaloReachMCCInfo();
                    break;
                case CacheBuild.Halo4MCC:
                    buildInfo = new Halo4MCCInfo();
                    break;
                case CacheBuild.Halo2AMPMCC:
                    buildInfo = new Halo2AMPMCCInfo();
                    break;
            }

            if (buildInfo != null)
            {
                if (buildInfo.VerifyBuildInfo(build))
                {
                    return buildInfo;
                }
            }

            return null;
        }

        public readonly struct FileValidationResult
        {
            public readonly bool IsValid;
            public readonly string FilePath;
            public readonly string ErrorMessage;
            public readonly FileType Type;

            public FileValidationResult(bool isValid, string errorMessage = null) 
            {
                IsValid = isValid;
                ErrorMessage = errorMessage;
            }

            public FileValidationResult(bool isValid, string filePath, FileType type, string errorMessage = null)
            {
                IsValid = isValid;
                FilePath = filePath;
                Type = type;
                ErrorMessage = errorMessage;
            }
        }

        public enum FileType
        {
            None,
            Map,
            Cache,
            Shared,
            Resource,
        }
    }
}
