using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precursor.Tags.Definitions.Reports
{
    public class TagDefinitionReportCacheFile
    {
        public string FileName;
        public List<string> Groups = [];

        public TagDefinitionReportCacheFile(string fileName) 
        {
            FileName = fileName;
        }

        public static void GenerateReportCacheFiles(TagDefinitionReportCacheFile reportCacheFile, string outputPath) 
        {
            var outputObject = JsonConvert.SerializeObject(reportCacheFile, Formatting.Indented);

            var fileInfo = new FileInfo(Path.Combine($"Reports\\TagDefinitions", outputPath));

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            File.WriteAllText(Path.Combine(Program.PrecursorDirectory, fileInfo.FullName), outputObject);
        }
    }
}
