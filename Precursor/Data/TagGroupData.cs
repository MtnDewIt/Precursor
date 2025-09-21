using System.Collections.Generic;

namespace Precursor.Data
{
    public class TagGroupData
    {
        public string TagGroup { get; set; }
        public string GroupName { get; set; }
        public List<TagData> Tags { get; set; }
        public string ErrorLevel { get; set; }
        public int TagErrorCount { get; set; }
    }
}
