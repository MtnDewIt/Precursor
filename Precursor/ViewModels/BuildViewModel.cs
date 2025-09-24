using System.Collections.Generic;

namespace Precursor.ViewModels
{
    public class BuildViewModel
    {
        public string Build { get; set; }
        public string ErrorLevel { get; set; }
        public int FileErrorCount { get; set; }
        public List<string> Files { get; set; }
        public string BuildInfo { get; set; }
        public string ErrorInfo { get; set; }
    }
}
