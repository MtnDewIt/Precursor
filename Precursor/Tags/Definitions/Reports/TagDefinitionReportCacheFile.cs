using Precursor.Reports;
using Precursor.Tags.Definitions.Reports.Handlers;
using System.Collections.Generic;
using System.IO;

namespace Precursor.Tags.Definitions.Reports
{
    public class TagDefinitionReportCacheFile
    {
        public string FileName;
        public ReportErrorLevel ErrorLevel;
        public int GroupErrorCount;
        public List<string> Groups = [];

        public TagDefinitionReportCacheFile(string fileName) 
        {
            FileName = fileName;
        }

        public static void GenerateReportCacheFiles(TagDefinitionReportCacheFile reportCacheFile, string outputPath) 
        {
            var handler = new TagDefinitionReportCacheFileHandler();

            var outputObject = handler.Serialize(reportCacheFile);

            var fileInfo = new FileInfo(Path.Combine($"Reports\\TagDefinitions", outputPath));

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            File.WriteAllText(Path.Combine(Program.PrecursorDirectory, fileInfo.FullName), outputObject);
        }
    }
}
