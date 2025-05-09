namespace Precursor.Common
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
            
        }
    }
}
