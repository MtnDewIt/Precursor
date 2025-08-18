using PrecursorShell.Cache.BuildInfo;
using System.Collections.Generic;

namespace PrecursorShell.Cache.BuildTable
{
    public class BuildTable
    {
        private List<BuildInfoEntry> BuildInfo { get; set; }

        public BuildTable()
        {
            BuildInfo = new List<BuildInfoEntry>();
        }

        public List<BuildInfoEntry> GetEntryTable() => BuildInfo;

        public void AddEntry(BuildInfoEntry entry) => BuildInfo.Add(entry);

        public void RemoveEntry(BuildInfoEntry entry) => BuildInfo.Remove(entry);

        public void EmptyTable() => BuildInfo.Clear();
    }
}
