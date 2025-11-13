using HaloShaderGenerator.Globals;
using PrecursorShell.Common;
using PrecursorShell.Tags.Definitions.ElDewritoDebug;
using System;
using System.Collections.Generic;
using System.IO;
using TagTool.Cache;
using TagTool.Cache.HaloOnline;
using TagTool.Shaders.ShaderGenerator;

namespace PrecursorShell.Commands.ConvertCache
{
    public class ConvertCacheCommand : PrecursorCommand
    {
        public enum ElDewritoBuild
        {
            None = 0,
            ED061745,
        }

        public GameCache Cache { get; set; }

        public ConvertCacheCommand() : base
        (
            true,
            "ConvertCache",
            "Converts the specified cache so that it functions in older eldewrito builds",
            "ConvertCache <Cache Path> <Build>",
            "Converts the specified cache so that it functions in older eldewrito builds\n\n" +
            "WARNING: If you are converting mod packages, esnure they are placed in the mods folder of\n" +
            "you eldewrito install, as the command will pull the base cache path from the maps folder"
        )
        {
        }

        public override object Execute(List<string> args) 
        {
            if (args.Count != 2)
                return new PrecursorError($"Incorrect amount of arguments supplied!");

            var file = new FileInfo(args[0]);

            if (!file.Exists)
                return new PrecursorError($"The specified file could not be found! \"{file.Name}\"");

            if (!Enum.TryParse(args[1], true, out ElDewritoBuild build))
                return new PrecursorError($"Invalid Build!");

            ParseInputPath(file);

            if (Cache is GameCacheHaloOnline)
            {
                using (Stream stream = Cache.OpenCacheReadWrite())
                {
                    ConvertTagData(Cache, stream);
                }
            }
            else if (Cache is GameCacheModPackage modCache)
            {
                for (int i = 0; i < modCache.BaseModPackage.GetTagCacheCount(); i++)
                {
                    modCache.SetActiveTagCache(i);

                    using (Stream tagCacheStream = modCache.OpenCacheReadWrite())
                    {
                        ConvertTagData(modCache, tagCacheStream);
                    }
                }
            }

            return true;
        }

        public void ParseInputPath(FileInfo file) 
        {
            if (file.Name.EndsWith(".dat")) 
            {
                Cache = GameCache.Open(file);
            }
            else if (file.Name.EndsWith(".pak"))
            {
                var basePath = file.Directory.FullName.Replace("mods", "");

                var baseCache = GameCache.Open($@"{basePath}maps\tags.dat");

                Cache = new GameCacheModPackage(baseCache as GameCacheHaloOnline, file);
            }
        }

        public void ConvertTagData(GameCache cache, Stream stream) 
        {
            foreach (CachedTag tag in cache.TagCache.NonNull())
            {
                if (!(tag as CachedTagHaloOnline).IsEmpty()) 
                {
                    if (tag.IsInGroup("sefc"))
                        ConvertAreaScreenEffect(stream, tag);

                    if (tag.IsInGroup("forg"))
                        ConvertForgeGlobalsDefintion(stream, tag);

                    if (tag.IsInGroup("modg"))
                        ConvertModGlobalsDefinition(stream, tag);

                    if (tag.IsInGroup("pact"))
                        ConvertPlayerActionSet(stream, tag);

                    if (tag.IsInGroup("scnr"))
                        ConvertScenario(stream, tag);
                }
            }

            GenerateExplicitShader(stream, ExplicitShader.final_composite, false);
            GenerateExplicitShader(stream, ExplicitShader.final_composite_debug, false);
            GenerateExplicitShader(stream, ExplicitShader.final_composite_dof, false);
            GenerateExplicitShader(stream, ExplicitShader.final_composite_zoom, false);
            GenerateExplicitShader(stream, ExplicitShader.shadow_apply_point, false);
            GenerateExplicitShader(stream, ExplicitShader.shadow_apply_bilinear, false);
            GenerateExplicitShader(stream, ExplicitShader.shadow_apply_fancy, false);
            GenerateExplicitShader(stream, ExplicitShader.shadow_apply_faster, false);
            GenerateExplicitShader(stream, ExplicitShader.shadow_apply, false);
        }

