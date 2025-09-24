using System.Collections.Generic;

namespace Precursor.ViewModels
{
    public class TagViewModel
    {
        public string TagName { get; set; }
        public List<string> Errors { get; set; }
        public string ErrorLevel { get; set; }
        public string TagInfo { get; set; }
        public string ErrorInfo { get; set; }
    }
}
