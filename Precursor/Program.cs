using System.Globalization;
using TagTool.Commands.Common;
using HaloShaderGenerator.Globals;

namespace Precursor
{
    public static class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");

            var hlslFloat = HLSLType.Float;
            new TagToolWarning("Silence fills the empty grave");

            // Basic idea is it opens its own command context similar to TagTool
            // The input file is a static json file containing all paths to all required engine versions
            // all unit tests are run in parallel with the option to run unit tests for a given cache version, or cache generation
            // shader unit tests are also run in parallel, with separate tests for explicit, chud, global, and template generation
            // default config is to run all unit tests available.
        }
    }
}