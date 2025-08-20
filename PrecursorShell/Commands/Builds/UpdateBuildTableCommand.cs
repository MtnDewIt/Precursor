using PrecursorShell.Cache;
using PrecursorShell.Cache.BuildTable;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace PrecursorShell.Commands.Builds
{
    public class UpdateBuildTableCommand : PrecursorCommand
    {
        public UpdateBuildTableCommand() : base
        (
            true,
            "UpdateBuildTable",
            "Updates the specified entry in the precursor build table properties",

            "UpdateBuildTable <Build> <Path>",
            "Updates the specified entry in the precursor build table properties\n" + 
            "Updating any entries in the build table properties will require the\n" + 
            "build table to be reverified. See \"VerifyBuilds\" for more information.\n" +
            "Verifying the build table using an external file will override any\n" +
            "modifications to the build table made using this command." 
        )
        {
        }

        public override object Execute(List<string> args) 
        {
            if (args.Count != 2)
                return new PrecursorError($"Incorrect amount of arguments supplied");

            if (!Enum.TryParse(args[0], out CacheBuild build))
                return new PrecursorError($"Invalid build");

            var buildPath = args[1];

            if (!Path.Exists(buildPath))
                return new PrecursorError($"Unable to locate directory \"{buildPath}\"");

            var buildTable = BuildTableConfig.ParseConfig();

            foreach (var buildEntry in buildTable.Builds) 
            {
                if (build == CacheBuild.MCCRetail)
                {
                    switch (buildEntry.Build)
                    {
                        case CacheBuild.Halo1MCC:
                            buildEntry.Path = $@"{buildPath}\halo1\maps";
                            break;
                        case CacheBuild.Halo2MCC:
                            buildEntry.Path = $@"{buildPath}\halo2\h2_maps_win64_dx11";
                            break;
                        case CacheBuild.Halo3MCC:
                            buildEntry.Path = $@"{buildPath}\halo3\maps";
                            break;
                        case CacheBuild.Halo3ODSTMCC:
                            buildEntry.Path = $@"{buildPath}\halo3odst\maps";
                            break;
                        case CacheBuild.HaloReachMCC:
                            buildEntry.Path = $@"{buildPath}\haloreach\maps";
                            break;
                        case CacheBuild.Halo4MCC:
                            buildEntry.Path = $@"{buildPath}\halo4\maps";
                            break;
                        case CacheBuild.Halo2AMPMCC:
                            buildEntry.Path = $@"{buildPath}\groundhog\maps";
                            break;
                    }
                }
                else if (buildEntry.Build == build)
                {
                    buildEntry.Path = buildPath;
                }
            }

            buildTable.GenerateConfig();

            Console.WriteLine($"Successfully updated path for {build} build");

            return true;
        }
    }
}
