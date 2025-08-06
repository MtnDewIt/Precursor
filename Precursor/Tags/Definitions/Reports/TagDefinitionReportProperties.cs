using Precursor.Cache;
using Precursor.Reports;
using Precursor.Tags.Definitions.Reports.Handlers;
using System.Collections.Generic;
using System.IO;

namespace Precursor.Tags.Definitions.Reports
{
    public class TagDefinitionReportProperties
    {
        public List<ReportBuildEntry> Builds { get; set; }

        public TagDefinitionReportProperties()
        {
            Builds = [];
        }

        public class ReportBuildEntry
        {
            public CacheBuild Build;
            public ReportErrorLevel ErrorLevel;
            public int FileErrorCount;
            public List<string> Files = [];

            public ReportBuildEntry(CacheBuild build, ReportErrorLevel errorLevel, int errorCount, List<string> files)
            {
                Build = build;
                ErrorLevel = errorLevel;
                FileErrorCount = errorCount;
                Files = files;
            }
        }

        public static void GenerateProperties() 
        {
            var properties = new TagDefinitionReportProperties();

            foreach (var build in Program.TagDefinitionReport.GetBuildTable())
            {
                properties.Builds.Add(new ReportBuildEntry(build.Build, build.ErrorLevel, build.FileErrorCount, build.Files));
            }

            var handler = new TagDefinitionReportPropertiesHandler();

            var outputObject = handler.Serialize(properties);

            var fileInfo = new FileInfo(Path.Combine(Program.PrecursorDirectory, "Reports", "TagDefinitions", "Reports.json"));

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            File.WriteAllText(fileInfo.FullName, outputObject);
        }
    }
}
