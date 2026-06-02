using Newtonsoft.Json;
using PrecursorShell.JSON.Handlers;
using PrecursorShell.JSON.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TagTool.Cache;
using TagTool.Cache.HaloOnline;
using TagTool.Common;
using TagTool.Scripting.Compiler;
using TagTool.Tags;
using TagTool.Tags.Definitions;

namespace PrecursorShell.JSON.Parsers
{
    public class TagObjectParser
    {
        private GameCache Cache;
        private Stream CacheStream;
        private TagObjectHandler TagHandler;
        private MapObjectParser MapParser;
        private string InputPath;

        private static readonly Regex UnicodeRegex = new Regex(@"\\[uU]([0-9A-F]{4})", RegexOptions.Compiled);
        
        public TagObjectParser(GameCache cache, Stream cacheStream, string inputPath)
        {
            Cache = cache;
            CacheStream = cacheStream;
            InputPath = inputPath;
            TagHandler = new TagObjectHandler(Cache, CacheStream, this);
            MapParser = new MapObjectParser(Cache, CacheStream, InputPath);
        }

        public void ParseFile(string filePath)
        {
            if (Cache is GameCacheHaloOnline hoCache) 
            {
                var fullPath = $@"{InputPath}\tags\{filePath}.json";

                if (!File.Exists(fullPath))
                {
                    // Add error message?
                    return;
                }

                using var fileStream = File.OpenRead(fullPath);
                using var streamReader = new StreamReader(fileStream);
                using var jsonReader = new JsonTextReader(streamReader);
                var tagObject = TagHandler.Deserialize(jsonReader);

                hoCache.TagCache.TryGetTag($@"{tagObject.TagName}.{tagObject.TagType}", out var tag);

                if (tag == null)
                {
                    hoCache.TagCache.TryParseGroupTag(tagObject.TagType, out var tagGroup);
                    var type = hoCache.TagCache.TagDefinitions.GetTagDefinitionType(tagGroup);
                    tag = hoCache.TagCache.AllocateTag(type, tagObject.TagName);
                    var definition = (TagStructure)Activator.CreateInstance(type);
                    hoCache.Serialize(CacheStream, tag, definition);
                    hoCache.SaveTagNames();
                }

                switch (tagObject.TagData)
                {
                    case Bitmap:
                        ParseBitmapData(tagObject, tag);
                        break;
                    case ModelAnimationGraph:
                        ParseAnimationData(tagObject, tag);
                        break;
                    case ParticleModel:
                        ParseParticleModelData(tagObject, tag);
                        break;
                    case RenderModel:
                        ParseRenderModelData(tagObject, tag);
                        break;
                    case Scenario:
                        ParseScenarioData(tagObject, tag);
                        break;
                    case ScenarioLightmapBspData:
                        ParseScenarioLightmapBspData(tagObject, tag);
                        break;
                    case ScenarioStructureBsp:
                        ParseScenarioStructureBspData(tagObject, tag);
                        break;
                    case Sound:
                        ParseSoundData(tagObject, tag);
                        break;
                    case MultilingualUnicodeStringList:
                        ParseUnicodeStringData(tagObject, tag);
                        break;
                    default:
                        hoCache.Serialize(CacheStream, tag, tagObject.TagData);
                        break;
                }

                hoCache.SaveStrings();
            }
        }

        public void ParseBitmapData(TagObject tagObject, CachedTag tagInstance)
        {
            var tagData = tagObject.TagData as Bitmap;

            if (tagObject.Bitmaps != null) 
            {
                if (tagObject.Bitmaps.Textures != null)
                {
                    tagData.HardwareTextures = new List<TagResourceReference>();

                    for (int i = 0; i < tagObject.Bitmaps.Textures.Count; i++)
                    {
                        var texture = tagObject.Bitmaps.Textures[i];

                        if (texture != null) 
                        {
                            var reference = Cache.ResourceCache.CreateBitmapResource(texture);

                            tagData.HardwareTextures.Add(reference);
                        }
                    }
                }

                if (tagObject.Bitmaps.InterleavedTextures != null)
                {
                    tagData.InterleavedHardwareTextures = new List<TagResourceReference>();

                    for (int i = 0; i < tagObject.Bitmaps.InterleavedTextures.Count; i++)
                    {
                        var interleavedTexture = tagObject.Bitmaps.InterleavedTextures[i];

                        if (interleavedTexture != null) 
                        {
                            var reference = Cache.ResourceCache.CreateBitmapInterleavedResource(interleavedTexture);

                            tagData.InterleavedHardwareTextures.Add(reference);
                        }
                    }
                }
            }

            Cache.Serialize(CacheStream, tagInstance, tagData);
        }

