using System.Collections.Generic;

namespace Precursor.ViewModels
{
    public class FileViewModel
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ErrorLevel { get; set; }
        public int GroupErrorCount { get; set; }
        public List<string> Groups { get; set; }
        public string FileInfo { get; set; }
        public string ErrorInfo { get; set; }
    }
}
