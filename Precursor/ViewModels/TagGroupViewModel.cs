using Precursor.Data;
using System.Collections.Generic;

namespace Precursor.ViewModels
{
    public class TagGroupViewModel
    {
        public string GroupPath { get; set; }
        public string TagGroup { get; set; }
        public string GroupName { get; set; }
        public string ErrorLevel { get; set; }
        public int TagErrorCount { get; set; }
        public List<TagData> Tags { get; set; }
        public string ErrorInfo { get; set; }
    }
}