        public void ParseAnimationData(TagObject tagObject, CachedTag tagInstance)
        {
            var tagData = tagObject.TagData as ModelAnimationGraph;

            if (tagObject.Animations != null)
            {
                for (int i = 0; i < tagObject.Animations.Animations.Count; i++)
                {
                    var animation = tagObject.Animations.Animations[i];

                    if (animation.GroupMembers != null)
                        animation.GroupMembers.AddressType = CacheAddressType.Definition;

                    var reference = Cache.ResourceCache.CreateModelAnimationGraphResource(animation);

                    tagData.ResourceGroups[i].ResourceReference = reference;
                }
            }

            Cache.Serialize(CacheStream, tagInstance, tagData);
        }

        public void ParseParticleModelData(TagObject tagObject, CachedTag tagInstance)
        {
            var tagData = tagObject.TagData as ParticleModel;

            if (tagObject.RenderGeometry != null)
            {
                var geometry = tagObject.RenderGeometry.Geometry;

                if (geometry != null) 
                {
                    if (geometry.VertexBuffers != null)
                        geometry.VertexBuffers.AddressType = CacheAddressType.Definition;

                    if (geometry.IndexBuffers != null)
                        geometry.IndexBuffers.AddressType = CacheAddressType.Definition;

                    var reference = Cache.ResourceCache.CreateRenderGeometryApiResource(geometry);

                    tagData.Geometry.Resource = reference;
                }
            }

            Cache.Serialize(CacheStream, tagInstance, tagData);
        }

        public void ParseRenderModelData(TagObject tagObject, CachedTag tagInstance)
        {
            var tagData = tagObject.TagData as RenderModel;

            if (tagObject.RenderGeometry != null)
            {
                var geometry = tagObject.RenderGeometry.Geometry;

                if (geometry != null) 
                {
                    if (geometry.VertexBuffers != null)
                        geometry.VertexBuffers.AddressType = CacheAddressType.Definition;

                    if (geometry.IndexBuffers != null)
                        geometry.IndexBuffers.AddressType = CacheAddressType.Definition;

                    var reference = Cache.ResourceCache.CreateRenderGeometryApiResource(geometry);
                    
                    tagData.Geometry.Resource = reference;
                }
            }

            Cache.Serialize(CacheStream, tagInstance, tagData);
        }

        public void ParseScenarioData(TagObject tagObject, CachedTag tagInstance)
        {
            var tagData = tagObject.TagData as Scenario;

            Cache.Serialize(CacheStream, tagInstance, tagData);

            if (tagObject.BlamScriptResource != null)
            {
                CompileScript(tagInstance, $@"{InputPath}\data\{tagObject.TagName}\scripts\{tagObject.BlamScriptResource.BlamScriptFile}");
            }

            var fileName = $@"{tagObject.TagName}\{tagObject.TagName.Split('\\').Last()}";

            MapParser.ParseFile(fileName);
        }

        public void ParseScenarioLightmapBspData(TagObject tagObject, CachedTag tagInstance)
        {
            var tagData = tagObject.TagData as ScenarioLightmapBspData;

            if (tagObject.RenderGeometry != null)
            {
                var geometry = tagObject.RenderGeometry.Geometry;

                if (geometry.VertexBuffers != null)
                    geometry.VertexBuffers.AddressType = CacheAddressType.Definition;

                if (geometry.IndexBuffers != null)
                    geometry.IndexBuffers.AddressType = CacheAddressType.Definition;

                var reference = Cache.ResourceCache.CreateRenderGeometryApiResource(geometry);

                tagData.Geometry.Resource = reference;
            }

            Cache.Serialize(CacheStream, tagInstance, tagData);
        }

