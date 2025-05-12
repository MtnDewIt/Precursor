using Precursor.Cache.BuildInfo;
using System.Collections.Generic;

namespace Precursor.Cache.BuildTable
{
    public class BuildTable
    {
        private List<BuildInfoEntry> BuildInfo = new List<BuildInfoEntry>();

        public void AddEntry(BuildInfoEntry entry) 
        {
            BuildInfo.Add(entry);
        }

        public void EmptyTable()
        {
            BuildInfo.Clear();
        }
    }
}
