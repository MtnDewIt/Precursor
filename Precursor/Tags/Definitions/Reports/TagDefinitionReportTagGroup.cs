using Precursor.Reports;
using Precursor.Tags.Definitions.Reports.Handlers;
using System.Collections.Generic;
using System.IO;

namespace Precursor.Tags.Definitions.Reports
{
    public class TagDefinitionReportTagGroup
    {
        public string TagGroup;
        public string GroupName;
        public ReportErrorLevel ErrorLevel;
        public int TagErrorCount;
        public List<TagDefinitionReportTagInstance> Tags = [];

        public TagDefinitionReportTagGroup(string tagGroup, string groupName = null)
        {
            TagGroup = tagGroup;
            GroupName = groupName;
        }

        public static void GenerateReportTagGroup(TagDefinitionReportTagGroup reportTagGroup, string outputPath) 
        {
            var handler = new TagDefinitionReportTagGroupHandler();

            var outputObject = handler.Serialize(reportTagGroup);

            var fileInfo = new FileInfo(Path.Combine($"Reports\\TagDefinitions", outputPath));

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            File.WriteAllText(Path.Combine(Program.PrecursorDirectory, fileInfo.FullName), outputObject);
        }
    }
}
