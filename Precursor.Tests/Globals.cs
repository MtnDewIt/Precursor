using Precursor.Tests.Cache.BuildTable;

namespace Precursor.Tests
{
    public static class Globals
    {
        public const string TablePath = @"Resources\BuildTable.json";

        public static BuildTable BuildTable = new();
        public static BuildTableConfig BuildTableConfig = new();
    }
}
