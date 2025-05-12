using Precursor.Cache.BuildTable;
using Precursor.Cache.BuildTable.Handlers;
using Precursor.Cache.Resolvers;
using Precursor.Common;
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

            "VerifyBuilds [Precursor Build Table]",
            "Verifies all paths and files in the specified precursor build table.\n" + 
            "If no path is specified, then the data in the default build table will be parsed"
        )
        {
        }

        public override object Execute(List<string> args)
        {
            if (args.Count > 1)
                return new PrecursorError($"Incorrect amount of arguments supplied");

            string filePath = args.Count == 1 ? args[0] : Program.PrecursorInput;

            if (!File.Exists(filePath))
                return new PrecursorError($"Unable to locate file \"{filePath}\"");

            var jsonData = File.ReadAllText(filePath);

            BuildTableProperties buildProperties;

            try
            {
                var handler = new BuildTablePropertiesHandler();

                buildProperties = handler.Deserialize(jsonData);

                // TODO: Throw exception is invalid JSON format
                // Either do this in the command or in the handler
            }
            catch (Exception)
            {
                return new PrecursorError($"Unable to parse file \"{filePath}\"");
            }

            foreach (var build in buildProperties?.Builds)
            {
                Console.WriteLine($"\nVerifying {build.Build} Cache Files...");

                var resolver = CacheResolver.GetResolver(build);

                var buildTableEntry = resolver?.VerifyBuild(build);

                Program.BuildTable.EmptyTable();

                if (buildTableEntry != null)
                {
                    Program.BuildTable.AddEntry(buildTableEntry);
                }
            }

            return true;
        }
    }
}
