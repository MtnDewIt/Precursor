using Microsoft.VisualStudio.TestTools.UnitTesting;
using Precursor.Tests.Cache;
using Precursor.Tests.Cache.BuildInfo;
using Precursor.Tests.Cache.BuildTable;
using Precursor.Tests.Cache.Reports;
using Precursor.Tests.Cache.Resolvers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Precursor.Tests.CacheTests.Halo4220811
{
    [TestClass]
    public class Halo4220811CacheTest
    {
        private static BuildTableConfig.BuildTableEntry Build = Globals.BuildTableConfig.Builds.FirstOrDefault(x => x.Build == CacheBuild.Halo4220811);
        private static BuildTableEntry Entry = BuildTableEntry.GetBuildEntry(Build);

        [TestMethod]
        public void RunFileTest()
        {
            Console.WriteLine($"[CACHE FILE UNIT TEST OUTPUT]");

            var files = Directory.EnumerateFiles(Build.Path, "*.*", SearchOption.AllDirectories).Where(x => x.EndsWith(".dat") || x.StartsWith($@"{Build.Path}\blobs")).ToArray();

            if (!BuildTableEntry.ParseFileCount(files.Length))
            {
                Assert.Fail();
            }

            var (validCount, errors) = Task.Run(async () => await Entry.VerifyFilesAsync(files)).GetAwaiter().GetResult();

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
            bool hasErrors = false;

            Console.WriteLine($"[CACHE FILE HEADER UNIT TEST OUTPUT]");

            if (!Entry.VerifyBuildInfo(Build))
            {
                Console.WriteLine($"[ERROR]: Failed to verify build info");

                Assert.Fail();
            }

            CacheDefinitionReport report = CacheDefinitionResolver.ParseDefinitionsAsync(Entry);

            foreach (var file in report.Files)
            {
                if (file.Errors.Count > 0)
                {
                    hasErrors = true;

                    Console.WriteLine($"[ERROR]: Errors encountered when parsing \"{file.FileName}\"");

                    foreach (string error in file.Errors)
                    {
                        Console.WriteLine($"[ERROR]: {error}");
                    }
                }
            }

            if (hasErrors)
            {
                Assert.Fail();
            }
            else
            {
                Console.WriteLine($"[SUCCESS] Successfully Verified Headers for {report.Files.Count} Files\n");
            }
        }
    }
}
