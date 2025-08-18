using PrecursorShell.Commands.Context;
using PrecursorShell.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TagTool.Cache;
using TagTool.Cache.HaloOnline;

namespace PrecursorShell.Commands.GenerateDonkeyCache
{
    /*
    partial class GenerateDonkeyCacheCommand : PrecursorCommand
    {
        public GameCache Cache { get; set; }
        public GameCacheHaloOnline CacheContext { get; set; }
        public PrecursorContextStack ContextStack { get; set; }
        public Stream CacheStream { get; set; }
        public static DirectoryInfo SourceDirectoryInfo { get; set; }
        public static DirectoryInfo OutputDirectoryInfo { get; set; }
        public Stopwatch StopWatch { get; set; }

        public static DirectoryInfo haloOnlineDirectoryInfo { get; set; }
        public static DirectoryInfo halo3DirectoryInfo { get; set; }
        public static DirectoryInfo halo3MythicDirectoryInfo { get; set; }
        public static DirectoryInfo halo3ODSTDirectoryInfo { get; set; }

        public static GameCache haloOnlineCache { get; set; }
        public static PortingContextGen3 haloOnline { get; set; }

        public GameCache h3MainMenuCache { get; set; }
        public PortingContextGen3 h3MainMenu { get; set; }
        public GameCache introCache { get; set; }
        public PortingContextGen3 intro { get; set; }
        public GameCache jungleCache { get; set; }
        public PortingContextGen3 jungle { get; set; }
        public GameCache crowsCache { get; set; }
        public PortingContextGen3 crows { get; set; }
        public GameCache outskirtsCache { get; set; }
        public PortingContextGen3 outskirts { get; set; }
        public GameCache voiCache { get; set; }
        public PortingContextGen3 voi { get; set; }
        public GameCache floodvoiCache { get; set; }
        public PortingContextGen3 floodvoi { get; set; }
        public GameCache wasteCache { get; set; }
        public PortingContextGen3 waste { get; set; }
        public GameCache citadelCache { get; set; }
        public PortingContextGen3 citadel { get; set; }
        public GameCache highCharityCache { get; set; }
        public PortingContextGen3 highCharity { get; set; }
        public GameCache haloCache { get; set; }
        public PortingContextGen3 halo { get; set; }
        public GameCache epilogueCache { get; set; }
        public PortingContextGen3 epilogue { get; set; }

        public GameCache mythicMainMenuCache { get; set; }
        public PortingContextGen3 mythicMainMenu { get; set; }
        public GameCache armoryCache { get; set; }
        public PortingContextGen3 armory { get; set; }
        public GameCache bunkerworldCache { get; set; }
        public PortingContextGen3 bunkerworld { get; set; }
        public GameCache chillCache { get; set; }
        public PortingContextGen3 chill { get; set; }
        public GameCache chilloutCache { get; set; }
        public PortingContextGen3 chillout { get; set; }
        public GameCache constructCache { get; set; }
        public PortingContextGen3 construct { get; set; }
        public GameCache cyberdyneCache { get; set; }
        public PortingContextGen3 cyberdyne { get; set; }
        public GameCache deadlockCache { get; set; }
        public PortingContextGen3 deadlock { get; set; }
        public GameCache descentCache { get; set; }
        public PortingContextGen3 descent { get; set; }
        public GameCache docksCache { get; set; }
        public PortingContextGen3 docks { get; set; }
        public GameCache fortressCache { get; set; }
        public PortingContextGen3 fortress { get; set; }
        public GameCache ghosttownCache { get; set; }
        public PortingContextGen3 ghosttown { get; set; }
        public GameCache guardianCache { get; set; }
        public PortingContextGen3 guardian { get; set; }
        public GameCache isolationCache { get; set; }
        public PortingContextGen3 isolation { get; set; }
        public GameCache lockoutCache { get; set; }
        public PortingContextGen3 lockout { get; set; }
        public GameCache midshipCache { get; set; }
        public PortingContextGen3 midship { get; set; }
        public GameCache riverworldCache { get; set; }
        public PortingContextGen3 riverworld { get; set; }
        public GameCache salvationCache { get; set; }
        public PortingContextGen3 salvation { get; set; }
        public GameCache sandboxCache { get; set; }
        public PortingContextGen3 sandbox { get; set; }
        public GameCache shrineCache { get; set; }
        public PortingContextGen3 shrine { get; set; }
        public GameCache sidewinderCache { get; set; }
        public PortingContextGen3 sidewinder { get; set; }
        public GameCache snowboundCache { get; set; }
        public PortingContextGen3 snowbound { get; set; }
        public GameCache spacecampCache { get; set; }
        public PortingContextGen3 spacecamp { get; set; }
        public GameCache warehouseCache { get; set; }
        public PortingContextGen3 warehouse { get; set; }
        public GameCache zanzibarCache { get; set; }
        public PortingContextGen3 zanzibar { get; set; }

        public GameCache odstMainMenuCache { get; set; }
        public PortingContextGen3 odstMainMenu { get; set; }
        public GameCache h100Cache { get; set; }
        public PortingContextGen3 h100 { get; set; }
        public GameCache c100Cache { get; set; }
        public PortingContextGen3 c100 { get; set; }
        public GameCache c200Cache { get; set; }
        public PortingContextGen3 c200 { get; set; }
        public GameCache l200Cache { get; set; }
        public PortingContextGen3 l200 { get; set; }
        public GameCache l300Cache { get; set; }
        public PortingContextGen3 l300 { get; set; }
        public GameCache sc100Cache { get; set; }
        public PortingContextGen3 sc100 { get; set; }
        public GameCache sc110Cache { get; set; }
        public PortingContextGen3 sc110 { get; set; }
        public GameCache sc120Cache { get; set; }
        public PortingContextGen3 sc120 { get; set; }
        public GameCache sc130Cache { get; set; }
        public PortingContextGen3 sc130 { get; set; }
        public GameCache sc140Cache { get; set; }
        public PortingContextGen3 sc140 { get; set; }
        public GameCache sc150Cache { get; set; }
        public PortingContextGen3 sc150 { get; set; }

        public GenerateDonkeyCacheCommand(GameCache cache, PrecursorContextStack contextStack) : base
        (
            true,
            "GenerateDonkeyCache",
            "Generates a new cache for use with Managed Donkey",
            "GenerateDonkeyCache <Source Path> <Output Path>",
            GenerateHelpText()
        )
        {
            Cache = cache;
            ContextStack = contextStack;
            StopWatch = new Stopwatch();
        }

        private static string GenerateHelpText()
        {
            var buffer = new StringBuilder();

            // TODO: Add donkey specific help text

            return buffer.ToString();
        }

        public override object Execute(List<string> args)
        {
            StopWatch.Reset();

            if (args.Count > 2)
                return new PrecursorError($"Incorrect amount of arguments supplied");

            SourceDirectoryInfo = new DirectoryInfo(args[0]);
            OutputDirectoryInfo = new DirectoryInfo(args[1]);

            if (!SourceDirectoryInfo.Exists)
                return new PrecursorError("Source data path does not exist, or could not be found");

            if (!OutputDirectoryInfo.Exists)
                return new PrecursorError("Output path does not exist, or could not be found");

            if (OutputDirectoryInfo.FullName == Cache.Directory.FullName)
                return new PrecursorError("Output path cannot be the same as the current working directory");

            GetCacheFiles();

            StopWatch.Start();

            RebuildCache(OutputDirectoryInfo.FullName);
            RetargetCache(OutputDirectoryInfo.FullName);

            using (CacheStream = Cache.OpenCacheReadWrite())
            {
                PortTagData();
                UpdateTagData();
                UpdateMapData();
                //UpdateBlfData();
            }

            ContextStack.Pop();

            StopWatch.Stop();

            var output = StopWatch.ElapsedMilliseconds.FormatMilliseconds();
            var startColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Cache Generated Sucessfully. Time Taken: " + output);
            Console.ForegroundColor = startColor;
            return true;
        }

        public void GetCacheFiles()
        {
            haloOnlineDirectoryInfo = GetDirectoryInfo(haloOnlineDirectoryInfo, "Halo Online MS23");

            haloOnlineCache = GameCache.Open($@"{haloOnlineDirectoryInfo.FullName}\tags.dat");

            halo3DirectoryInfo = GetDirectoryInfo(halo3DirectoryInfo, "Halo 3");

            h3MainMenuCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\mainmenu.map");
            introCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\005_intro.map");
            jungleCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\010_jungle.map");
            crowsCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\020_base.map");
            outskirtsCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\030_outskirts.map");
            voiCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\040_voi.map");
            floodvoiCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\050_floodvoi.map");
            wasteCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\070_waste.map");
            citadelCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\100_citadel.map");
            highCharityCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\110_hc.map");
            haloCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\120_halo.map");
            epilogueCache = GameCache.Open($@"{halo3DirectoryInfo.FullName}\130_epilogue.map");

            halo3MythicDirectoryInfo = GetDirectoryInfo(halo3MythicDirectoryInfo, "Halo 3 Mythic");

            mythicMainMenuCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\mainmenu.map");
            armoryCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\armory.map");
            bunkerworldCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\bunkerworld.map");
            chillCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\chill.map");
            chilloutCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\chillout.map");
            constructCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\construct.map");
            cyberdyneCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\cyberdyne.map");
            deadlockCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\deadlock.map");
            descentCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\descent.map");
            docksCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\docks.map");
            fortressCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\fortress.map");
            ghosttownCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\ghosttown.map");
            guardianCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\guardian.map");
            isolationCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\isolation.map");
            lockoutCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\lockout.map");
            midshipCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\midship.map");
            riverworldCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\riverworld.map");
            salvationCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\salvation.map");
            sandboxCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\sandbox.map");
            shrineCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\shrine.map");
            sidewinderCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\sidewinder.map");
            snowboundCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\snowbound.map");
            spacecampCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\spacecamp.map");
            warehouseCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\warehouse.map");
            zanzibarCache = GameCache.Open($@"{halo3MythicDirectoryInfo.FullName}\zanzibar.map");

            halo3ODSTDirectoryInfo = GetDirectoryInfo(halo3ODSTDirectoryInfo, "Halo 3 ODST");

            odstMainMenuCache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\mainmenu.map");
            h100Cache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\h100.map");
            c100Cache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\c100.map");
            c200Cache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\c200.map");
            l200Cache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\l200.map");
            l300Cache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\l300.map");
            sc100Cache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\sc100.map");
            sc110Cache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\sc110.map");
            sc120Cache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\sc120.map");
            sc130Cache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\sc130.map");
            sc140Cache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\sc140.map");
            sc150Cache = GameCache.Open($@"{halo3ODSTDirectoryInfo.FullName}\sc150.map");
        }

        public DirectoryInfo GetDirectoryInfo(DirectoryInfo directoryInfo, string build)
        {
            Console.WriteLine("\nEnter the directory for your " + build + " cache files: ");
            var inputDirectory = Console.ReadLine().Replace("\"", "");
            directoryInfo = new DirectoryInfo(inputDirectory);

            if (!directoryInfo.Exists)
            {
                new PrecursorError($"Directory not found: '{directoryInfo.FullName}'");
                throw new ArgumentException();
            }

            if (directoryInfo.Exists && !directoryInfo.GetFiles().Any(x => x.FullName.EndsWith(".map")))
            {
                new PrecursorError($"No .map files found in '{directoryInfo.FullName}'");
                throw new ArgumentException();
            }

            return directoryInfo;
        }
    }
    */
}
