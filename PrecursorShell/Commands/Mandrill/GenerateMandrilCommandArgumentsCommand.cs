using Newtonsoft.Json;
using PrecursorShell.Cache;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PrecursorShell.Commands.Mandrill
{
    public class GenerateMandrillCommandArgumentsCommand : PrecursorCommand
    {
        public enum MandrillProject 
        {
            None = 0,
            DefinitionTweaker,
            EldoradoCacheFileTest,
        }

        public GenerateMandrillCommandArgumentsCommand() : base
        (
            false,
            "GenerateMandrillCommandArguments",
            "Generates command line argument files for the specified mandrill project",

            "GenerateMandrillCommandArguments <Project>",
            "Generates command line argument files for the specified mandrill project"
        )
        {
        }

        public override object Execute(List<string> args)
        {
            if (args.Count != 1)
                return new PrecursorError($"Incorrect amount of arguments supplied!");

            if (!Enum.TryParse(args[0], true, out MandrillProject project))
                return new PrecursorError($"Invalid Project!");

            switch (project) 
            {
                case MandrillProject.DefinitionTweaker:
                    GenerateDefinitionTweakerArguments();
                    break;
                case MandrillProject.EldoradoCacheFileTest:
                    GenerateEldoradoCacheFileTestArguments();
                    break;
                default:
                    return new PrecursorError($"Project not implemented!");
            }

            return true;
        }

        public static void GenerateDefinitionTweakerArguments() 
        {
            var fileInfo = new FileInfo($"{Program.PrecursorDirectory}\\Mandrill\\Arguments\\definitiontweaker.args.json");

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            using var sw = new StreamWriter(fileInfo.FullName);
            using var writer = new JsonTextWriter(sw)
            {
                Formatting = Formatting.Indented,
            };

            writer.WriteStartObject();
            writer.WritePropertyName("FileVersion");
            writer.WriteValue(2);
            writer.WritePropertyName("Id");
            writer.WriteValue(Guid.NewGuid());
            writer.WritePropertyName("Items");
            writer.WriteStartArray();

            foreach (var build in Program.BuildTable.BuildInfo.Where(b => b.Generation == CacheGeneration.GenHaloOnline)) 
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                writer.WriteValue(Guid.NewGuid());
                writer.WritePropertyName("Command");
                writer.WriteValue(build.Build.ToString());
                writer.WritePropertyName("Items");
                writer.WriteStartArray();

                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                writer.WriteValue(Guid.NewGuid());
                writer.WritePropertyName("Command");
                writer.WriteValue($@"-eldoradodir:{ParseFilePath(build.CurrentCacheFiles.First())}");
                writer.WriteEndObject();

                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                writer.WriteValue(Guid.NewGuid());
                writer.WritePropertyName("Command");
                writer.WriteValue($@"-tag-definitions-output-directory:{ParseFilePath(build.CurrentCacheFiles.First())}output\definitions\");
                writer.WriteEndObject();

                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                writer.WriteValue(Guid.NewGuid());
                writer.WritePropertyName("Command");
                writer.WriteValue($@"-tag-groups-output-directory:{ParseFilePath(build.CurrentCacheFiles.First())}output\groups\");
                writer.WriteEndObject();

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public static void GenerateEldoradoCacheFileTestArguments() 
        {
            var fileInfo = new FileInfo($"{Program.PrecursorDirectory}\\Mandrill\\Arguments\\eldoradocachefiletest.args.json");

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            using var sw = new StreamWriter(fileInfo.FullName);
            using var writer = new JsonTextWriter(sw)
            {
                Formatting = Formatting.Indented,
            };

            writer.WriteStartObject();
            writer.WritePropertyName("FileVersion");
            writer.WriteValue(2);
            writer.WritePropertyName("Id");
            writer.WriteValue(Guid.NewGuid());
            writer.WritePropertyName("Items");
            writer.WriteStartArray();

            foreach (var build in Program.BuildTable.BuildInfo.Where(b => b.Generation == CacheGeneration.GenHaloOnline)) 
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                writer.WriteValue(Guid.NewGuid());
                writer.WritePropertyName("Command");
                writer.WriteValue(build.Build.ToString());
                writer.WritePropertyName("Items");
                writer.WriteStartArray();

                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                writer.WriteValue(Guid.NewGuid());
                writer.WritePropertyName("Command");
                writer.WriteValue($@"-eldoradodir:{ParseFilePath(build.CurrentCacheFiles.First())}");
                writer.WriteEndObject();

                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                writer.WriteValue(Guid.NewGuid());
                writer.WritePropertyName("Command");
                writer.WriteValue($@"-outputdir:{ParseFilePath(build.CurrentCacheFiles.First())}tags\");
                writer.WriteEndObject();

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public static string ParseFilePath(string filePath) 
        {
            var result = Path.GetDirectoryName(filePath);

            return result.Replace("maps", "");
        }
    }
}