        public void ConvertAreaScreenEffect(Stream stream, CachedTag tag) 
        {
            TagTool.Tags.Definitions.AreaScreenEffect definition = Cache.Deserialize<TagTool.Tags.Definitions.AreaScreenEffect>(stream, tag);

            AreaScreenEffect adapter = new AreaScreenEffect 
            {
                ScreenEffects = new List<AreaScreenEffect.ScreenEffectBlock>(),
            };

            for (int i = 0; i < definition.ScreenEffects.Count; i++) 
            {
                TagTool.Tags.Definitions.AreaScreenEffect.ScreenEffectBlock screenEffect = definition.ScreenEffects[i];

                adapter.ScreenEffects.Add(new AreaScreenEffect.ScreenEffectBlock 
                {
                    Name = screenEffect.Name,
                    Flags = screenEffect.Flags,
                    HiddenFlags = screenEffect.HiddenFlags,
                    MaximumDistance = screenEffect.MaximumDistance,
                    DistanceFalloffFunction = screenEffect.DistanceFalloffFunction,
                    Lifetime = screenEffect.Lifetime,
                    TimeEvolutionFunction = screenEffect.TimeEvolutionFunction,
                    AngleFalloffFunction = screenEffect.AngleFalloffFunction,
                    ExposureBoost = screenEffect.ExposureBoost,
                    HueLeft = screenEffect.HueLeft,
                    HueRight = screenEffect.HueRight,
                    Saturation = screenEffect.Saturation,
                    Desaturation = screenEffect.Desaturation,
                    ContrastEnhance = screenEffect.ContrastEnhance,
                    GammaEnhance = screenEffect.GammaEnhance,
                    GammaReduce = screenEffect.GammaReduce,
                    ColorFilter = screenEffect.ColorFilter,
                    ColorFloor = screenEffect.ColorFloor,
                    VisionMode = screenEffect.VisionMode,
                    VisionNoise = screenEffect.VisionNoise,
                    ScreenShader = screenEffect.ScreenShader,
                });
            }

            Cache.Serialize(stream, tag, adapter);
        }

        public void ConvertForgeGlobalsDefintion(Stream stream, CachedTag tag) 
        {
            ForgeGlobalsDefinition definition = Cache.Deserialize<ForgeGlobalsDefinition>(stream, tag);

            ForgeGlobalsDefinition adapter = new ForgeGlobalsDefinition 
            {
                InvisibleRenderMethod = definition.InvisibleRenderMethod,
                DefaultRenderMethod = definition.DefaultRenderMethod,
                ReForgeMaterials = definition.ReForgeMaterials,
                ReForgeMaterialTypes = definition.ReForgeMaterialTypes,
                ReForgeObjects = definition.ReForgeObjects,
                PrematchCameraObject = definition.PrematchCameraObject,
                ModifierObject = definition.ModifierObject,
                KillVolumeObject = definition.KillVolumeObject,
                GarbageVolumeObject = definition.GarbageVolumeObject,
                Descriptions = definition.Descriptions,
                PaletteCategories = definition.PaletteCategories,
                Palette = definition.Palette,
                WeatherEffects = definition.WeatherEffects,
                Skies = definition.Skies,
                FxObject = definition.FxObject,
                FxLight = definition.FxLight,
            };

            Cache.Serialize(stream, tag, adapter);
        }

