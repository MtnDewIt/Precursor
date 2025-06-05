using HaloShaderGenerator.Globals;
using System.Collections.Generic;
using System.IO;
using TagTool.Cache.HaloOnline;
using TagTool.Cache;
using TagTool.Shaders.ShaderGenerator;
using Precursor.Tags.Definitions.ElDewritoDebug;

namespace Precursor.Commands.ConvertCache
{
    public class ConvertCacheCommand : PrecursorCommand
    {
        public GameCache Cache { get; set; }
        public GameCacheHaloOnlineBase CacheContext { get; set; }

        public ConvertCacheCommand(GameCache cache, GameCacheHaloOnlineBase cacheContext) : base
        (
            true,
            "ConvertCache",
            "[REDACTED]",
            "ConvertCache",
            "[REDACTED]"
        )
        {
            Cache = cache;
            CacheContext = cacheContext;
        }

        public override object Execute(List<string> args) 
        {
            if (Cache is GameCacheHaloOnline)
            {
                using (Stream stream = CacheContext.OpenCacheReadWrite())
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

        public void ConvertTagData(GameCache cache, Stream stream) 
        {
            foreach (CachedTag tag in cache.TagCache.NonNull())
            {
                if (tag.IsInGroup("sefc"))
                    ConvertAreaScreenEffect(stream, tag);

                if (tag.IsInGroup("forg"))
                    ConvertForgeGlobalsDefintion(stream, tag);

                if (tag.IsInGroup("modg"))
                    ConvertModGlobalsDefinition(stream, tag);

                if (tag.IsInGroup("pact"))
                    ConvertPlayerActionSet(stream, tag);

                // TODO: Add something to revert the mp_wake_script script operator in the scenario
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
            TagTool.Tags.Definitions.AreaScreenEffect definition = CacheContext.Deserialize<TagTool.Tags.Definitions.AreaScreenEffect>(stream, tag);

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

            CacheContext.Serialize(stream, tag, adapter);
        }

        public void ConvertForgeGlobalsDefintion(Stream stream, CachedTag tag) 
        {
            ForgeGlobalsDefinition definition = CacheContext.Deserialize<ForgeGlobalsDefinition>(stream, tag);

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

            CacheContext.Serialize(stream, tag, adapter);
        }

        public void ConvertModGlobalsDefinition(Stream stream, CachedTag tag) 
        {
            ModGlobalsDefinition definition = CacheContext.Deserialize<ModGlobalsDefinition>(stream, tag);

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

            CacheContext.Serialize(stream, tag, adapter);
        }

        public void ConvertPlayerActionSet(Stream stream, CachedTag tag) 
        {
            TagTool.Tags.Definitions.PlayerActionSet definition = CacheContext.Deserialize<TagTool.Tags.Definitions.PlayerActionSet>(stream, tag);

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

            CacheContext.Serialize(stream, tag, adapter);
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