using System.Collections.Generic;

namespace Precursor.Data
{
    public class BuildData
    {
        public string Build { get; set; }
        public string ErrorLevel { get; set; }
        public int FileErrorCount { get; set; }
        public List<string> Files { get; set; }
    }
}
