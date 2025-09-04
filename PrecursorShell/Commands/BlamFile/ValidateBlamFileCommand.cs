using PrecursorShell.BlamFile.Resolvers;
using PrecursorShell.Cache;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PrecursorShell.Commands.BlamFile
{
    public class ValidateBlamFileCommand : PrecursorCommand
    {
        public ValidateBlamFileCommand() : base
        (
            false,
            "ValidateBlamFiles",
            "Validates all blf files associated with the specified build version.",

            "ValidateBlamFiles <Build>",
            "Validates all blf files associated with the specified build version."
        )
        {
        }

        public override object Execute(List<string> args)
        {
            if (args.Count != 1)
                return new PrecursorError($"Incorrect amount of arguments supplied");

            if (!Enum.TryParse(args[0], true, out CacheBuild build))
                return new PrecursorError($"Invalid build");

            var buildTable = Program.BuildTable.BuildInfo;

            if (build == CacheBuild.All)
            {
                foreach (var buildInfo in buildTable) 
                {
                    if (buildInfo.ResourcePath != null && Directory.Exists(buildInfo.ResourcePath)) 
                    {
                        BlfResolver.ParseFiles(buildInfo);
                    }
                }
            }
            else 
            {
                var buildInfo = buildTable.Where(x => x.Build == build).FirstOrDefault();

                if (buildInfo.ResourcePath != null && Directory.Exists(buildInfo.ResourcePath))
                {
                    BlfResolver.ParseFiles(buildInfo);
                }
            }

            return true;
        }
    }
}
