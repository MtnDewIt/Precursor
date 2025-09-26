using PrecursorShell.Cache;
using PrecursorShell.Common;
using PrecursorShell.Tags.Definitions.Resolvers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PrecursorShell.Commands.Tags
{
    public class ValidateTagResourceDefinitionsCommand : PrecursorCommand
    {
        public ValidateTagResourceDefinitionsCommand() : base
        (
            false,
            "ValidateTagResourceDefinitions",
            "Validates all tag definitions in the specified build version.",

            "ValidateTagResourceDefinitions <Build>",
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
                    //TagResourceDefinitionResolver.ParseDefinitionsAsync(buildInfo);
                }
            }
            else
            {
                var buildInfo = Program.BuildTable.BuildInfo.Where(x => x.Build == build).FirstOrDefault();

                //TagResourceDefinitionResolver.ParseDefinitionsAsync(buildInfo);
            }

            Program.TagDefinitionReport.GenerateReport();

            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);

            return true;
        }
    }
}
