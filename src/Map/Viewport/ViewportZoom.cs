using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Ui.Design;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Viewport;

/// <summary>Scroll-wheel zoom and drag pan on the fitted raid map (post-raid + RECORDS modal).</summary>
internal sealed class ViewportZoom : MonoBehaviour{
    internal const string BackdropObjectName = "MapZoomBackdrop";

    private const float MinUserZoom    = 1f;
    private const float MaxUserZoom    = 9f;
    private const float WheelZoomStep  = 0.12f;
    private const float StepZoomFactor = 1.12f;

    private RectTransform    _viewport;
    private RectTransform    _mapRoot;
    private float            _fitScale = 1f;
    private float            _userZoom = 1f;
    private Vector2          _pan;
    private bool             _canPan;
    private TextMeshProUGUI  _controlsHint;
    private ZoomInputSurface _inputSurface;
    private Action           _dismissPopover;

    public void SetDismissPopover(Action dismiss){
        _dismissPopover = dismiss;
    }

    public void Bind(RectTransform viewport, RectTransform mapRoot){
        _viewport = viewport;
        _mapRoot  = mapRoot;
        EnsureHitTarget();
        EnsureMask(viewport);
        EnsureControlsHint(viewport);
        ResetView();
    }

    public float CombinedScale => _fitScale * _userZoom;

    public float UserZoomNormalized => Mathf.InverseLerp(MinUserZoom, MaxUserZoom, _userZoom);

    public void StepZoomIn(){
        StepZoom(true);
    }

    public void StepZoomOut(){
        StepZoom(false);
    }

    public void SetUserZoomNormalized(float normalized){
        _userZoom = Mathf.Lerp(MinUserZoom, MaxUserZoom, Mathf.Clamp01(normalized));

        if(Mathf.Approximately(_userZoom, MinUserZoom)) _pan = Vector2.zero;

        ApplyTransform();
    }

    public void SetControlsHintVisible(bool visible){
        if(_controlsHint) _controlsHint.gameObject.SetActive(visible);
    }

    public void SetControlsHintBottomOffset(float offsetY){
        if(!_controlsHint) return;

        var rect = _controlsHint.rectTransform;
        rect.anchoredPosition = new Vector2(0f, offsetY);
    }

    public void ResetView(){
        _userZoom = 1f;
        _pan      = Vector2.zero;
        ApplyTransform();
    }

    public void SetFitScale(float fitScale){
        if(fitScale < 0.001f) return;

        _fitScale = fitScale;

        if(Mathf.Approximately(_userZoom, MinUserZoom)) _pan = Vector2.zero;

        ApplyTransform();
    }

    private void OnScroll(PointerEventData eventData){
        if(!_mapRoot || eventData == null) return;

        var delta = eventData.scrollDelta.y;

        if(Mathf.Abs(delta) < 0.01f) return;

        _dismissPopover?.Invoke();

        var factor = 1f + delta * WheelZoomStep;
        ZoomAt(ViewportCenterScreen(eventData.pressEventCamera), factor, eventData.pressEventCamera);
    }

    private void OnBeginDrag(){
        _dismissPopover?.Invoke();
        _canPan = _userZoom > 1.01f;
    }

    private void OnDrag(PointerEventData eventData){
        if(!_canPan || !_mapRoot || eventData == null) return;

        _pan += eventData.delta;
        ApplyTransform();
    }

    private void OnPointerClick(PointerEventData eventData){
        if(eventData == null) return;

        switch(eventData.clickCount){
            case 2:
                _dismissPopover?.Invoke();
                ResetView();

                return;
            case 1 when IsBackdropPress(eventData):
                _dismissPopover?.Invoke();

                break;
        }
    }

    private bool IsBackdropPress(PointerEventData eventData){
        var pressed = eventData.pointerPressRaycast.gameObject;

        return pressed && _inputSurface && pressed == _inputSurface.gameObject;
    }

