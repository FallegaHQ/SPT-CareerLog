using Softwyx.CareerLog.Map.Data;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Viewport;

internal static class ViewportLayout{
    public static void StretchFill(RectTransform rect, Transform parent){
        rect.SetParent(parent, false);
        rect.anchorMin     = Vector2.zero;
        rect.anchorMax     = Vector2.one;
        rect.offsetMin     = Vector2.zero;
        rect.offsetMax     = Vector2.zero;
        rect.localScale    = Vector3.one;
        rect.localRotation = Quaternion.identity;
        rect.localPosition = Vector3.zero;
        rect.pivot         = new Vector2(0.5f, 0.5f);
    }

    public static void FitToParent(RectTransform mapRoot, LocationDefinition definition, RectTransform parent){
        ViewportFitter.ApplyFit(mapRoot, definition, parent);
    }
}
