using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precursor.Tags.Definitions.Reports
{
    public class TagDefinitionReportTagGroup
    {
        public string TagGroup;
        public string GroupName;
        public List<TagDefinitionReportTagInstance> Tags = [];

        public TagDefinitionReportTagGroup(string tagGroup, string groupName = null)
        {
            TagGroup = tagGroup;
            GroupName = groupName;
        }

        public static void GenerateReportTagGroup(TagDefinitionReportTagGroup reportTagGroup, string outputPath) 
        {
            var outputObject = JsonConvert.SerializeObject(reportTagGroup, Formatting.Indented);

            var fileInfo = new FileInfo(outputPath);

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            File.WriteAllText(Path.Combine(Program.PrecursorDirectory, fileInfo.FullName), outputObject);
        }
    }
}
