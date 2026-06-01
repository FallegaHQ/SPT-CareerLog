using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Viewport;

internal static class UiFactory{
    public static void CreateSvgImage(string name, Transform parent, Sprite sprite){
        var go = new GameObject(name, typeof(RectTransform)){
                                                                layer = parent.gameObject.layer
                                                            };

        var rect = go.GetComponent<RectTransform>();
        ViewportLayout.StretchFill(rect, parent);

        var image = go.AddComponent<SVGImage>();
        image.sprite         = sprite;
        image.preserveAspect = false;
        image.raycastTarget  = false;
        image.color          = Color.white;
    }

    public static RawImage CreateRawOverlay(string name, Transform parent){
        var go = new GameObject(name, typeof(RectTransform), typeof(RawImage)){
                                                                                  layer = parent.gameObject.layer
                                                                              };

        var rect = go.GetComponent<RectTransform>();
        ViewportLayout.StretchFill(rect, parent);

        var raw = go.GetComponent<RawImage>();
        raw.raycastTarget = false;
        raw.color         = Color.white;

        return raw;
    }
}
