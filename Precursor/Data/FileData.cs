using System.Collections.Generic;

namespace Precursor.Data
{
    public class FileData
    {
        public string FileName { get; set; }
        public List<string> Groups { get; set; }
        public string ErrorLevel { get; set; }
        public int GroupErrorCount { get; set; }
    }
}
