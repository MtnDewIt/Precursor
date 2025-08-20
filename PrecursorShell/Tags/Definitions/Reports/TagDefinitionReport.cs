using Newtonsoft.Json;
using PrecursorShell.Cache;
using PrecursorShell.Reports;
using System.Collections.Generic;
using System.IO;

namespace PrecursorShell.Tags.Definitions.Reports
{
    public class TagDefinitionReport
    {
        private List<TagDefinitionReportBuild> _builds { get; set; }  

        public TagDefinitionReport()
        {
            _builds = [];
        }

        public class TagDefinitionReportBuild
        {
            public CacheBuild Build;
            public ReportErrorLevel ErrorLevel;
            public int FileErrorCount;
            public List<string> Files = [];

            public TagDefinitionReportBuild(CacheBuild build)
            {
                Build = build;
            }
        }

        public List<TagDefinitionReportBuild> Builds => _builds;

        public void AddEntry(TagDefinitionReportBuild build) => _builds.Add(build);

        public void RemoveEntry(TagDefinitionReportBuild build) => _builds.Remove(build);

        public void GenerateReport()
        {
            var fileInfo = new FileInfo($"{Program.PrecursorDirectory}\\Reports\\TagDefinitions\\Reports.json");

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