        public void ParseScenarioStructureBspData(TagObject tagObject, CachedTag tagInstance)
        {
            var tagData = tagObject.TagData as ScenarioStructureBsp;

            if (tagObject.StructureBsp != null)
            {
                if (tagObject.StructureBsp.DecoratorGeometry != null)
                {
                    var decoratorGeometry = tagObject.StructureBsp.DecoratorGeometry;
            
                    if (decoratorGeometry.VertexBuffers != null)
                        decoratorGeometry.VertexBuffers.AddressType = CacheAddressType.Definition;
            
                    if (decoratorGeometry.IndexBuffers != null)
                        decoratorGeometry.IndexBuffers.AddressType = CacheAddressType.Definition;
            
                    var reference = Cache.ResourceCache.CreateRenderGeometryApiResource(decoratorGeometry);
            
                    tagData.DecoratorGeometry.Resource = reference;
                }
            
                if (tagObject.StructureBsp.Geometry != null)
                {
                    var geometry = tagObject.StructureBsp.Geometry;
            
                    if (geometry.VertexBuffers != null)
                        geometry.VertexBuffers.AddressType = CacheAddressType.Definition;
            
                    if (geometry.IndexBuffers != null)
                        geometry.IndexBuffers.AddressType = CacheAddressType.Definition;

                    var reference = Cache.ResourceCache.CreateRenderGeometryApiResource(geometry);
            
                    tagData.Geometry.Resource = reference;
                }
            
                if (tagObject.StructureBsp.Collision != null)
                {
                    var collision = tagObject.StructureBsp.Collision;

                    if (collision.CollisionBsps != null)
                    {
                        collision.CollisionBsps.AddressType = CacheAddressType.Definition;

                        foreach (var bsp in collision.CollisionBsps) 
                        {
                            if (bsp.Bsp3dNodes != null)
                                bsp.Bsp3dNodes.AddressType = CacheAddressType.Data;

                            if (bsp.Bsp3dSupernodes != null)
                                bsp.Bsp3dSupernodes.AddressType = CacheAddressType.Data;

                            if (bsp.Planes != null)
                                bsp.Planes.AddressType = CacheAddressType.Data;

                            if (bsp.Leaves != null)
                                bsp.Leaves.AddressType = CacheAddressType.Data;

                            if (bsp.Bsp2dReferences != null)
                                bsp.Bsp2dReferences.AddressType = CacheAddressType.Data;

                            if (bsp.Bsp2dNodes != null)
                                bsp.Bsp2dNodes.AddressType = CacheAddressType.Data;

                            if (bsp.Surfaces != null)
                                bsp.Surfaces.AddressType = CacheAddressType.Data;

                            if (bsp.Edges != null)
                                bsp.Edges.AddressType = CacheAddressType.Data;

                            if (bsp.Vertices != null)
                                bsp.Vertices.AddressType = CacheAddressType.Data;
                        }
                    }
            
                    if (collision.LargeCollisionBsps != null)
                    {
                        collision.LargeCollisionBsps.AddressType = CacheAddressType.Definition;

                        foreach (var bsp in collision.LargeCollisionBsps) 
                        {
                            if (bsp.Bsp3dNodes != null)
                                bsp.Bsp3dNodes.AddressType = CacheAddressType.Data;

                            if (bsp.Bsp3dSupernodes != null)
                                bsp.Bsp3dSupernodes.AddressType = CacheAddressType.Data;

                            if (bsp.Planes != null)
                                bsp.Planes.AddressType = CacheAddressType.Data;

                            if (bsp.Leaves != null)
                                bsp.Leaves.AddressType = CacheAddressType.Data;

                            if (bsp.Bsp2dReferences != null)
                                bsp.Bsp2dReferences.AddressType = CacheAddressType.Data;

                            if (bsp.Bsp2dNodes != null)
                                bsp.Bsp2dNodes.AddressType = CacheAddressType.Data;

                            if (bsp.Surfaces != null)
                                bsp.Surfaces.AddressType = CacheAddressType.Data;

                            if (bsp.Edges != null)
                                bsp.Edges.AddressType = CacheAddressType.Data;

                            if (bsp.Vertices != null)
                                bsp.Vertices.AddressType = CacheAddressType.Data;
                        }
                    }
            
                    if (collision.InstancedGeometry != null)
                    {
                        collision.InstancedGeometry.AddressType = CacheAddressType.Definition;

                        foreach (var geo in collision.InstancedGeometry) 
                        {
                            if (geo.CollisionInfo.Bsp3dNodes != null)
                                geo.CollisionInfo.Bsp3dNodes.AddressType = CacheAddressType.Data;

                            if (geo.CollisionInfo.Bsp3dSupernodes != null)
                                geo.CollisionInfo.Bsp3dSupernodes.AddressType = CacheAddressType.Data;

                            if (geo.CollisionInfo.Planes != null)
                                geo.CollisionInfo.Planes.AddressType = CacheAddressType.Data;

                            if (geo.CollisionInfo.Leaves != null)
                                geo.CollisionInfo.Leaves.AddressType = CacheAddressType.Data;

                            if (geo.CollisionInfo.Bsp2dReferences != null)
                                geo.CollisionInfo.Bsp2dReferences.AddressType = CacheAddressType.Data;

                            if (geo.CollisionInfo.Bsp2dNodes != null)
                                geo.CollisionInfo.Bsp2dNodes.AddressType = CacheAddressType.Data;

                            if (geo.CollisionInfo.Surfaces != null)
                                geo.CollisionInfo.Surfaces.AddressType = CacheAddressType.Data;

                            if (geo.CollisionInfo.Edges != null)
                                geo.CollisionInfo.Edges.AddressType = CacheAddressType.Data;

                            if (geo.CollisionInfo.Vertices != null)
                                geo.CollisionInfo.Vertices.AddressType = CacheAddressType.Data;

                            if (geo.RenderBsp != null)
                            {
                                geo.RenderBsp.AddressType = CacheAddressType.Definition;

                                foreach (var bsp in geo.RenderBsp) 
                                {
                                    if (bsp.Bsp3dNodes != null)
                                        bsp.Bsp3dNodes.AddressType = CacheAddressType.Data;

                                    if (bsp.Bsp3dSupernodes != null)
                                        bsp.Bsp3dSupernodes.AddressType = CacheAddressType.Data;

                                    if (bsp.Planes != null)
                                        bsp.Planes.AddressType = CacheAddressType.Data;

                                    if (bsp.Leaves != null)
                                        bsp.Leaves.AddressType = CacheAddressType.Data;

                                    if (bsp.Bsp2dReferences != null)
                                        bsp.Bsp2dReferences.AddressType = CacheAddressType.Data;

                                    if (bsp.Bsp2dNodes != null)
                                        bsp.Bsp2dNodes.AddressType = CacheAddressType.Data;

                                    if (bsp.Surfaces != null)
                                        bsp.Surfaces.AddressType = CacheAddressType.Data;

                                    if (bsp.Edges != null)
                                        bsp.Edges.AddressType = CacheAddressType.Data;

                                    if (bsp.Vertices != null)
                                        bsp.Vertices.AddressType = CacheAddressType.Data;
                                }
                            }

                            if (geo.CollisionMoppCodes != null) 
                            {
                                geo.CollisionMoppCodes.AddressType = CacheAddressType.Definition;

                                foreach (var moppCode in geo.CollisionMoppCodes)
                                {
                                    if (moppCode.Data != null)
                                        moppCode.Data.AddressType = CacheAddressType.Definition;
                                }
                            }

                            if (geo.BreakableSurfaces != null)
                                geo.BreakableSurfaces.AddressType = CacheAddressType.Definition;

                            if (geo.Polyhedra != null)
                                geo.Polyhedra.AddressType = CacheAddressType.Definition;

                            if (geo.PolyhedraFourVectors != null)
                                geo.PolyhedraFourVectors.AddressType = CacheAddressType.Definition;

                            if (geo.PolyhedraPlaneEquations != null)
                                geo.PolyhedraPlaneEquations.AddressType = CacheAddressType.Definition;

                            if (geo.SmallSurfacesPlanes != null)
                                geo.SmallSurfacesPlanes.AddressType = CacheAddressType.Definition;

                            if (geo.SurfacePlanes != null)
                                geo.SurfacePlanes.AddressType = CacheAddressType.Definition;

                            if (geo.Planes != null)
                                geo.Planes.AddressType = CacheAddressType.Definition;

                            if (geo.ExtraData != null) 
                            {
                                geo.ExtraData.AddressType = CacheAddressType.Definition;

                                foreach (var dataBlock in geo.ExtraData)
                                {
                                    if (dataBlock.Polyhedra != null)
                                        dataBlock.Polyhedra.AddressType = CacheAddressType.Data;

                                    if (dataBlock.PolyhedraFourVectors != null)
                                        dataBlock.PolyhedraFourVectors.AddressType = CacheAddressType.Data;

                                    if (dataBlock.PolyhedraPlaneEquations != null)
                                        dataBlock.PolyhedraPlaneEquations.AddressType = CacheAddressType.Data;
                                }
                            }

                            if (geo.MeshMopp != null) 
                            {
                                geo.MeshMopp.AddressType = CacheAddressType.Definition;

                                foreach (var meshMopp in geo.MeshMopp)
                                {
                                    if (meshMopp.Data != null)
                                        meshMopp.Data.AddressType = CacheAddressType.Definition;
                                }
                            }
                        }
                    }
            
                    if (collision.HavokData != null)
                    {
                        collision.HavokData.AddressType = CacheAddressType.Definition;

                        foreach (var dataBlock in collision.HavokData) 
                        {
                            dataBlock.HavokGeometries.AddressType = CacheAddressType.Definition;
                            dataBlock.HavokInvertedGeometries.AddressType = CacheAddressType.Definition;
                        }
                    }
            
                    var reference = Cache.ResourceCache.CreateStructureBspResource(collision);
            
                    tagData.CollisionBspResource = reference;
                }
            
                if (tagObject.StructureBsp.Pathfinding != null)
                {
                    var pathfinding = tagObject.StructureBsp.Pathfinding;

                    if (pathfinding.SurfacePlanes != null)
                        pathfinding.SurfacePlanes.AddressType = CacheAddressType.Data;

                    if (pathfinding.Planes != null)
                        pathfinding.Planes.AddressType = CacheAddressType.Data;

                    if (pathfinding.EdgeToSeams != null)
                        pathfinding.EdgeToSeams.AddressType = CacheAddressType.Data;

                    if (pathfinding.PathfindingData != null)
                        pathfinding.PathfindingData.AddressType = CacheAddressType.Definition;

                    var reference = Cache.ResourceCache.CreateStructureBspCacheFileResource(pathfinding);
                
                    tagData.PathfindingResource = reference;
                }
            }

            Cache.Serialize(CacheStream, tagInstance, tagData);
        }

