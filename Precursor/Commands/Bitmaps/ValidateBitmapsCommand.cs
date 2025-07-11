using Precursor.Bitmaps.Resolvers;
using Precursor.BlamFile.Resolvers;
using Precursor.Cache;
using Precursor.Commands.Context;
using Precursor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TagTool.Cache;

namespace Precursor.Commands
{
    class ValidateBitmapsCommand : PrecursorCommand
    {
        public GameCache Cache { get; set; }
        public GameCacheHaloOnlineBase CacheContext { get; set; }
        public PrecursorContextStack ContextStack { get; set; }

        public ValidateBitmapsCommand(GameCache cache, GameCacheHaloOnlineBase cacheContext, PrecursorContextStack contextStack) : base
        (
            false,
            "ValidateBitmaps",
            "Validates all bitmap formats in the specified build version.",

            "ValidateBitmaps <Build>",
            "Validates all bitmap formats in the specified build version."
        )
        {
            Cache = cache;
            CacheContext = cacheContext;
            ContextStack = contextStack;
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
                    switch (buildInfo.GetBuild()) 
                    {
                        case CacheBuild.Halo3Retail:
                        case CacheBuild.Halo3MythicRetail:
                        case CacheBuild.Halo3ODST:
                        case CacheBuild.HaloReach:
                        case CacheBuild.Halo3MCC:
                        case CacheBuild.Halo3ODSTMCC:
                        case CacheBuild.HaloReachMCC:
                            BitmapResolver.ParseFiles(buildInfo);
                            break;
                        default:
                            return new PrecursorWarning($"Unsupported build - {buildInfo.GetBuild()}");
                    }
                }
            }
            else
            {
                var buildInfo = buildTable.Where(x => x.GetBuild() == build).FirstOrDefault();

                switch (buildInfo.GetBuild())
                {
                    case CacheBuild.Halo3Retail:
                    case CacheBuild.Halo3MythicRetail:
                    case CacheBuild.Halo3ODST:
                    case CacheBuild.HaloReach:
                    case CacheBuild.Halo3MCC:
                    case CacheBuild.Halo3ODSTMCC:
                    case CacheBuild.HaloReachMCC:
                        BitmapResolver.ParseFiles(buildInfo);
                        break;
                    default:
                        return new PrecursorWarning($"Unsupported build - {buildInfo.GetBuild()}");
                }
            }

            return true;
        }
    }
}
