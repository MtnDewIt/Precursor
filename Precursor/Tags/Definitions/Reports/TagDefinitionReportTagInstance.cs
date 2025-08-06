using Precursor.Reports;
using System.Collections.Generic;

namespace Precursor.Tags.Definitions.Reports
{
    public class TagDefinitionReportTagInstance
    {
        public string TagName;
        public List<string> Errors = [];

        public TagDefinitionReportTagInstance(string tagName) 
        {
            TagName = tagName;
        }
    }
}
