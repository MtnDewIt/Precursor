using Microsoft.VisualStudio.TestTools.UnitTesting;
using Precursor.Tests.Cache;
using Precursor.Tests.Cache.BuildInfo;
using Precursor.Tests.Cache.BuildTable;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Precursor.Tests.CacheTests.HaloOnline498295
{
    [TestClass]
    public class HaloOnline498295CacheTest
    {
        private static BuildTableConfig.BuildTableEntry Build = Globals.BuildTableConfig.Builds.FirstOrDefault(x => x.Build == CacheBuild.HaloOnline498295);
        private static BuildTableEntry Entry = BuildTableEntry.GetBuildEntry(Build);

        [TestMethod]
        public void RunFileTest()
        {
            Console.WriteLine($"[CACHE FILE UNIT TEST OUTPUT]");

            var files = Directory.EnumerateFiles(Build.Path, "*.*", SearchOption.AllDirectories).Where(x => x.EndsWith(".map") || x.EndsWith(".dat")).ToArray();

            if (!BuildTableEntry.ParseFileCount(files.Length))
            {
                Assert.Fail();
            }

            var (validCount, errors) = Task.Run(async () => await Entry.VerifyFilesAsync(files)).GetAwaiter().GetResult();

            errors = [.. errors.Union(BuildTableEntry.ParseFiles(Entry.CacheFiles, Entry.CurrentCacheFiles))];
            errors = [.. errors.Union(BuildTableEntry.ParseFiles(Entry.SharedFiles, Entry.CurrentSharedFiles))];

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    Console.WriteLine($"[ERROR]: {error}");
                }

                Assert.Fail();
            }

            Console.WriteLine($"[SUCCESS] Successfully Verified {validCount}/{files.Length} Files\n");
        }
        
        [TestMethod]
        public void RunCacheTest()
        {
            
        }
    }
}
