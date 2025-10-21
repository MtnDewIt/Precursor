using Bungie;
using Bungie.Tags;
using PrecursorShell.Commands.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.Cache;

namespace PrecursorShell.Commands
{
    class DebugTestCommand : PrecursorCommand
    {
        public GameCache Cache { get; set; }
        public GameCacheEldoradoBase CacheContext { get; set; }
        public PrecursorContextStack ContextStack { get; set; }

        public DebugTestCommand(GameCache cache, GameCacheEldoradoBase cacheContext, PrecursorContextStack contextStack) : base
        (
            false,
            "DebugTest",
            "Self Explanatory",

            "DebugTest",
            "Self Explanatory"
        )
        {
            Cache = cache;
            CacheContext = cacheContext;
            ContextStack = contextStack;
        }

        public override object Execute(List<string> args)
        {
            var cache = GameCache.Open(@"D:\SteamLibrary\steamapps\common\H3EK\maps\guardian.map");

            var groups = (cache as GameCacheGen3).TagCacheGen3.Groups.ToDictionary(g => g.Tag, g => g.Name);

            string h3ek = "D:\\SteamLibrary\\steamapps\\common\\H3EK";
            ManagedBlamSystem.InitializeProject(InitializationType.TagsOnly, h3ek);

            foreach (var group in groups) 
            {
                Console.WriteLine(group.Key);

                switch (group.Key.ToString()) 
                {
                    case "rm  ":
                    case "rmc":
                    case "rmp":
                    case "rmb ":
                    case "rmlv":
                    case "devi":
                    case "item":
                    case "obje":
                    case "unit":
                        continue;
                    default:
                        break;
                }

                var tagPath = TagPath.FromPathAndType($"definition_dumper\\test_{group.Value}_definition", $"{group.Key}*");

                using (var tagFile = new TagFile())
                {
                    tagFile.New(tagPath);
                    tagFile.Save();
                }
            }

            return true;
        }
    }
}
