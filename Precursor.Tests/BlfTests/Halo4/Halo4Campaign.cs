using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TagTool.BlamFile;
using TagTool.Cache;
using TagTool.IO;

namespace Precursor.Tests.BlfTests.Halo4
{
    [TestClass]
    public class Halo4Campaign
    {
        private const string ResourcePath = @"Resources\Gen4\Halo4\campaign";

        private const CacheVersion Version = CacheVersion.Halo4;
        private const CachePlatform Platform = CachePlatform.Original;

        private static bool HasErrors = false;

        public static Blf ReadBlf(string file)
        {
            var input = new FileInfo(file);
            var blf = new Blf(Version, Platform);

            string consoleOutput = BlfOutputHandler.CaptureConsoleOutput(() =>
            {
                try
                {
                    using (var inStream = input.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var reader = new EndianReader(inStream);

                        blf.Read(reader);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[ERROR]: Failed to read file \"{input.Name}\": {e.Message}");
                }
            });

            List<string> errors = BlfOutputHandler.ParseOutput(consoleOutput);

            if (errors.Count == 0)
            {
                Console.WriteLine($"[SUCCESS]: Successfully read file \"{input.Name}\"");

                return blf;
            }
            else
            {
                HasErrors = true;

                Console.WriteLine($"[ERROR]: Warnings encountered reading file \"{input.Name}\"");

                foreach (string error in errors)
                {
                    Console.WriteLine($"[ERROR]: {error}");
                }
            }

            return null;
        }

        public static void WriteBlf(Blf blf, string file)
        {
            string fileName = Path.GetFileName(file);

            FileInfo output = new FileInfo(Path.Combine(Path.GetTempPath(), ResourcePath, fileName));

            if (!Directory.Exists(output.DirectoryName))
            {
                Directory.CreateDirectory(output.DirectoryName);
            }

            string consoleOutput = BlfOutputHandler.CaptureConsoleOutput(() =>
            {
                try
                {
                    using (var outStream = output.Create())
                    {
                        var writer = new EndianWriter(outStream);

                        blf.Write(writer);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[ERROR]: Failed to write file \"{output.Name}\": {e.Message}");
                }
            });

            List<string> errors = BlfOutputHandler.ParseOutput(consoleOutput);

            if (errors.Count == 0)
            {
                Console.WriteLine($"[SUCCESS]: Successfully wrote file \"{output.Name}\"");
            }
            else
            {
                HasErrors = true;

                Console.WriteLine($"[ERROR]: Warnings encountered writing file \"{output.Name}\"");

                foreach (string error in errors)
                {
                    Console.WriteLine($"[ERROR]: {error}");
                }
            }

            if (output.Exists)
            {
                File.Delete(output.FullName);
            }
        }

        [TestMethod]
        public void RunReadTest()
        {
            Console.WriteLine($"[BLF READ UNIT TEST OUTPUT]");

            var files = Directory.EnumerateFiles(ResourcePath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                Console.WriteLine($"\n[PARSING FILE {Path.GetFileName(file).ToUpper()}...]");

                ReadBlf(file);
            }

            if (HasErrors)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RunWriteTest()
        {
            Console.WriteLine($"[BLF WRITE UNIT TEST OUTPUT]");

            var files = Directory.EnumerateFiles(ResourcePath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                Console.WriteLine($"\n[PARSING FILE {Path.GetFileName(file).ToUpper()}...]");

                Blf blf = ReadBlf(file);

                WriteBlf(blf, file);
            }

            if (HasErrors)
            {
                Assert.Fail();
            }
        }
    }
}
