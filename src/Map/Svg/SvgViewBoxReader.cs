using System;
using System.Globalization;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Svg;

/// <summary>Reads the root <c>viewBox</c> from debrief map SVG markup (no full XML parse).</summary>
internal static class SvgViewBoxReader{
    private const string ViewBoxAttr = "viewBox=\"";

    public static bool TryRead(string svgMarkup, out Rect viewBox){
        viewBox = default;

        if(string.IsNullOrEmpty(svgMarkup)) return false;

        var start = svgMarkup.IndexOf(ViewBoxAttr, StringComparison.OrdinalIgnoreCase);

        if(start < 0) return false;

        start += ViewBoxAttr.Length;

        var end = svgMarkup.IndexOf('"', start);

        return end > start && TryParseQuad(svgMarkup.Substring(start, end - start), out viewBox);
    }

    private static bool TryParseQuad(string raw, out Rect viewBox){
        viewBox = default;

        if(string.IsNullOrWhiteSpace(raw)) return false;

        var parts = raw.Split([' ', '\t', '\r', '\n', ','], StringSplitOptions.RemoveEmptyEntries);

        if(parts.Length != 4) return false;

        if(!TryParseFloat(parts[0], out var x)) return false;

        if(!TryParseFloat(parts[1], out var y)) return false;

        if(!TryParseFloat(parts[2], out var w)) return false;

        if(!TryParseFloat(parts[3], out var h)) return false;

        if(w <= 0f || h <= 0f) return false;

        viewBox = new Rect(x, y, w, h);

        return true;
    }

    private static bool TryParseFloat(string text, out float value){
        return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
