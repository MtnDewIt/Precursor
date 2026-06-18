using System.Collections.Generic;

namespace Precursor.Tests.Cache.Reports
{
    public struct CacheDefinitionFileReport
    {
        public string FileName;
        public List<string> Errors;
    }

    public struct CacheDefinitionReport
    {
        public List<CacheDefinitionFileReport> Files;

        public CacheDefinitionReport()
        {
            Files = [];
        }
    }
}
