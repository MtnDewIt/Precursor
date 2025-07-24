using Precursor.Cache;
using Precursor.Common;
using Precursor.Tags.Definitions.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Precursor.Commands.Tags
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

            var buildTable = Program.BuildTable.GetEntryTable();

            if (build == CacheBuild.All)
            {
                foreach (var buildInfo in buildTable)
                {
                    TagDefinitionResolver.ParseDefinitions(buildInfo);
                }
            }
            else
            {
                var buildInfo = buildTable.Where(x => x.GetBuild() == build).FirstOrDefault();

                TagDefinitionResolver.ParseDefinitions(buildInfo);
            }

            return true;
        }
    }
}
