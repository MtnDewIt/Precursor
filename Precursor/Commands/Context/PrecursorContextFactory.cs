using Precursor.Commands.BlamFile;
using Precursor.Commands.Builds;
using Precursor.Commands.Common;
using Precursor.Commands.ConvertCache;
using Precursor.Commands.GenerateCache;
using Precursor.Commands.GenerateDonkeyCache;

namespace Precursor.Commands.Context
{
    public static class PrecursorContextFactory
    {
        public static PrecursorContext Create(PrecursorContextStack contextStack)
        {
            var context = new PrecursorContext(contextStack.Context, "");
            Populate(contextStack, context);
            return context;
        }

        public static void Populate(PrecursorContextStack contextStack, PrecursorContext context) 
        {
            context.AddCommand(new ClearCommand());
            context.AddCommand(new HelpCommand(contextStack));

            context.AddCommand(new VerifyBuildsCommand());
            context.AddCommand(new UpdateBuildTableCommand());

            // TODO: Pass the cache into the command contsructor
            context.AddCommand(new ValidateBitmapsCommand(null, null, contextStack));
            context.AddCommand(new ValidateBlamFileCommand());

            // TODO: Pass the cache into the command contsructor
            context.AddCommand(new ConvertCacheCommand(null, null));
            context.AddCommand(new GenerateCacheCommand(null));
            context.AddCommand(new GenerateDonkeyCacheCommand(null, contextStack));

            context.AddCommand(new DebugTestCommand(null, null, contextStack));
        }
    }
}
