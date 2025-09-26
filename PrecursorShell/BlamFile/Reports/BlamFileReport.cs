using Newtonsoft.Json;
using PrecursorShell.Cache;
using PrecursorShell.Reports;
using System.Collections.Generic;
using System.IO;

namespace PrecursorShell.BlamFile.Reports
{
    public class BlamFileReport
    {
        private List<BlamFileReportBuild> _builds { get; set; }

        public BlamFileReport()
        {
            _builds = [];
        }

        public class BlamFileReportBuild 
        {
            public CacheBuild Build;
            public ReportHelper.ReportErrorLevel ErrorLevel;
            public int FileErrorCount;
            public List<string> Files = [];

            public BlamFileReportBuild(CacheBuild build)
            {
                Build = build;
            }
        }

        public List<BlamFileReportBuild> Builds => _builds;

        public void AddEntry(BlamFileReportBuild build) => _builds.Add(build);

        public void RemoveEntry(BlamFileReportBuild build) => _builds.Remove(build);

        public void GenerateReport()
        {
            var fileInfo = new FileInfo($"{Program.PrecursorDirectory}\\Reports\\BlamFiles\\Reports.json");

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
