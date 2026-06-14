using System.Collections.Generic;
using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Persistence.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Markers;

/// <summary>Victim position pins shown while a kill-family marker popover is open.</summary>
internal sealed class VictimOverlay : MonoBehaviour{
    private const string LayerName = "MapVictimMarkers";

    private const    float         VictimScreenPixels = 33f;
    private const    float         LinkThickness      = 3f;
    private readonly List<Vector2> _anchors           = [];

    private readonly List<RectTransform> _icons = [];
    private readonly List<RectTransform> _links = [];
    private          RectTransform       _activeMarkerRect;
    private          LocationDefinition  _definition;

    private RectTransform _layer;
    private RectTransform _trailAnchor;

    private void LateUpdate(){
        if(_icons.Count == 0 || !_trailAnchor) return;

        SyncTransforms();
    }

    public void Bind(RectTransform trailAnchor, LocationDefinition definition){
        _trailAnchor = trailAnchor;
        _definition  = definition;
        EnsureLayer();
    }

    public void Show(RectTransform markerRect, RaidMovementValue marker){
        Clear();

        if(!_trailAnchor || _definition == null || marker?.Victims == null) return;

        _activeMarkerRect = markerRect;

        if(!IconLoader.TryGetVictimIcon(out var sprite)) return;

        EnsureLayer();

        foreach(var victim in marker.Victims){
            if(victim?.Point == null || victim.Point.Length < 2) continue;

            var value = new RaidMovementValue{
                                                 Point = victim.Point
                                             };

            var go = new GameObject("VictimPin", typeof(RectTransform), typeof(Image)){
                         layer = gameObject.layer
                     };

            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(_layer, false);
            rect.anchorMin = new Vector2(0.5f,               0.5f);
            rect.anchorMax = new Vector2(0.5f,               0.5f);
            rect.pivot     = new Vector2(0.5f,               0.5f);
            rect.sizeDelta = new Vector2(VictimScreenPixels, VictimScreenPixels);

            var image = go.GetComponent<Image>();
            image.sprite         = sprite;
            image.color          = Color.white;
            image.preserveAspect = true;
            image.raycastTarget  = false;

            _icons.Add(rect);
            _anchors.Add(AnchorLocalPosition(value));
        }

        SyncTransforms();
    }

    public void Hide(){
        Clear();
    }

    private void Clear(){
        _icons.Clear();
        _anchors.Clear();
        _links.Clear();
        _activeMarkerRect = null;

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
        _layer.SetAsLastSibling();
    }

    private Vector2 AnchorLocalPosition(RaidMovementValue marker){
        var mapPoint = CoordinateConverter.MovementToMapPoint(marker);
        var centerX  = (_definition.BoundsMinX + _definition.BoundsMaxX) * 0.5f;
        var centerY  = (_definition.BoundsMinY + _definition.BoundsMaxY) * 0.5f;

        return new Vector2(mapPoint.x - centerX, mapPoint.y - centerY);
    }

    private void SyncTransforms(){
        var host = transform as RectTransform;

        if(!host || !_trailAnchor || _icons.Count != _anchors.Count) return;

        var cam = EventCamera();

        EnsureLinks();

        for(var i = 0; i < _icons.Count; i++){
            var rect = _icons[i];

            if(!rect) continue;

            var world  = _trailAnchor.TransformPoint(_anchors[i]);
            var screen = RectTransformUtility.WorldToScreenPoint(cam, world);

            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(host, screen, cam, out var local))
                rect.anchoredPosition = local;
        }

        UpdateLinks();
    }

    private void EnsureLinks(){
        if(_links.Count == _icons.Count) return;

        for(var i = _links.Count; i < _icons.Count; i++){
            var go = new GameObject("VictimLink", typeof(RectTransform), typeof(Image)){
                         layer = gameObject.layer
                     };
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(_layer, false);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot     = new Vector2(0f,   0.5f);

            var img = go.GetComponent<Image>();
            img.color         = new Color(0.9f, 0.15f, 0.15f, 0.45f);
            img.raycastTarget = false;

            // Put links behind pins.
            rect.SetAsFirstSibling();

            _links.Add(rect);
        }
    }

    private void UpdateLinks(){
        if(!_activeMarkerRect) return;

        var start = _activeMarkerRect.anchoredPosition;

        for(var i = 0; i < _icons.Count && i < _links.Count; i++){
            var victim = _icons[i];
            var link   = _links[i];

            if(!victim || !link) continue;

            var end = victim.anchoredPosition;
            var dir = end - start;
            var len = dir.magnitude;

            if(len < 1f){
                link.gameObject.SetActive(false);

                continue;
            }

            link.gameObject.SetActive(true);
            link.anchoredPosition = start;
            link.sizeDelta        = new Vector2(len, LinkThickness);
            link.localRotation    = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }
    }

    private Camera EventCamera(){
        var canvas = GetComponentInParent<Canvas>();

        return canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
    }
}
