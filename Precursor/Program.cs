using Precursor.Cache.Resolvers;
using Precursor.Resolvers;
using System.Globalization;
using System.IO;
using System.Reflection;
using System;
using Precursor.Cache.Objects;
using Precursor.Common;
using Precursor.Commands;
using Precursor.Commands.Context;

namespace Precursor
{
    public static class Program
    {
        public static string PrecursorDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string PrecursorInput = Path.Combine(PrecursorDirectory, "Precursor.json");

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");

            AssemblyResolver.ResolveManagedAssemblies();
            AssemblyResolver.ResolveUnmanagedAssemblies();

            Console.WriteLine($"Precursor [{Assembly.GetExecutingAssembly().GetName().Version} (Built {GetLinkerTimestampUtc(Assembly.GetExecutingAssembly())} UTC)]\n");

            var isNewFile = false;

            if (!File.Exists(PrecursorInput))
            {
                new PrecursorWarning("Unable to locate Precursor.json");
                new PrecursorWarning("Generating default data...");
                CacheObject.GenerateBuildData(PrecursorInput);
                isNewFile = true;
            }

            if (!isNewFile)
            {
                CacheResolver.VerifyBuilds(PrecursorInput);
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