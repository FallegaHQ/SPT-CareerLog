using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Ui.Design;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Trail;

internal static class TrailTextureDrawer{
    private const int   TextureSize    = 2048;
    private const float BaseLineWidth  = 8f;
    private const float MinLineWidth   = 0.525f;
    private const float MaxLineWidth   = 14f;
    private const float StampRadiusMul = 0.55f;
    private const float StampStepMul   = 0.6f;

    public static float LineWidthForScale(float baselineCombinedScale, float currentCombinedScale){
        if(currentCombinedScale < 0.001f) return BaseLineWidth;

        if(baselineCombinedScale < 0.001f) baselineCombinedScale = currentCombinedScale;

        var width = BaseLineWidth * (baselineCombinedScale / currentCombinedScale);

        return Mathf.Clamp(width, MinLineWidth, MaxLineWidth);
    }

    public static Texture2D CreateOverlayTexture(){
        var texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false){
                          filterMode = FilterMode.Bilinear,
                          wrapMode   = TextureWrapMode.Clamp
                      };

        Clear(texture);

        return texture;
    }

    public static void Clear(Texture2D texture){
        if(!texture) return;

        var clear = new Color32[texture.width * texture.height];
        texture.SetPixels32(clear);
        texture.Apply();
    }

    public static void DrawTrail(
        Texture2D              texture, LocationDefinition definition, IReadOnlyList<Vector2> mapPoints,
        RaidTrailPlaybackState state,   float              lineWidth = BaseLineWidth
    ){
        if(!texture || definition == null || mapPoints == null || mapPoints.Count == 0 || state.IsEmpty){
            Clear(texture);

            return;
        }

        var pixels = texture.GetPixels32();
        Array.Clear(pixels, 0, pixels.Length);

        var committedEnd = Mathf.Clamp(state.CommittedSegmentEnd, 1, mapPoints.Count);

        if(committedEnd > 1)
            DrawSegmentRange(pixels, texture.width, texture.height, definition, mapPoints, 1, committedEnd, lineWidth);

        var anchor = mapPoints[state.HeadIndex];

        if(!Mathf.Approximately(anchor.x, state.Tip.x) || !Mathf.Approximately(anchor.y, state.Tip.y))
            DrawLine(
                     pixels,
                     texture.width,
                     texture.height,
                     definition,
                     anchor,
                     state.Tip,
                     lineWidth,
                     Colors.Map.TrailLine
                    );

        texture.SetPixels32(pixels);
        texture.Apply();
    }

    private static void DrawSegmentRange(
        Color32[] pixels,        int width, int height, LocationDefinition definition, IReadOnlyList<Vector2> mapPoints,
        int       fromInclusive, int visiblePointCount, float lineWidth
    ){
        if(mapPoints == null || visiblePointCount <= 1) return;

        var end = Mathf.Min(visiblePointCount, mapPoints.Count);

        for(var i = fromInclusive; i < end; i++)
            DrawLine(
                     pixels,
                     width,
                     height,
                     definition,
                     mapPoints[i - 1],
                     mapPoints[i],
                     lineWidth,
                     Colors.Map.TrailLine
                    );
    }

    private static void DrawLine(
        Color32[] pixels, int width, int height, LocationDefinition def, Vector2 a, Vector2 b, float lineWidth,
        Color32   color
    ){
        var pixelA  = MapToPixel(a, def, width, height);
        var pixelB  = MapToPixel(b, def, width, height);
        var dist    = Vector2.Distance(pixelA, pixelB);
        var radius  = Mathf.Max(0.35f, lineWidth * StampRadiusMul);
        var stepLen = Mathf.Max(0.45f, radius    * StampStepMul);
        var steps   = Mathf.Max(1,     Mathf.CeilToInt(dist / stepLen));

        for(var i = 0; i <= steps; i++){
            var t = i / (float) steps;
            var p = Vector2.Lerp(a, b, t);

            StampCircle(pixels, width, height, def, p, radius, color);
        }
    }

    private static void StampCircle(
        Color32[] pixels, int width, int height, LocationDefinition def, Vector2 mapPoint, float radius, Color32 color
    ){
        var pixel = MapToPixel(mapPoint, def, width, height);
        var r     = Mathf.CeilToInt(radius);

        for(var y = -r; y <= r; y++){
            for(var x = -r; x <= r; x++){
                if(x * x + y * y > r * r) continue;

                var px = pixel.x + x;
                var py = pixel.y + y;

                if(px < 0 || py < 0 || px >= width || py >= height) continue;

                pixels[py * width + px] = color;
            }
        }
    }

    private static Vector2Int MapToPixel(Vector2 mapPoint, LocationDefinition def, int width, int height){
        var mapWidth  = def.BoundsMaxX - def.BoundsMinX;
        var mapHeight = def.BoundsMaxY - def.BoundsMinY;

        if(mapWidth <= 0f) mapWidth = 1f;

        if(mapHeight <= 0f) mapHeight = 1f;

        var nx = (mapPoint.x - def.BoundsMinX) / mapWidth;
        var ny = (mapPoint.y - def.BoundsMinY) / mapHeight;

        return new Vector2Int(
                              Mathf.Clamp(Mathf.RoundToInt(nx * (width  - 1)), 0, width  - 1),
                              Mathf.Clamp(Mathf.RoundToInt(ny * (height - 1)), 0, height - 1)
                             );
    }
}