        public void ParseSoundData(TagObject tagObject, CachedTag tagInstance)
        {
            var tagData = tagObject.TagData as Sound;

            if (tagObject.Sounds != null)
            {
                var sound = tagObject.Sounds.Sound;

                var reference = Cache.ResourceCache.CreateSoundResource(sound);

                tagData.Resource = reference;
            }

            Cache.Serialize(CacheStream, tagInstance, tagData);
        }

        public void ParseUnicodeStringData(TagObject tagObject, CachedTag tagInstance)
        {
            var tagDefinition = new MultilingualUnicodeStringList 
            {
                Strings = new List<LocalizedString>(),
                Data = [],
                OffsetCounts = new ushort[24],
            };

            if (tagObject.UnicodeStrings != null)
            {
                for (int i = 0; i < tagObject.UnicodeStrings.Languages[(int)GameLanguage.English].UnicodeStrings.Count; i++)
                {
                    foreach (GameLanguage language in Enum.GetValues(typeof(GameLanguage))) 
                    {
                        var unicodeString = tagObject.UnicodeStrings.Languages[(int)language].UnicodeStrings[i];

                        AddString(tagDefinition, language, unicodeString.StringIdName, unicodeString.StringIdContent);
                    }
                }
            }

            Cache.Serialize(CacheStream, tagInstance, tagDefinition);
        }

