using PrecursorShell.Cache;
using PrecursorShell.Common;
using PrecursorShell.Tags.Definitions.Resolvers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PrecursorShell.Commands.Tags
{
    public class ValidateTagDefinitionsCommand : PrecursorCommand
    {
        public ValidateTagDefinitionsCommand() : base
        (
            false,
            "ValidateTagDefinitions",
            "Validates all tag definitions in the specified build version.",

            "ValidateTagDefinitions <Build>",
            "Validates all tag definitions in the specified build version."
        )
        {
        }

        public override object Execute(List<string> args) 
        {
            if (args.Count != 1)
                return new PrecursorError($"Incorrect amount of arguments supplied");

            if (!Enum.TryParse(args[0], true, out CacheBuild build))
                return new PrecursorError($"Invalid build");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (build == CacheBuild.All)
            {
                foreach (var buildInfo in Program.BuildTable.BuildInfo)
                {
                    TagDefinitionResolver.ParseDefinitionsAsync(buildInfo);
                }
            }
            else
            {
                var buildInfo = Program.BuildTable.BuildInfo.Where(x => x.Build == build).FirstOrDefault();

                TagDefinitionResolver.ParseDefinitionsAsync(buildInfo);
            }

            Program.TagDefinitionReport.GenerateReport();

            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);

            return true;
        }
    }
}
