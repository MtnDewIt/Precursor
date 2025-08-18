using System.Collections.Generic;

namespace PrecursorShell.Commands
{
    public abstract class PrecursorCommand
    {
        protected PrecursorCommand(bool inherit, string name, string description, string usage, string helpMessage = "", bool ignoreVars = false, string examples = "")
        {
            Name = name;
            Description = description;
            Usage = usage;
            HelpMessage = helpMessage;
            Examples = examples;
            Inherit = inherit;
            IgnoreArgumentVariables = ignoreVars;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Usage { get; private set; }
        public string HelpMessage { get; private set; }
        public string Examples { get; private set; }
        public bool Inherit { get; private set; }
        public bool IgnoreArgumentVariables { get; private set; }

        public abstract object Execute(List<string> args);
    }
}
