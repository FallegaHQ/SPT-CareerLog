using System.Collections.Generic;
using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Persistence.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Markers;

/// <summary>Markers in viewport space, synced to the rotated trail overlay each frame (zoom/pan safe).</summary>
internal sealed class ScreenOverlay : MonoBehaviour{
    private const string LayerName = "MapMarkers";

    private const float DefaultMarkerPixels = 32f;

    private readonly List<MarkerEntry>  _entries = [];
    private          LocationDefinition _definition;

    private RectTransform _layer;
    private PopoverHost   _popoverHost;
    private RaidRecord    _raid;
    private RectTransform _trailAnchor;

    private void LateUpdate(){
        if(_entries.Count == 0 || !_trailAnchor) return;

        SyncTransforms();
    }

    public void Bind(RectTransform trailAnchor, LocationDefinition definition){
        _trailAnchor = trailAnchor;
        _definition  = definition;
        EnsureLayer();
    }

    public void SetPopoverHost(PopoverHost host, RaidRecord raid){
        _popoverHost = host;
        _raid        = raid;
    }

    public void Rebuild(IReadOnlyList<RaidMovementValue> markers){
        ClearIcons();

        if(!_trailAnchor || _definition == null || markers == null) return;

        EnsureLayer();

        foreach(var marker in markers){
            if(marker == null) continue;

            if(!IconLoader.TryGetForValue(marker, out var sprite)) continue;

            AddIcon(marker, sprite);
        }

        SyncTransforms();
    }

    public void PlaceSingle(RaidMovementValue marker){
        if(!_trailAnchor || marker == null || _definition == null) return;

        if(!IconLoader.TryGetForValue(marker, out var sprite)) return;

        EnsureLayer();
        AddIcon(marker, sprite);
        SyncTransforms();
    }

    public void ClearIcons(){
        _entries.Clear();

        if(!_layer) return;

        for(var i = _layer.childCount - 1; i >= 0; i--)
            Destroy(
                    _layer.GetChild(i).
                           gameObject
                   );
    }

    private void EnsureLayer(){
        if(_layer) return;

        var go = new GameObject(LayerName, typeof(RectTransform)){
                                                                     layer = gameObject.layer
                                                                 };

        _layer = go.GetComponent<RectTransform>();
        _layer.SetParent(transform, false);
        _layer.anchorMin        = Vector2.zero;
        _layer.anchorMax        = Vector2.one;
        _layer.offsetMin        = Vector2.zero;
        _layer.offsetMax        = Vector2.zero;
        _layer.localScale       = Vector3.one;
        _layer.localRotation    = Quaternion.identity;
        _layer.anchoredPosition = Vector2.zero;
    }

    private void AddIcon(RaidMovementValue marker, Sprite sprite){
        var anchorLocal = AnchorLocalPosition(marker);
        var pixels      = MarkerPixels(marker?.Type);

        if(marker == null) return;

        var go = new GameObject(
                                $"Marker_{marker.Type}",
                                typeof(RectTransform),
                                typeof(Image),
                                typeof(Outline),
                                typeof(ClickTarget)
                               ){
                                    layer = gameObject.layer
                                };

        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(_layer, false);
        rect.anchorMin  = new Vector2(0.5f,   0.5f);
        rect.anchorMax  = new Vector2(0.5f,   0.5f);
        rect.pivot      = new Vector2(0.5f,   0.5f);
        rect.sizeDelta  = new Vector2(pixels, pixels);
        rect.localScale = Vector3.one;

        var image = go.GetComponent<Image>();
        image.sprite         = sprite;
        image.color          = Color.white;
        image.preserveAspect = true;
        image.raycastTarget  = true;

        var outline = go.GetComponent<Outline>();
        outline.effectColor    = new Color(0.25f, 0.02f, 0.02f, 0.65f);
        outline.effectDistance = new Vector2(2f, -2f);
        outline.enabled        = false;

        go.GetComponent<ClickTarget>().
           Configure(marker, _raid, _popoverHost);

        if(marker.LongestKill == true) AttachLongestKillBadge(rect);

        _entries.Add(
                     new MarkerEntry{
                                        Rect        = rect,
                                        AnchorLocal = anchorLocal,
                                        Value       = marker
                                    }
                    );
    }

    /// <summary>Centre-relative map coords in trail-overlay local space (matches trail texture UV layout).</summary>
    private Vector2 AnchorLocalPosition(RaidMovementValue marker){
        var mapPoint = CoordinateConverter.MovementToMapPoint(marker);
        var centerX  = (_definition.BoundsMinX + _definition.BoundsMaxX) * 0.5f;
        var centerY  = (_definition.BoundsMinY + _definition.BoundsMaxY) * 0.5f;

        return new Vector2(mapPoint.x - centerX, mapPoint.y - centerY);
    }

    private void SyncTransforms(){
        var host = transform as RectTransform;

        if(!host || !_trailAnchor || !_layer) return;

        var cam = EventCamera();

        foreach(var entry in _entries){
            if(!entry.Rect) continue;

            var world  = _trailAnchor.TransformPoint(entry.AnchorLocal);
            var screen = RectTransformUtility.WorldToScreenPoint(cam, world);

            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(host, screen, cam, out var local))
                entry.Rect.anchoredPosition = local;

            var pixels = MarkerPixels(entry.Value?.Type);
            entry.Rect.sizeDelta = new Vector2(pixels, pixels);
        }
    }

    private static float MarkerPixels(string type){
        return type switch{
                   RaidMarkerTypes.Kill or RaidMarkerTypes.BossKill or RaidMarkerTypes.Killstreak => 28f,
                   RaidMarkerTypes.Achievement                                                    => 44f,
                   RaidMarkerTypes.Extract or RaidMarkerTypes.Death                               => 36f,
                   RaidMarkerTypes.Loot                                                           => 40f,
                   RaidMarkerTypes.DoorUnlock                                                     => 32f,
                   _                                                                              => DefaultMarkerPixels
               };
    }

    private static void AttachLongestKillBadge(RectTransform parent){
        if(!IconLoader.TryGetLongestKillBadge(out var badge)) return;

        var go = new GameObject("LongestKillBadge", typeof(RectTransform), typeof(Image)){
                     layer = parent.gameObject.layer
                 };

        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin        = new Vector2(1f,  1f);
        rect.anchorMax        = new Vector2(1f,  1f);
        rect.pivot            = new Vector2(1f,  1f);
        rect.anchoredPosition = new Vector2(-2f, -2f);
        rect.sizeDelta        = new Vector2(18f, 18f);

        var image = go.GetComponent<Image>();
        image.sprite         = badge;
        image.color          = Color.white;
        image.preserveAspect = true;
        image.raycastTarget  = false;
    }

    private Camera EventCamera(){
        var canvas = GetComponentInParent<Canvas>();

        return canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
    }

    private struct MarkerEntry{
        internal RectTransform     Rect;
        internal Vector2           AnchorLocal;
        internal RaidMovementValue Value;
    }
}