        public void AddString(MultilingualUnicodeStringList unic, GameLanguage language, string stringIdName, string stringIdContent)
        {
            var stringIdIndex = Cache.StringTable.IndexOf(stringIdName);

            if (stringIdIndex < 0)
            {
                Cache.StringTable.AddString(stringIdName);
                Cache.SaveStrings();

                stringIdIndex = Cache.StringTable.IndexOf(stringIdName);
            }

            var stringId = Cache.StringTable.GetStringId(stringIdIndex);

            var parsedContent = stringIdContent != null ? UnicodeRegex.Replace(stringIdContent, match => ((char)int.Parse(match.Value.Substring(2), NumberStyles.HexNumber)).ToString()) : null;

            var localizedStr = unic.Strings.FirstOrDefault(s => s.StringID == stringId);

            if (localizedStr == null)
            {
                localizedStr = new LocalizedString
                {
                    StringID = stringId,
                };

                unic.Strings.Add(localizedStr);
            }

            parsedContent = parsedContent != null ? DecodeNonAsciiCharacters(parsedContent) : null;

            SetLocalizedString(unic, localizedStr, language, parsedContent);
        }

        public static string DecodeNonAsciiCharacters(string value)
        {
            if (string.IsNullOrEmpty(value)) 
            {
                return value;
            }

            var sb = new StringBuilder(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '\\' && i + 1 < value.Length)
                {
                    char next = value[i + 1];
                    if (next == 'r') 
                    { 
                        sb.Append('\r'); i++; 
                    }
                    else if (next == 'n') 
                    { 
                        sb.Append('\n'); i++; 
                    }
                    else if (next == 't') 
                    { 
                        sb.Append('\t'); i++; 
                    }
                    else if (next == 'u' || next == 'U')
                    {
                        if (i + 5 < value.Length)
                        {
                            if (int.TryParse(value.AsSpan(i + 2, 4), NumberStyles.HexNumber, null, out int unicodeValue))
                            {
                                sb.Append((char)unicodeValue);
                                i += 5;
                                continue;
                            }
                        }
                        sb.Append(value[i]);
                    }
                    else
                    {
                        sb.Append(value[i]);
                    }
                }
                else
                {
                    sb.Append(value[i]);
                }
            }

