using Precursor.Cache;
using Precursor.Cache.BuildTable;
using Precursor.Tags.Definitions.Reports.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public List<string> Files = [];

            public ReportBuildEntry(CacheBuild build, List<string> files)
            {
                Build = build;
                Files = files;
            }
        }

        public static void GenerateProperties() 
        {
            var properties = new TagDefinitionReportProperties();

            foreach (var build in Program.TagDefinitionReport.GetBuildTable())
            {
                properties.Builds.Add(new ReportBuildEntry(build.Build, build.Files));
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
