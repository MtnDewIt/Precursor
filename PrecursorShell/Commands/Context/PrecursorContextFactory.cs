using PrecursorShell.Commands.BlamFile;
using PrecursorShell.Commands.Builds;
using PrecursorShell.Commands.Cache;
using PrecursorShell.Commands.Common;
using PrecursorShell.Commands.ConvertCache;
using PrecursorShell.Commands.Mandrill;
using PrecursorShell.Commands.Tags;

namespace PrecursorShell.Commands.Context
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

            context.AddCommand(new ValidateBitmapsCommand());
            context.AddCommand(new ValidateBlamFilesCommand());
            context.AddCommand(new ValidateCacheDefinitionsCommand());
            context.AddCommand(new ValidateTagResourceDefinitionsCommand());
            context.AddCommand(new ValidateTagDefinitionsCommand());

            context.AddCommand(new GenerateMandrillCommandArgumentsCommand());

            context.AddCommand(new ConvertCacheCommand());

            // TODO: Pass the cache into the command contsructor
            //context.AddCommand(new GenerateCacheCommand(null));
            //context.AddCommand(new GenerateDonkeyCacheCommand(null, contextStack));

            context.AddCommand(new DebugTestCommand(null, null, contextStack));
        }
    }
}
