using Softwyx.CareerLog.Map.Data;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Viewport;

internal static class ViewportFitter{
    private const float FitPadding      = 0.92f;
    private const float MinFitDimension = 8f;

    public static Vector2 GetBoundsSize(LocationDefinition definition){
        if(definition == null) return Vector2.zero;

        var mapWidth  = definition.BoundsMaxX - definition.BoundsMinX;
        var mapHeight = definition.BoundsMaxY - definition.BoundsMinY;

        if(mapWidth <= 0f || mapHeight <= 0f) return Vector2.zero;

        return IsRotatedQuarterTurn(definition.CoordinateRotation)
                   ? new Vector2(mapHeight, mapWidth)
                   : new Vector2(mapWidth,  mapHeight);
    }

    public static bool TryComputeScale(RectTransform parent, LocationDefinition definition, out float scale){
        scale = 0f;

        if(!parent || definition == null) return false;

        var boundsSize = GetBoundsSize(definition);

        if(boundsSize.x <= 0f || boundsSize.y <= 0f) return false;

        var parentSize = parent.rect.size;

        if(parentSize.x < MinFitDimension || parentSize.y < MinFitDimension) return false;

        scale = Mathf.Min(parentSize.x / boundsSize.x, parentSize.y / boundsSize.y) * FitPadding;

        return scale >= 0.001f;
    }

    public static void ApplyFit(RectTransform mapRoot, LocationDefinition definition, RectTransform parent){
        if(!mapRoot || !parent || definition == null) return;

        var boundsSize = GetBoundsSize(definition);

        if(boundsSize.x <= 0f || boundsSize.y <= 0f) return;

        mapRoot.sizeDelta = boundsSize;

        if(!TryComputeScale(parent, definition, out var scale)) return;

        mapRoot.localScale = new Vector3(scale, scale, 1f);
    }

    private static bool IsRotatedQuarterTurn(float degrees){
        return Mathf.Abs(Mathf.RoundToInt(degrees / 90f)) % 2 == 1;
    }
}
