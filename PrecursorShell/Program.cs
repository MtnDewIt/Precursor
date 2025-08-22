using PrecursorShell.Cache.BuildInfo;
using PrecursorShell.Cache.BuildTable;
using PrecursorShell.Commands;
using PrecursorShell.Commands.Context;
using PrecursorShell.Common;
using PrecursorShell.Tags.Definitions.Reports;
using System;
using System.IO;
using System.Reflection;

namespace PrecursorShell
{
    public class Program
    {
        public static string PrecursorDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string ConfigPath = Path.Combine(PrecursorDirectory, "Config.json");

        // TODO: Move these elsewhere
        public static BuildTable BuildTable = new BuildTable();
        // public static BlamFileReport BlamFileReport = new BlamFileReport();
        // public static CacheBuildReport CacheBuildReport = new CacheFileReport();
        public static TagDefinitionReport TagDefinitionReport = new TagDefinitionReport();

        static void Main(string[] args)
        {
            Console.WriteLine($"Precursor [{Assembly.GetExecutingAssembly().GetName().Version} (Built {GetLinkerTimestampUtc(Assembly.GetExecutingAssembly())} UTC)]\n");

            var isNewFile = false;

            if (!File.Exists(ConfigPath))
            {
                new PrecursorWarning("Unable to locate Config.json. Generating default data...");
                BuildTableConfig.GenerateEmptyConfig();
                isNewFile = true;
            }

            if (!isNewFile)
            {
                var buildTable = BuildTableConfig.ParseConfig();

                BuildTable.ParseBuildTable(buildTable);
            }
            else
            {
                new PrecursorWarning("Default data generated. Cache paths will need to be populated manually");
            }

            Console.WriteLine("\nEnter \"help\" to list available commands. Enter \"quit\" to quit.");

            var contextStack = new PrecursorContextStack();
            var context = PrecursorContextFactory.Create(contextStack);
            contextStack.Push(context);

            var commandRunner = new PrecursorCommandRunner(contextStack);

            while (!commandRunner.Exit) 
            {
                Console.WriteLine();
                Console.Write("> ");
                Console.Title = $"Precursor";

                var line = Console.ReadLine();

                commandRunner.RunCommand(line);
            }
        }

        public static DateTime GetLinkerTimestampUtc(Assembly assembly)
        {
            var location = assembly.Location;
            return GetLinkerTimestampUtc(location);
        }

        public static DateTime GetLinkerTimestampUtc(string filePath)
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;
            var bytes = new byte[2048];

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                file.Read(bytes, 0, bytes.Length);
            }

            var headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(bytes, headerPos + linkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddSeconds(secondsSince1970);
        }
    }
}