    private void ZoomAt(Vector2 screenPosition, float factor, Camera eventCamera){
        var oldUserZoom = _userZoom;
        var newUserZoom = Mathf.Clamp(_userZoom * factor, MinUserZoom, MaxUserZoom);

        if(Mathf.Approximately(oldUserZoom, newUserZoom)) return;

        var scaleBefore = _fitScale * oldUserZoom;
        var scaleAfter  = _fitScale * newUserZoom;

        if(_viewport
        && RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                                                   _viewport,
                                                                   screenPosition,
                                                                   eventCamera,
                                                                   out var pointerLocal
                                                                  ))
            _pan = pointerLocal - (pointerLocal - _pan) * (scaleAfter / scaleBefore);

        _userZoom = newUserZoom;

        if(Mathf.Approximately(_userZoom, MinUserZoom)) _pan = Vector2.zero;

        ApplyTransform();
    }

    private Vector2 ViewportCenterScreen(Camera eventCamera){
        return !_viewport
                   ? Vector2.zero
                   : RectTransformUtility.WorldToScreenPoint(eventCamera, _viewport.TransformPoint(Vector3.zero));
    }

    private void StepZoom(bool zoomIn){
        if(!_mapRoot) return;

        var factor = zoomIn ? StepZoomFactor : 1f / StepZoomFactor;
        ZoomAt(ViewportCenterScreen(null), factor, null);
    }

    private void ApplyTransform(){
        if(!_mapRoot) return;

        _mapRoot.localScale       = Vector3.one * (_fitScale * _userZoom);
        _mapRoot.anchoredPosition = _pan;
    }

    private void EnsureHitTarget(){
        if(_inputSurface) return;

        var backdrop = new GameObject(
                                      BackdropObjectName,
                                      typeof(RectTransform),
                                      typeof(Image),
                                      typeof(ZoomInputSurface)
                                     );
        var rect = backdrop.GetComponent<RectTransform>();
        rect.SetParent(transform, false);
        rect.SetAsFirstSibling();
        ViewportLayout.StretchFill(rect, transform as RectTransform);

        var image = backdrop.GetComponent<Image>();
        image.color         = Colors.Control.DragHitTransparent;
        image.raycastTarget = true;

        _inputSurface = backdrop.GetComponent<ZoomInputSurface>();
        _inputSurface.Bind(this);
    }

    private static void EnsureMask(RectTransform viewport){
        if(!viewport || viewport.GetComponent<RectMask2D>()) return;

        viewport.gameObject.AddComponent<RectMask2D>();
    }

    private void EnsureControlsHint(RectTransform viewport){
        if(_controlsHint || !viewport) return;

        var hintGo = new GameObject("MapControlsHint", typeof(RectTransform)){
                                                                                 layer = viewport.gameObject.layer
                                                                             };

        var rect = hintGo.GetComponent<RectTransform>();
        rect.SetParent(viewport, false);
        rect.SetAsLastSibling();
        rect.anchorMin        = new Vector2(0f,   0f);
        rect.anchorMax        = new Vector2(1f,   0f);
        rect.pivot            = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f,   8f);
        rect.sizeDelta        = new Vector2(0f,   32f);

        _controlsHint               = hintGo.AddComponent<TextMeshProUGUI>();
        _controlsHint.alignment     = TextAlignmentOptions.Bottom;
        _controlsHint.fontSize      = 13f;
        _controlsHint.color         = new Color(1f, 1f, 1f, 0.7f);
        _controlsHint.raycastTarget = false;
        _controlsHint.text          = LocaleLoader.Format(LocaleKeys.MapZoomHint);
    }

    private sealed class ZoomInputSurface : MonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler,
                                            IPointerClickHandler{
        private ViewportZoom _owner;

        public void Bind(ViewportZoom owner){
            _owner = owner;
        }

        public void OnScroll(PointerEventData eventData){
            _owner?.OnScroll(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData){
            _owner?.OnBeginDrag();
        }

        public void OnDrag(PointerEventData eventData){
            _owner?.OnDrag(eventData);
        }

        public void OnPointerClick(PointerEventData eventData){
            _owner?.OnPointerClick(eventData);
        }
    }
}
