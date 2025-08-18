using System.Collections.Generic;
using TagTool.Cache;
using TagTool.Common;
using TagTool.Tags;

namespace PrecursorShell.Tags.Definitions.ElDewritoDebug
{
    [TagStructure(Name = "forge_globals_definition", Tag = "forg", Size = 0xE0)]
    public class ForgeGlobalsDefinition : TagStructure
    {
        [TagField(ValidTags = new[] { "rm  " })]
        public CachedTag InvisibleRenderMethod;
        [TagField(ValidTags = new[] { "rm  " })]
        public CachedTag DefaultRenderMethod;

        public List<TagTool.Tags.Definitions.ForgeGlobalsDefinition.ReForgeMaterial> ReForgeMaterials;
        public List<TagTool.Tags.Definitions.ForgeGlobalsDefinition.ReForgeMaterialType> ReForgeMaterialTypes;
        public List<TagReferenceBlock> ReForgeObjects;

        [TagField(ValidTags = new[] { "obje" })]
        public CachedTag PrematchCameraObject;
        [TagField(ValidTags = new[] { "obje" })]
        public CachedTag ModifierObject;
        [TagField(ValidTags = new[] { "obje" })]
        public CachedTag KillVolumeObject;
        [TagField(ValidTags = new[] { "obje" })]
        public CachedTag GarbageVolumeObject;

        public List<TagTool.Tags.Definitions.ForgeGlobalsDefinition.Description> Descriptions;
        public List<TagTool.Tags.Definitions.ForgeGlobalsDefinition.PaletteCategory> PaletteCategories;
        public List<TagTool.Tags.Definitions.ForgeGlobalsDefinition.PaletteItem> Palette;
        public List<TagTool.Tags.Definitions.ForgeGlobalsDefinition.WeatherEffect> WeatherEffects;
        public List<TagTool.Tags.Definitions.ForgeGlobalsDefinition.Sky> Skies;

        public CachedTag FxObject;
        public CachedTag FxLight;
    }
}
