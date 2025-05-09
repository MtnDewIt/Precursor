using System;
using System.Collections.Generic;

namespace Precursor.Commands.Common
{
    public class ClearCommand : PrecursorCommand
    {
        public ClearCommand(): base
        (
            true,
            "Clear",
            "Clears the screen of all output",
            
            "Clear",
            "Clears the screen of all output."
        )
        {
        }

        public override object Execute(List<string> args)
        {
            if (args.Count > 0)
                return false;

            Console.Clear();
            return true;
        }
    }
}
