using System;
using System.Collections.Generic;

namespace Precursor.Commands.Context
{
    public class PrecursorContext
    {
        public PrecursorContext Parent { get; private set; }

        public string Name { get; private set; }

        private Dictionary<string, PrecursorCommand> CommandsByName { get; } = new Dictionary<string, PrecursorCommand>();

        public IEnumerable<PrecursorCommand> Commands => CommandsByName.Values;

        public Dictionary<object, object> ScriptGlobals { get; set; } = new Dictionary<object, object>();

        public PrecursorContext(PrecursorContext parent, string name)
        {
            Parent = parent;
            Name = name;
        }

        public void AddCommand(PrecursorCommand command)
        {
            CommandsByName[command.Name] = command;
        }

        public PrecursorCommand GetCommand(string name)
        {
            CommandsByName.TryGetValue(name, out PrecursorCommand result);

            if (result != null)
                return result;

            var nameLower = name.ToLower();

            foreach (var pair in CommandsByName)
                if (nameLower == pair.Key.ToLower())
                    return pair.Value;

            var nameSnake = name.ToSnakeCase();

            foreach (var pair in CommandsByName)
                if (nameSnake == pair.Key.ToSnakeCase())
                    return pair.Value;

            for (var p = Parent; p != null; p = p.Parent)
            {
                p.CommandsByName.TryGetValue(name, out result);

                if (result != null && result.Inherit)
                    return result;

                foreach (var pair in p.CommandsByName)
                    if (nameLower == pair.Key.ToLower())
                        if (pair.Value.Inherit)
                            return pair.Value;

                foreach (var pair in p.CommandsByName)
                    if (nameSnake == pair.Key.ToSnakeCase())
                        if (pair.Value.Inherit)
                            return pair.Value;
            }

            return null;
        }
    }
}
