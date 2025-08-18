using PrecursorShell.Commands.Context;
using System.Collections.Generic;
using TagTool.Cache;

namespace PrecursorShell.Commands
{
    class DebugTestCommand : PrecursorCommand
    {
        public GameCache Cache { get; set; }
        public GameCacheHaloOnlineBase CacheContext { get; set; }
        public PrecursorContextStack ContextStack { get; set; }

        public DebugTestCommand(GameCache cache, GameCacheHaloOnlineBase cacheContext, PrecursorContextStack contextStack) : base
        (
            false,
            "DebugTest",
            "Self Explanatory",

            "DebugTest",
            "Self Explanatory"
        )
        {
            Cache = cache;
            CacheContext = cacheContext;
            ContextStack = contextStack;
        }

        public override object Execute(List<string> args)
        {
            return true;
        }
    }
}
