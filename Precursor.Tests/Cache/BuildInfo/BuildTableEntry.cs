using Precursor.Tests.Cache.BuildInfo.Gen1;
using Precursor.Tests.Cache.BuildInfo.Gen2;
using Precursor.Tests.Cache.BuildInfo.Gen3;
using Precursor.Tests.Cache.BuildInfo.Gen4;
using Precursor.Tests.Cache.BuildInfo.HaloOnline;
using Precursor.Tests.Cache.BuildInfo.MCC;
using Precursor.Tests.Cache.BuildTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagTool.Cache;

namespace Precursor.Tests.Cache.BuildInfo
{
    public abstract class BuildTableEntry
    {
        public static readonly int MaxConcurrency = Environment.ProcessorCount * 2;

        public abstract CacheBuild Build { get; }
        public abstract CacheVersion Version { get; }
        public abstract CachePlatform Platform { get; }
        public abstract CacheGeneration Generation { get; }

        public abstract IReadOnlyList<string> BuildStrings { get; }

        public abstract IReadOnlyList<string> CacheFiles { get; }
        public abstract IReadOnlyList<string> SharedFiles { get; }
        public abstract IReadOnlyList<string> ResourceFiles { get; }

        public HashSet<string> CurrentMapFiles { get; set; } = [];
        public HashSet<string> CurrentCacheFiles { get; set; } = [];
        public HashSet<string> CurrentSharedFiles { get; set; } = [];
        public HashSet<string> CurrentResourceFiles { get; set; } = [];

        public abstract bool VerifyBuildInfo(BuildTableConfig.BuildTableEntry build);

        public abstract Task<(int ValidCount, List<string> Errors)> VerifyFilesAsync(string[] files);

        public static IEnumerable<string> ParseFiles(IReadOnlyList<string> mask, HashSet<string> files)
        {
            var currentFiles = files.Select(Path.GetFileName);

            if (mask != null) 
            {
                foreach (var file in mask.Where(file => !currentFiles.Contains(file)))
                {
                    yield return $"Missing Shared File: \"{file}\"";
                }
            }
        }

        public static bool ParseFileCount(int count) 
        {
            if (count == 0) 
            {
                Console.WriteLine("[ERROR]: No Valid Files Found in Directory, Skipping Verification...\n");
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

        public static BuildTableEntry GetBuildEntry(BuildTableConfig.BuildTableEntry build)
        {
            if (string.IsNullOrEmpty(build.Path) || !Path.Exists(build.Path))
            {
                Console.WriteLine("Invalid or Missing Path, Skipping Verification...\n");
                return null;
            }

            BuildTableEntry buildInfo = null;

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
                case CacheBuild.Halo3PreAlpha:
                    buildInfo = new Halo3PreAlphaInfo();
                    break;
                case CacheBuild.Halo3Alpha:
                    buildInfo = new Halo3AlphaInfo();
                    break;
                case CacheBuild.Halo3Beta:
                    buildInfo = new Halo3BetaInfo();
                    break;
                case CacheBuild.Halo3March7Delta:
                    buildInfo = new Halo3March7DeltaInfo();
                    break;
                case CacheBuild.Halo3March8Delta:
                    buildInfo = new Halo3March8DeltaInfo();
                    break;
                case CacheBuild.Halo3March9Delta:
                    buildInfo = new Halo3March9DeltaInfo();
                    break;
                case CacheBuild.Halo3Epsilon:
                    buildInfo = new Halo3EpsilonInfo();
                    break;
                case CacheBuild.Halo3DLC:
                    buildInfo = new Halo3DLCInfo();
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
                case CacheBuild.HaloReachAlpha:
                    buildInfo = new HaloReachAlphaInfo();
                    break;
                case CacheBuild.HaloReachPreBeta:
                    buildInfo = new HaloReachPreBetaInfo();
                    break;
                case CacheBuild.HaloReachBeta:
                    buildInfo = new HaloReachBetaInfo();
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
                case CacheBuild.HaloOnlineED:
                    buildInfo = new HaloOnlineEDInfo();
                    break;
                case CacheBuild.HaloOnline106708:
                    buildInfo = new HaloOnline106708Info();
                    break;
                case CacheBuild.HaloOnline155080:
                    buildInfo = new HaloOnline155080Info();
                    break;
                case CacheBuild.HaloOnline171227:
                    buildInfo = new HaloOnline171227Info();
                    break;
                case CacheBuild.HaloOnline177150:
                    buildInfo = new HaloOnline177150Info();
                    break;
                case CacheBuild.HaloOnline235640:
                    buildInfo = new HaloOnline235640Info();
                    break;
                case CacheBuild.HaloOnline301003:
                    buildInfo = new HaloOnline301003Info();
                    break;
                case CacheBuild.HaloOnline332089:
                    buildInfo = new HaloOnline332089Info();
                    break;
                case CacheBuild.HaloOnline373869:
                    buildInfo = new HaloOnline373869Info();
                    break;
                case CacheBuild.HaloOnline416138:
                    buildInfo = new HaloOnline416138Info();
                    break;
                case CacheBuild.HaloOnline430653:
                    buildInfo = new HaloOnline430653Info();
                    break;
                case CacheBuild.HaloOnline454665:
                    buildInfo = new HaloOnline454665Info();
                    break;
                case CacheBuild.HaloOnline479394:
                    buildInfo = new HaloOnline479394Info();
                    break;
                case CacheBuild.HaloOnline498295:
                    buildInfo = new HaloOnline498295Info();
                    break;
                case CacheBuild.HaloOnline530945:
                    buildInfo = new HaloOnline530945Info();
                    break;
                case CacheBuild.HaloOnline533032:
                    buildInfo = new HaloOnline533032Info();
                    break;
                case CacheBuild.HaloOnline554482:
                    buildInfo = new HaloOnline554482Info();
                    break;
                case CacheBuild.HaloOnline571698:
                    buildInfo = new HaloOnline571698Info();
                    break;
                case CacheBuild.HaloOnline604673:
                    buildInfo = new HaloOnline604673Info();
                    break;
                case CacheBuild.HaloOnline700255:
                    buildInfo = new HaloOnline700255Info();
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

            return buildInfo;
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
