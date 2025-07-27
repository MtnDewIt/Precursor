using Precursor.Cache;
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

        public List<ReportBuild> GetBuildTable() => Builds;

        public void AddEntry(ReportBuild build) => Builds.Add(build);

        public void RemoveEntry(ReportBuild build) => Builds.Remove(build);

        public class ReportBuild 
        {
            public CacheBuild Build;
            public List<ReportCacheFile> Files = [];
        }

        public class ReportCacheFile
        {
            public string Name;
            public List<ReportTagGroup> Groups = [];
        }

        public class ReportTagGroup
        {
            public string Signature;
            public string Name;
            public List<ReportTagInstance> Tags = [];
        }

        public class ReportTagInstance
        {
            public string Name;
            public List<string> Errors = [];
        }
    }
}
