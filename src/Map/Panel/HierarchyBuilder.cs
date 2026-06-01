using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Map.Svg;
using Softwyx.CareerLog.Map.Trail;
using Softwyx.CareerLog.Map.Viewport;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Panel;

internal sealed class HierarchyBuilder{
    public RectTransform Host{
        get;
        private set;
    }
    private RectTransform MapContent{
        get;
        set;
    }
    public RawImage TrailOverlay{
        get;
        private set;
    }
    public Texture2D TrailTexture{
        get;
        private set;
    }

    public void Build(Transform panelRoot, LocationDefinition definition){
        Host = new GameObject("MapBounds", typeof(RectTransform)).GetComponent<RectTransform>();
        Host.SetParent(panelRoot, false);
        Host.anchorMin        = new Vector2(0.5f, 0.5f);
        Host.anchorMax        = new Vector2(0.5f, 0.5f);
        Host.pivot            = new Vector2(0.5f, 0.5f);
        Host.anchoredPosition = Vector2.zero;
        Host.sizeDelta        = ViewportFitter.GetBoundsSize(definition);

        var mapWidth  = definition.BoundsMaxX - definition.BoundsMinX;
        var mapHeight = definition.BoundsMaxY - definition.BoundsMinY;

        var rotateRoot = new GameObject("MapRotate", typeof(RectTransform)).GetComponent<RectTransform>();
        rotateRoot.SetParent(Host, false);
        rotateRoot.anchorMin        = new Vector2(0.5f, 0.5f);
        rotateRoot.anchorMax        = new Vector2(0.5f, 0.5f);
        rotateRoot.pivot            = new Vector2(0.5f, 0.5f);
        rotateRoot.anchoredPosition = Vector2.zero;
        rotateRoot.sizeDelta        = new Vector2(mapWidth, mapHeight);
        rotateRoot.localRotation    = Quaternion.Euler(0f, 0f, definition.CoordinateRotation);

        MapContent = new GameObject("MapContent", typeof(RectTransform)).GetComponent<RectTransform>();
        MapContent.SetParent(rotateRoot, false);
        MapContent.anchorMin        = new Vector2(0.5f,     0.5f);
        MapContent.anchorMax        = new Vector2(0.5f,     0.5f);
        MapContent.pivot            = new Vector2(0.5f,     0.5f);
        MapContent.sizeDelta        = new Vector2(mapWidth, mapHeight);
        MapContent.anchoredPosition = Vector2.zero;
        MapContent.localRotation    = Quaternion.Euler(0f, 0f, -definition.CoordinateRotation);

        var sprite = SvgLoader.LoadSprite(definition.SvgPath);

        if(sprite) UiFactory.CreateSvgImage("MapImage", MapContent, sprite);

        TrailTexture         = TrailTextureDrawer.CreateOverlayTexture();
        TrailOverlay         = UiFactory.CreateRawOverlay("TrailOverlay", MapContent);
        TrailOverlay.texture = TrailTexture;

        var trailRect = TrailOverlay.rectTransform;
        trailRect.localRotation = Quaternion.Euler(0f, 0f, definition.TrailOverlayRotation);
    }

    public void FitToViewport(RectTransform viewport, LocationDefinition definition){
        ViewportLayout.FitToParent(Host, definition, viewport);
    }
}
