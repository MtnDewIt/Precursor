using Microsoft.VisualStudio.TestTools.UnitTesting;
using Precursor.Tests.Cache.BuildTable;

namespace Precursor.Tests.CacheTests
{
    [TestClass]
    public static class CacheTestSetup
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context) 
        {
            Globals.BuildTableConfig = BuildTableConfig.ParseConfig();
        }
    }
}