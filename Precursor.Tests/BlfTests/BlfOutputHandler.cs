using System;
using System.Collections.Generic;
using System.IO;

namespace Precursor.Tests.BlfTests
{
    public static class BlfOutputHandler
    {
        public static string CaptureConsoleOutput(Action action)
        {
            var originalOut = Console.Out;
            var sw = new StringWriter();

            try
            {
                Console.SetOut(sw);
                action();
                Console.Out.Flush();
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            return sw.ToString();
        }

        public static List<string> ParseOutput(string output)
        {
            List<string> problems = [];

            foreach (var line in output.Split('\r', '\n'))
            {
                var trimmed = line.Trim();

                if (trimmed.Contains("[WARNING]:") ||
                    trimmed.Contains("[ERROR]:") ||
                    trimmed.StartsWith("WARNING") ||
                    trimmed.StartsWith("ERROR") ||
                    trimmed.StartsWith("BLF file is invalid")) // Temp hack until we updating warnings in TagTool 
                {
                    trimmed = trimmed
                        .Replace("[WARNING]: ", string.Empty)
                        .Replace("[ERROR]: ", string.Empty)
                        .Replace("WARNING ", string.Empty)
                        .Replace("ERROR ", string.Empty);

                    problems.Add(trimmed);
                }
            }

            return problems;
        }
    }
}
