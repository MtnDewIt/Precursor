using System;
using System.Collections.Generic;
using TagTool.Commands;

namespace Precursor.Common
{
    public class PrecursorCommandRunner
    {
        public PrecursorContextStack ContextStack;
        public bool Exit { get; private set; } = false;

        [ThreadStatic]
        public static string CommandLine;

        [ThreadStatic]
        public static PrecursorCommandRunner Current;

        public static string CurrentCommandName = "";

        public PrecursorCommandRunner(PrecursorContextStack contextStack)
        {
            ContextStack = contextStack;
        }

        public void RunCommand(string commandLine)
        {
            if (commandLine == null)
            {
                Exit = true;
                return;
            }

            Current = this;
            CommandLine = commandLine;

            var commandArgs = ArgumentParser.ParseCommand(commandLine, out string redirectFile);

            if (commandArgs.Count == 0)
                return;

            switch (commandArgs[0].ToLower())
            {
                case "quit":
                    Exit = true;
                    return;
            }

            if (!ExecuteCommand(ContextStack.Context, commandArgs))
            {
                new PrecursorError($"Unrecognized command \"{commandArgs[0]}\"\n" + "Use \"help\" to list available commands.");
            }
        }

        private bool ExecuteCommand(PrecursorContext context, List<string> commandAndArgs)
        {
            if (commandAndArgs.Count == 0)
                return true;

            // Really need a better way of handling this
            // Maybe use a command id, instead of a name?
            PrecursorCommand command = context.GetCommand(commandAndArgs[0].ToLower()) ?? context.GetCommand(commandAndArgs[0]);

            if (command == null)
                return false;

            commandAndArgs.RemoveAt(0);

            CurrentCommandName = command.Name;
            command.Execute(commandAndArgs);
            CurrentCommandName = "";

            return true;
        }
    }
}
