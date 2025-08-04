using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
