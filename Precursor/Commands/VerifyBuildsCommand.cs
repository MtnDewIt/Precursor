using Precursor.Cache.BuildTable.Handlers;
using Precursor.Cache.Resolvers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Precursor.Commands
{
    public class VerifyBuildsCommand : PrecursorCommand
    {
        public VerifyBuildsCommand() : base
        (
            true,
            "VerifyBuilds",
            "Verifies all paths and files in the specified precursor build table.",

            "VerifyBuilds",
            "Verifies all paths and files in the specified precursor build table.\n" + 
            "If no path is specified, then the data in the default build table will be parsed"
        )
        {
        }

        public override object Execute(List<string> args)
        {
            // TODO: Add error handling

            // Maybe add the ability to specify a path to a different build table
            // (Would probably involve importing the data into the default build table location)

            var jsonData = File.ReadAllText(Program.PrecursorInput);

            var handler = new BuildTablePropertiesHandler();

            var cacheObject = handler.Deserialize(jsonData);

            foreach (var build in cacheObject.Builds)
            {
                Console.WriteLine($"Verifying {build.Build} Cache Files...");

                var resolver = CacheResolver.GetResolver(build.Build);

                // This will return a build data object at some point;
                resolver?.VerifyBuild(build);
            }

            return true;
        }
    }
}
