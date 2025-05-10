using Precursor.Commands.Context;
using Precursor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Precursor.Commands.Common
{
    public class HelpCommand : PrecursorCommand
    {
        private PrecursorContextStack ContextStack { get; }

        public HelpCommand(PrecursorContextStack contextStack): base
        (
            true,

            "Help",
            "Display help",

            "Help [command]",

            "Displays help on how to use a command.\n" +
            "If no command is given, help will list all available commands."
        )
        {
            ContextStack = contextStack;
        }

        public override object Execute(List<string> args)
        {
            // TODO: Add error handling

            if (args.Count == 1)
                GetCommandHelp(args[0]);
            else
                GetCommands(ContextStack.Context);
            return true;
        }

        public void GetCommands(PrecursorContext precursorContext) 
        {
            var commands = precursorContext.Commands.OrderBy(x => x.Name);

            var width = commands.Max(c => c.Name.Length);
            var format = "{0,-" + width + "}  {1}";

            Console.WriteLine("Available Commands:\n");

            foreach (var command in commands)
            {
                Console.WriteLine(format, command.Name, command.Description);
            }
        }

        public void GetCommandHelp(string command) 
        {
            var precursorCommand = ContextStack.Context.GetCommand(command);
            if (precursorCommand == null)
            {
                new PrecursorError($"Unable to find command \"{command}\"");
                return;
            }

            // TODO: Maybe add indents
            // (Need to figure out how to handle it better than TagTool)

            Console.WriteLine($"\n{precursorCommand.Name}: \n{precursorCommand.Description}");

            Console.WriteLine($"\nUsage: \n{precursorCommand.Usage}");

            if (precursorCommand.Examples != "")
                Console.WriteLine($"\nExamples: \n{precursorCommand.Examples}");

            if (precursorCommand.HelpMessage != "")
                Console.WriteLine($"\nNotes: \n{precursorCommand.HelpMessage}");
        }
    }
}
