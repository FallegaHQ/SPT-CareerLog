using UnityEngine;

namespace Softwyx.CareerLog.Map.Markers;

internal static class PopoverPlacement{
    private const float ViewportPadding = 12f;
    private const float MarkerGap       = 10f;

    public static void Apply(
        RectTransform popover, RectTransform markerRect, RectTransform placementRoot, Vector2 popoverSizeHint
    ){
        if(!popover || !markerRect || !placementRoot) return;

        var markerCenterWorld = markerRect.TransformPoint(markerRect.rect.center);
        var markerLocal       = (Vector2) placementRoot.InverseTransformPoint(markerCenterWorld);

        var popoverSize = popover.rect.size;

        if(popoverSize.x < 1f || popoverSize.y < 1f) popoverSize = popoverSizeHint;

        var halfW = popoverSize.x * 0.5f;

        var bounds = placementRoot.rect;
        var maxX   = bounds.width  * 0.5f - ViewportPadding;
        var maxY   = bounds.height * 0.5f - ViewportPadding;

        var markerHalf = Mathf.Max(markerRect.rect.width, markerRect.rect.height) * markerRect.lossyScale.x * 0.5f;

        var preferBelow = markerLocal.y > 0f;
        var preferLeft  = markerLocal.x > maxX - halfW;

        if(TryPlace(
                    preferBelow,
                    preferLeft,
                    markerLocal,
                    markerHalf,
                    maxX,
                    maxY,
                    popoverSize,
                    out var pos,
                    out var pivot
                   )
        || TryPlace(!preferBelow, preferLeft,  markerLocal, markerHalf, maxX, maxY, popoverSize, out pos, out pivot)
        || TryPlace(preferBelow,  !preferLeft, markerLocal, markerHalf, maxX, maxY, popoverSize, out pos, out pivot)){
            popover.pivot            = pivot;
            popover.anchoredPosition = pos;

            return;
        }

        popover.pivot = new Vector2(0.5f, preferBelow ? 1f : 0f);
        popover.anchoredPosition = new Vector2(
                                               Mathf.Clamp(markerLocal.x, -maxX + halfW, maxX - halfW),
                                               preferBelow
                                                   ? markerLocal.y - markerHalf - MarkerGap
                                                   : markerLocal.y + markerHalf + MarkerGap
                                              );
    }

    private static bool TryPlace(
        bool below, bool alignLeft, Vector2 markerLocal, float markerHalf, float maxX, float maxY, Vector2 popoverSize,
        out Vector2 position, out Vector2 pivot
    ){
        pivot = below ? new Vector2(alignLeft ? 1f : 0.5f, 1f) : new Vector2(alignLeft ? 1f : 0.5f, 0f);

        var x = alignLeft ? markerLocal.x - MarkerGap : markerLocal.x;

        var y = below ? markerLocal.y - markerHalf - MarkerGap : markerLocal.y + markerHalf + MarkerGap;

        position = new Vector2(x, y);

        return Fits(position, pivot, popoverSize, maxX, maxY);
    }

    private static bool Fits(Vector2 pos, Vector2 pivot, Vector2 size, float maxX, float maxY){
        var left   = pos.x  - pivot.x * size.x;
        var right  = left   + size.x;
        var bottom = pos.y  - pivot.y * size.y;
        var top    = bottom + size.y;

        return left >= -maxX && right <= maxX && bottom >= -maxY && top <= maxY;
    }
}
