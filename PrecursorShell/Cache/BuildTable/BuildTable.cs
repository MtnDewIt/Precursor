using PrecursorShell.Cache.BuildInfo;
using System.Collections.Generic;

namespace PrecursorShell.Cache.BuildTable
{
    public class BuildTable
    {
        private List<BuildTableEntry> _buildInfo { get; set; }

        public BuildTable()
        {
           _buildInfo = [];
        }

        public List<BuildTableEntry> BuildInfo => _buildInfo;

        public void AddEntry(BuildTableEntry entry) => _buildInfo.Add(entry);

        public void RemoveEntry(BuildTableEntry entry) => _buildInfo.Remove(entry);

        public void EmptyTable() => _buildInfo.Clear();
    }
}
