using PrecursorShell.Cache.BuildInfo.Gen1;
using PrecursorShell.Cache.BuildInfo.Gen2;
using PrecursorShell.Cache.BuildInfo.Gen3;
using PrecursorShell.Cache.BuildInfo.Gen4;
using PrecursorShell.Cache.BuildInfo.Eldorado;
using PrecursorShell.Cache.BuildInfo.MCC;
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
                case CacheBuild.Halo4220811:
                    buildInfo = new Halo4220811Info();
                    break;
                case CacheBuild.Halo4280911:
                    buildInfo = new Halo4280911Info();
                    break;
                case CacheBuild.Halo4E3:
                    buildInfo = new Halo4E3Info();
                    break;
                case CacheBuild.Halo4Retail:
                    buildInfo = new Halo4RetailInfo();
                    break;
                case CacheBuild.Halo4140113:
                    buildInfo = new Halo4140113Info();
                    break;
                case CacheBuild.Halo4131113:
                    buildInfo = new Halo4131113Info();
                    break;
                case CacheBuild.EldoradoED:
                    buildInfo = new EldoradoEDInfo();
                    break;
                case CacheBuild.Eldorado106708:
                    buildInfo = new Eldorado106708Info();
                    break;
                case CacheBuild.Eldorado155080:
                    buildInfo = new Eldorado155080Info();
                    break;
                case CacheBuild.Eldorado171227:
                    buildInfo = new Eldorado171227Info();
                    break;
                case CacheBuild.Eldorado177150:
                    buildInfo = new Eldorado177150Info();
                    break;
                case CacheBuild.Eldorado235640:
                    buildInfo = new Eldorado235640Info();
                    break;
                case CacheBuild.Eldorado301003:
                    buildInfo = new Eldorado301003Info();
                    break;
                case CacheBuild.Eldorado332089:
                    buildInfo = new Eldorado332089Info();
                    break;
                case CacheBuild.Eldorado373869:
                    buildInfo = new Eldorado373869Info();
                    break;
                case CacheBuild.Eldorado416138:
                    buildInfo = new Eldorado416138Info();
                    break;
                case CacheBuild.Eldorado430653:
                    buildInfo = new Eldorado430653Info();
                    break;
                case CacheBuild.Eldorado454665:
                    buildInfo = new Eldorado454665Info();
                    break;
                case CacheBuild.Eldorado479394:
                    buildInfo = new Eldorado479394Info();
                    break;
                case CacheBuild.Eldorado498295:
                    buildInfo = new Eldorado498295Info();
                    break;
                case CacheBuild.Eldorado530945:
                    buildInfo = new Eldorado530945Info();
                    break;
                case CacheBuild.Eldorado533032:
                    buildInfo = new Eldorado533032Info();
                    break;
                case CacheBuild.Eldorado554482:
                    buildInfo = new Eldorado554482Info();
                    break;
                case CacheBuild.Eldorado571698:
                    buildInfo = new Eldorado571698Info();
                    break;
                case CacheBuild.Eldorado604673:
                    buildInfo = new Eldorado604673Info();
                    break;
                case CacheBuild.Eldorado700255:
                    buildInfo = new Eldorado700255Info();
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