        public void ConvertModGlobalsDefinition(Stream stream, CachedTag tag) 
        {
            ModGlobalsDefinition definition = Cache.Deserialize<ModGlobalsDefinition>(stream, tag);

            ModGlobalsDefinition adapter = new ModGlobalsDefinition 
            {
                PlayerCharacterSets = new List<ModGlobalsDefinition.PlayerCharacterSet>(),
                PlayerCharacterCustomizations = new List<ModGlobalsDefinition.PlayerCharacterCustomization>(),
            };

            for (int i = 0; i < definition.PlayerCharacterSets.Count; i++) 
            {
                ModGlobalsDefinition.PlayerCharacterSet characterSet = definition.PlayerCharacterSets[i];

                adapter.PlayerCharacterSets.Add(new ModGlobalsDefinition.PlayerCharacterSet 
                {
                    DisplayName = characterSet.DisplayName,
                    Name = characterSet.Name,
                    RandomChance = characterSet.RandomChance,
                    Characters = characterSet.Characters,
                });
            }

            for (int i = 0; i < definition.PlayerCharacterCustomizations.Count; i++) 
            {
                ModGlobalsDefinition.PlayerCharacterCustomization customization = definition.PlayerCharacterCustomizations[i];

                adapter.PlayerCharacterCustomizations.Add(new ModGlobalsDefinition.PlayerCharacterCustomization());

                adapter.PlayerCharacterCustomizations[i].GlobalPlayerCharacterTypeIndex = customization.GlobalPlayerCharacterTypeIndex;
                adapter.PlayerCharacterCustomizations[i].CharacterName = customization.CharacterName;
                adapter.PlayerCharacterCustomizations[i].CharacterDescription = customization.CharacterDescription;
                adapter.PlayerCharacterCustomizations[i].HudGlobals = customization.HudGlobals;
                adapter.PlayerCharacterCustomizations[i].VisionGlobals = customization.VisionGlobals;
                adapter.PlayerCharacterCustomizations[i].ActionSet = customization.ActionSet;

                adapter.PlayerCharacterCustomizations[i].RegionCameraScripts = new List<ModGlobalsDefinition.PlayerCharacterCustomization.PlayerCharacterRegionScript>();

                for (int j = 0; j < customization.RegionCameraScripts.Count; j++) 
                {
                    ModGlobalsDefinition.PlayerCharacterCustomization.PlayerCharacterRegionScript cameraRegion = customization.RegionCameraScripts[j];

                    adapter.PlayerCharacterCustomizations[i].RegionCameraScripts.Add(new ModGlobalsDefinition.PlayerCharacterCustomization.PlayerCharacterRegionScript 
                    {
                        RegionName = cameraRegion.RegionName,
                        ScriptNameWidescreen = cameraRegion.ScriptNameWidescreen,
                        ScriptNameStandard = cameraRegion.ScriptNameStandard,
                        BipedRotation = cameraRegion.BipedRotation,
                        RotationDuration = cameraRegion.RotationDuration,
                    });
                }

                adapter.PlayerCharacterCustomizations[i].CharacterPositionData = new ModGlobalsDefinition.PlayerCharacterCustomization.CharacterPositionInfo 
                {
                    Flags = customization.CharacterPositionData.Flags,
                    BipedNameIndex = customization.CharacterPositionData.BipedNameIndex,
                    SettingsCameraIndex = customization.CharacterPositionData.SettingsCameraIndex,
                    PlatformNameIndex = customization.CharacterPositionData.PlatformNameIndex,
                    RelativeBipedPosition = customization.CharacterPositionData.RelativeBipedPosition,
                    RelativeBipedRotation = customization.CharacterPositionData.RelativeBipedRotation,
                    BipedPositionWidescreen = customization.CharacterPositionData.BipedPositionWidescreen,
                    BipedPositionStandard = customization.CharacterPositionData.BipedPositionStandard,
                    BipedRotation = customization.CharacterPositionData.BipedRotation,
                };

                adapter.PlayerCharacterCustomizations[i].CharacterColors = customization.CharacterColors;
            }

            Cache.Serialize(stream, tag, adapter);
        }

        public void ConvertPlayerActionSet(Stream stream, CachedTag tag) 
        {
            TagTool.Tags.Definitions.PlayerActionSet definition = Cache.Deserialize<TagTool.Tags.Definitions.PlayerActionSet>(stream, tag);

            PlayerActionSet adapter = new PlayerActionSet 
            {
                Widget = new PlayerActionSet.WidgetData
                {
                    // Don't really know what to do if the definition contains more than 1 widget :/
                    Title = definition.Widget[0].Title,
                    Type = definition.Widget[0].Type,
                    Flags = definition.Widget[0].Flags,
                },
                Actions = new List<PlayerActionSet.Action>(),
            };

            for (int i = 0; i < definition.Actions.Count; i++) 
            {
                TagTool.Tags.Definitions.PlayerActionSet.Action action = definition.Actions[i];

                adapter.Actions.Add(new PlayerActionSet.Action 
                {
                    Title = action.Title,
                    IconName = action.IconName,
                    AnimationEnter = action.AnimationEnter,
                    AnimationIdle = action.AnimationIdle,
                    AnimationExit = action.AnimationExit,
                    Flags = action.Flags,
                    OverrideCamera = action.OverrideCamera,
                });
            }

            Cache.Serialize(stream, tag, adapter);
        }

        public void ConvertScenario(Stream stream, CachedTag tag) 
        {
            // TODO: Add something to revert the mp_wake_script script operator in the scenario
        }

        public void GenerateExplicitShader(Stream stream, ExplicitShader shader, bool applyFixes = false)
        {
            CachedTag pixlTag = Cache.TagCache.GetTag<TagTool.Tags.Definitions.PixelShader>($"rasterizer\\shaders\\{shader}") ?? Cache.TagCache.AllocateTag<TagTool.Tags.Definitions.PixelShader>($"rasterizer\\shaders\\{shader}");
            CachedTag vtshTag = Cache.TagCache.GetTag<TagTool.Tags.Definitions.VertexShader>($"rasterizer\\shaders\\{shader}") ?? Cache.TagCache.AllocateTag<TagTool.Tags.Definitions.VertexShader>($"rasterizer\\shaders\\{shader}");

            ShaderGeneratorNew.GenerateExplicitShader(Cache, stream, shader.ToString(), applyFixes, out TagTool.Tags.Definitions.PixelShader pixl, out TagTool.Tags.Definitions.VertexShader vtsh);

            Cache.Serialize(stream, vtshTag, vtsh);
            Cache.Serialize(stream, pixlTag, pixl);
        }
    }
}