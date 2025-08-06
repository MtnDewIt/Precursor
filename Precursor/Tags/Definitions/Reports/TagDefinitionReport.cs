using Precursor.Cache;
using Precursor.Reports;
using System.Collections.Generic;

namespace Precursor.Tags.Definitions.Reports
{
    public class TagDefinitionReport
    {
        private List<ReportBuild> Builds { get; set; }  

        public TagDefinitionReport()
        {
            Builds = [];
        }

        public class ReportBuild
        {
            public CacheBuild Build;
            public ReportErrorLevel ErrorLevel;
            public int FileErrorCount;
            public List<string> Files = [];

            public ReportBuild(CacheBuild build)
            {
                Build = build;
            }
        }

        public List<ReportBuild> GetBuildTable() => Builds;

        public void AddEntry(ReportBuild build) => Builds.Add(build);

        public void RemoveEntry(ReportBuild build) => Builds.Remove(build);
    }
}
