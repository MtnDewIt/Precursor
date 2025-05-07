using Precursor.Cache.Resolvers;
using Precursor.Resolvers;
using System.Globalization;
using System.IO;
using System.Reflection;
using System;

namespace Precursor
{
    public static class Program
    {
        public static string PrecursorDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");

            AssemblyResolver.ResolveManagedAssemblies();
            AssemblyResolver.ResolveUnmanagedAssemblies();

            Console.WriteLine($"Precursor [{Assembly.GetExecutingAssembly().GetName().Version} (Built {GetLinkerTimestampUtc(Assembly.GetExecutingAssembly())} UTC)]\n");

            var inputPath = $@"{PrecursorDirectory}\Precursor.json";

            var isNewFile = false;

            if (!File.Exists(inputPath)) 
            {
                Console.WriteLine("> Unable to locate Precursor.json\n");
                Console.WriteLine("> Generating default data...\n");
                CacheResolver.GenerateBuildData(inputPath);
                isNewFile = true;
            }

            if (!isNewFile)
            {
                CacheResolver.VerifyBuilds(inputPath);
            }
            else 
            {
                Console.WriteLine("> Default data generated. Cache paths will need to be populated manually");
            }

            Console.WriteLine("\nEnter \"help\" to list available commands. Enter \"quit\" to quit.");


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