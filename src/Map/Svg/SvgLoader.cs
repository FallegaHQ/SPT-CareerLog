using System;
using System.Collections.Generic;
using System.IO;
using Unity.VectorGraphics;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Svg;

/// <summary>Cached access to debrief map SVG sprites.</summary>
internal static class SvgLoader{
    private static readonly Dictionary<string, Sprite> Cache = new(StringComparer.OrdinalIgnoreCase);

    public static Sprite LoadSprite(string svgAbsolutePath){
        if(string.IsNullOrEmpty(svgAbsolutePath)) return null;

        if(Cache.TryGetValue(svgAbsolutePath, out var cached)) return cached;

        if(!File.Exists(svgAbsolutePath)){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Map SVG missing: {svgAbsolutePath}"));

            return null;
        }

        try{
            var sprite = FromAbsolutePath(svgAbsolutePath);

            if(sprite) Cache[svgAbsolutePath] = sprite;

            return sprite;
        }
        catch(Exception ex){
            CareerLogPlugin.Log?.LogWarning(
                                            PluginInfo.Format($"Map SVG load failed ({svgAbsolutePath}): {ex.Message}")
                                           );

            return null;
        }
    }

    /// <summary>Reads a debrief SVG file and produces a map sprite via VectorGraphics tessellation.</summary>
    private static Sprite FromAbsolutePath(string absolutePath){
        if(string.IsNullOrEmpty(absolutePath) || !File.Exists(absolutePath)) return null;

        var markup = File.ReadAllText(absolutePath);

        if(!SvgViewBoxReader.TryRead(markup, out var viewBox)){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Map SVG has no usable viewBox: {absolutePath}"));

            return null;
        }

        using var reader   = new StringReader(markup);
        var       imported = SVGParser.ImportSVG(reader, ViewportOptions.OnlyApplyRootViewBox);

        return SvgQualityLadder.Rasterize(imported.Scene, imported.NodeOpacity, viewBox, absolutePath);
    }
}
