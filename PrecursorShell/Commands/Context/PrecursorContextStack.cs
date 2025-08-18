using System;
using System.Collections.Generic;

namespace PrecursorShell.Commands.Context
{
    public class PrecursorContextStack
    {
        private Stack<PrecursorContext> ContextStack { get; } = new Stack<PrecursorContext>();

        public event Action<PrecursorContext> ContextPopped;

        public PrecursorContext Context => ContextStack.Count > 0 ? ContextStack.Peek() : null;

        public void Push(PrecursorContext context)
        {
            ContextStack.Push(context);
        }

        public bool Pop()
        {
            var context = ContextStack.Pop();
            ContextPopped?.Invoke(context);

            return true;
        }
    }
}
