using PrecursorShell.Cache;
using PrecursorShell.Cache.Resolvers;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PrecursorShell.Commands.Cache
{
    public class ValidateCacheDefinitionsCommand : PrecursorCommand
    {
        public ValidateCacheDefinitionsCommand() : base
        (
            false,
            "ValidateCacheDefinitions",
            "Validates header definitions for each file in the specified build version.",

            "ValidateCacheDefinitions <Build>",
            "Validates header definitions for each file in the specified build version."
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
                    CacheDefinitionResolver.ParseDefinitionsAsync(buildInfo);
                }
            }
            else
            {
                var buildInfo = Program.BuildTable.BuildInfo.Where(x => x.Build == build).FirstOrDefault();

                CacheDefinitionResolver.ParseDefinitionsAsync(buildInfo);
            }

            Program.CacheDefinitionReport.GenerateReport();

            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);

            return true;
        }
    }
}
