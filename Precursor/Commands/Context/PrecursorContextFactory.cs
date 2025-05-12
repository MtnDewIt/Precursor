using Precursor.Commands.Builds;
using Precursor.Commands.Common;

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
        }
    }
}