            return sb.ToString();
        }

        public static void SetLocalizedString(MultilingualUnicodeStringList unic, LocalizedString str, GameLanguage language, string newValue) 
        {
            var offset = str.Offsets[(int)language];

            if (offset < 0 || offset >= unic.Data.Length)
                offset = unic.Data.Length;

            var endOffset = offset;

            while (endOffset < unic.Data.Length && unic.Data[endOffset] != 0)
                endOffset++;

            var oldLength = endOffset - offset;

            var bytes = (newValue != null) ? Encoding.UTF8.GetBytes(newValue) : new byte[0];

            if (bytes.Length > 0 || newValue == string.Empty && offset == unic.Data.Length)
            {
                var nullTerminated = new byte[bytes.Length + 1];
                Buffer.BlockCopy(bytes, 0, nullTerminated, 0, bytes.Length);
                bytes = nullTerminated;
            }

            unic.Data = ArrayUtil.Replace(unic.Data, offset, oldLength, bytes);

            str.Offsets[(int)language] = (bytes.Length > 0) ? offset : -1;

            var sizeDelta = bytes.Length - oldLength;

            foreach (var adjustStr in unic.Strings)
            {
                for (var i = 0; i < adjustStr.Offsets.Length; i++)
                {
                    if (adjustStr.Offsets[i] > offset)
                        adjustStr.Offsets[i] += sizeDelta;
                }
            }
        }

        public void CompileScript(CachedTag tag, string scriptFile)
        {
            var scriptData = new FileInfo(scriptFile);

            var scnr = Cache.Deserialize<Scenario>(CacheStream, tag);

            ScriptCompiler scriptCompiler = new ScriptCompiler(Cache, scnr);

            scriptCompiler.CompileFile(scriptData);

            Cache.Serialize(CacheStream, tag, scnr);
        }
    }
}
