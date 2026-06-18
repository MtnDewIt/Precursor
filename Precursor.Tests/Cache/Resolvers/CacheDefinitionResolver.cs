using Precursor.Tests.Cache.BuildInfo;
using Precursor.Tests.Cache.Reports;
using Precursor.Tests.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagTool.Cache;

namespace Precursor.Tests.Cache.Resolvers
{
    public class CacheDefinitionResolver
    {
        private static readonly ParallelOptions Options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2
        };

        public static CacheDefinitionReport ParseDefinitionsAsync(BuildTableEntry buildInfo) 
        {
            if (buildInfo.Build == CacheBuild.HaloReach11883 ||
                buildInfo.Build == CacheBuild.Halo4220811 || 
                buildInfo.Build == CacheBuild.Halo4280911 ||
                buildInfo.Build == CacheBuild.Halo4140113 ||
                buildInfo.Build == CacheBuild.Halo4131113) 
                return new();

            var files = GetBuildFiles(buildInfo);
            var build = buildInfo.Build;

            var cacheReport = new CacheDefinitionReport();

            Parallel.ForEach(files, Options, file =>
            {
                var result = ProcessFileAsync(buildInfo, file);

                cacheReport.Files.Add(result);
            });

            return cacheReport;
        }

        private static CacheDefinitionFileReport ProcessFileAsync(BuildTableEntry buildInfo, string file)
        {
            var fileInfo = new FileInfo(file);

            var errors = ProcessCacheFileAsync(buildInfo, fileInfo);

            return new CacheDefinitionFileReport
            {
                FileName = fileInfo.Name,
                Errors = errors,
            };
        }

        private static List<string> ProcessCacheFileAsync(BuildTableEntry buildInfo, FileInfo fileInfo)
        {
            List<string> errors = [];

            Deserializer deserializer = new Deserializer(buildInfo.Version, buildInfo.Platform);
            Type headerType = CacheFileHeader.GetHeaderType(buildInfo.Version, buildInfo.Platform);

            using (var stream = fileInfo.OpenRead()) 
            {
                try 
                {
                    ProcessCacheFile(deserializer, stream, headerType);
                }
                catch (Exception ex) 
                {
                    errors.Add($"Failed to deserialize header \"{fileInfo.Name}\": {ex.Message}");
                }
            }

            errors.AddRange(deserializer.Problems);

            return errors; ;
        }

        private static void ProcessCacheFile(Deserializer deserializer, Stream stream, Type headerType) 
        {
            var header = deserializer.DeserializeStructure(stream, headerType);
            var blf = deserializer.DeserializeBlf(stream);
            var reports = deserializer.DeserializeCacheFileReports(stream, header as CacheFileHeader);
        }

        private static HashSet<string> GetBuildFiles(BuildTableEntry buildInfo)
        {
            return buildInfo.Build switch
            {
                CacheBuild.Halo2Alpha or
                CacheBuild.Halo2Beta or
                CacheBuild.Halo2Xbox or
                CacheBuild.Halo2Vista or
                CacheBuild.Halo3Beta or
                CacheBuild.Halo3Retail or
                CacheBuild.Halo3MythicRetail or
                CacheBuild.Halo3ODST or
                CacheBuild.HaloReach or
                CacheBuild.Halo4Retail or
                CacheBuild.Halo2MCC or
                CacheBuild.Halo3MCC or
                CacheBuild.Halo3ODSTMCC or
                CacheBuild.HaloReachMCC or
                CacheBuild.Halo4MCC or
                CacheBuild.Halo2AMPMCC => buildInfo.CurrentCacheFiles.Union(buildInfo.CurrentSharedFiles).ToHashSet(),

                CacheBuild.HaloOnlineED or
                CacheBuild.HaloOnline106708 or
                CacheBuild.HaloOnline155080 or
                CacheBuild.HaloOnline171227 or
                CacheBuild.HaloOnline177150 or
                CacheBuild.HaloOnline235640 or
                CacheBuild.HaloOnline301003 or
                CacheBuild.HaloOnline332089 or
                CacheBuild.HaloOnline373869 or
                CacheBuild.HaloOnline416138 or
                CacheBuild.HaloOnline430653 or
                CacheBuild.HaloOnline454665 or
                CacheBuild.HaloOnline479394 or
                CacheBuild.HaloOnline498295 or
                CacheBuild.HaloOnline530945 or
                CacheBuild.HaloOnline533032 or
                CacheBuild.HaloOnline554482 or
                CacheBuild.HaloOnline571698 or
                CacheBuild.HaloOnline604673 or
                CacheBuild.HaloOnline700255 => buildInfo.CurrentMapFiles,

                _ => buildInfo.CurrentCacheFiles,
            };
        }
    }
}
