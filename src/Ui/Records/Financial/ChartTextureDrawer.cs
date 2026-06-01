using Softwyx.CareerLog.Persistence.Financial;
using Softwyx.CareerLog.Ui.Design;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal static class ChartTextureDrawer{
    private const int DefaultWidth  = 2048;
    private const int DefaultHeight = 768;

    private const int   PadLeft        = 128;
    private const int   PadRight       = 128;
    private const int   PadTop         = 24;
    private const int   PadBottom      = 48;
    private const float LineWidth      = 2f;
    private const float GridLineWidth  = 4f;
    private const float PointRadius    = 6f;
    private const float PointHighlight = 8f;
    private const float BarWidthFactor = 0.32f;

    public static (Texture2D texture, ChartPlotLayout layout) Draw(
        IReadOnlyList<ChartPoint> points, ChartStyle style, int highlightIndex = -1
    ){
        var texture = new Texture2D(DefaultWidth, DefaultHeight, TextureFormat.RGBA32, false){
                          filterMode = FilterMode.Bilinear,
                          wrapMode   = TextureWrapMode.Clamp
                      };

        var pixels = new Color32[DefaultWidth * DefaultHeight];

        for(var i = 0; i < pixels.Length; i++) pixels[i] = Colors.Chart.Background;

        if(points == null || points.Count == 0){
            texture.SetPixels32(pixels);
            texture.Apply();

            return (texture,
                    new ChartPlotLayout(DefaultWidth, DefaultHeight, PadLeft, PadRight, PadTop, PadBottom, 0, 1, 0));
        }

        const long minWorth = 0L;
        var        maxWorth = points[0].TotalWorth;

        foreach(var point in points)
            if(point.TotalWorth > maxWorth)
                maxWorth = point.TotalWorth;

        if(maxWorth <= 0L) maxWorth = 1L;

        var displayMax = ChartPlotLayout.ComputeDisplayMax(maxWorth);

        var layout = new ChartPlotLayout(
                                         DefaultWidth,
                                         DefaultHeight,
                                         PadLeft,
                                         PadRight,
                                         PadTop,
                                         PadBottom,
                                         minWorth,
                                         displayMax,
                                         points.Count
                                        );

        DrawGrid(pixels, layout);

        if(style == ChartStyle.Bar)
            DrawBars(pixels, points, layout, highlightIndex);
        else
            DrawLineSeries(pixels, points, layout, highlightIndex);

        texture.SetPixels32(pixels);
        texture.Apply();

        return (texture, layout);
    }

    private static void DrawGrid(Color32[] pixels, ChartPlotLayout layout){
        for(var i = 0; i <= 4; i++){
            var y = layout.PadBottom                                + layout.PlotHeight * i / 4f;
            DrawHorizontalLine(pixels, layout.PadLeft, layout.Width - layout.PadRight, y, Colors.Chart.GridLine);
        }

        if(layout.PointCount <= 0) return;

        for(var i = 0; i < layout.PointCount; i++){
            var x = layout.MapIndexToX(i);
            DrawVerticalLine(pixels, x, layout.PadBottom, layout.Height - layout.PadTop, Colors.Chart.GridLineV);
        }
    }

    private static void DrawLineSeries(
        Color32[] pixels, IReadOnlyList<ChartPoint> points, ChartPlotLayout layout, int highlightIndex
    ){
        if(points.Count == 1){
            var p = layout.MapPoint(points[0], 0);
            DrawDisc(pixels, p.x, p.y, PointHighlight, Colors.Chart.Point);

            return;
        }

        for(var i = 1; i < points.Count; i++){
            var a = layout.MapPoint(points[i - 1], i - 1);
            var b = layout.MapPoint(points[i],     i);
            DrawLine(pixels, a, b, LineWidth, Colors.Chart.Line);
        }

        for(var i = 0; i < points.Count; i++){
            var p     = layout.MapPoint(points[i], i);
            var color = i == highlightIndex ? Colors.Chart.Point : Colors.Chart.Line;
            var size  = i == highlightIndex ? PointHighlight : PointRadius;
            DrawDisc(pixels, p.x, p.y, size, color);
        }
    }

    private static void DrawBars(
        Color32[] pixels, IReadOnlyList<ChartPoint> points, ChartPlotLayout layout, int highlightIndex
    ){
        var barWidth = Mathf.Max(4f, layout.PlotWidth / (float) Math.Max(1, points.Count) * BarWidthFactor);

        for(var i = 0; i < points.Count; i++){
            var top    = layout.MapPoint(points[i], i);
            var bottom = new Vector2(top.x, layout.PadBottom);
            var half   = barWidth * 0.5f;
            var color  = i == highlightIndex ? Colors.Chart.Point : Colors.Chart.Bar;

            DrawVerticalBar(pixels, top.x - half, top.x + half, bottom.y, top.y, color);
        }
    }

    private static void DrawHorizontalLine(Color32[] pixels, int x0, int x1, float y, Color32 color){
        var half = GridLineWidth * 0.5f;
        var yLo  = Mathf.Clamp(Mathf.FloorToInt(y - half), 0, DefaultHeight - 1);
        var yHi  = Mathf.Clamp(Mathf.CeilToInt(y  + half), 0, DefaultHeight - 1);

        for(var yi = yLo; yi <= yHi; yi++)
            for(var x = x0; x < x1; x++){
                if(x is < 0 or >= DefaultWidth) continue;

                pixels[yi * DefaultWidth + x] = color;
            }
    }

    private static void DrawVerticalLine(Color32[] pixels, float x, float y0, float y1, Color32 color){
        var half = GridLineWidth * 0.5f;
        var xLo  = Mathf.Clamp(Mathf.FloorToInt(x - half),          0, DefaultWidth  - 1);
        var xHi  = Mathf.Clamp(Mathf.CeilToInt(x  + half),          0, DefaultWidth  - 1);
        var lo   = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(y0, y1)), 0, DefaultHeight - 1);
        var hi   = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(y0, y1)),  0, DefaultHeight - 1);

        for(var xi = xLo; xi <= xHi; xi++)
            for(var y = lo; y <= hi; y++)
                pixels[y * DefaultWidth + xi] = color;
    }

    private static void DrawLine(Color32[] pixels, Vector2 a, Vector2 b, float width, Color32 color){
        var steps = Mathf.CeilToInt(Vector2.Distance(a, b));

        for(var i = 0; i <= steps; i++){
            var t = steps == 0 ? 0f : i / (float) steps;
            var p = Vector2.Lerp(a, b, t);
            DrawDisc(pixels, p.x, p.y, width, color);
        }
    }

    private static void DrawVerticalBar(Color32[] pixels, float x0, float x1, float y0, float y1, Color32 color){
        var left   = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(x0, x1)), 0, DefaultWidth  - 1);
        var right  = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(x0, x1)),  0, DefaultWidth  - 1);
        var bottom = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(y0, y1)), 0, DefaultHeight - 1);
        var top    = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(y0, y1)),  0, DefaultHeight - 1);

        for(var y = bottom; y <= top; y++)
            for(var x = left; x <= right; x++)
                pixels[y * DefaultWidth + x] = color;
    }

    private static void DrawDisc(Color32[] pixels, float cx, float cy, float radius, Color32 color){
        var r2   = radius * radius;
        var minX = Mathf.Clamp(Mathf.FloorToInt(cx - radius), 0, DefaultWidth  - 1);
        var maxX = Mathf.Clamp(Mathf.CeilToInt(cx  + radius), 0, DefaultWidth  - 1);
        var minY = Mathf.Clamp(Mathf.FloorToInt(cy - radius), 0, DefaultHeight - 1);
        var maxY = Mathf.Clamp(Mathf.CeilToInt(cy  + radius), 0, DefaultHeight - 1);

        for(var y = minY; y <= maxY; y++)
            for(var x = minX; x <= maxX; x++){
                var dx = x - cx;
                var dy = y - cy;

                if(dx * dx + dy * dy <= r2) pixels[y * DefaultWidth + x] = color;
            }
    }
}
