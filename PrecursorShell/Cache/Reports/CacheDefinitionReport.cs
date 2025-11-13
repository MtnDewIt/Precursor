using Newtonsoft.Json;
using PrecursorShell.Reports;
using System.Collections.Generic;
using System.IO;

namespace PrecursorShell.Cache.Reports
{
    public class CacheDefinitionReport
    {
        private List<CacheDefinitionReportBuild> _builds { get; set; }

        public CacheDefinitionReport()
        {
            _builds = [];
        }

        public class CacheDefinitionReportBuild
        {
            public CacheBuild Build;
            public ReportHelper.ReportErrorLevel ErrorLevel;
            public int FileErrorCount;
            public List<string> Files = [];

            public CacheDefinitionReportBuild(CacheBuild build)
            {
                Build = build;
            }
        }

        public List<CacheDefinitionReportBuild> Builds => _builds;

        public void AddEntry(CacheDefinitionReportBuild build) => _builds.Add(build);

        public void RemoveEntry(CacheDefinitionReportBuild build) => _builds.Remove(build);

        public void GenerateReport()
        {
            var fileInfo = new FileInfo($"{DirectoryPaths.Base}\\Reports\\CacheDefinitions\\Reports.json");

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

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

                writer.WritePropertyName("ErrorLevel");
                writer.WriteValue(build.ErrorLevel.ToString());

                writer.WritePropertyName("FileErrorCount");
                writer.WriteValue(build.FileErrorCount);

                writer.WritePropertyName("Files");
                writer.WriteStartArray();

                foreach (var file in build.Files)
                {
                    writer.WriteValue(file);
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
