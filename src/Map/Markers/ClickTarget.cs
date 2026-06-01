using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Map.Viewport;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Markers;

/// <summary>Clickable map marker that opens the detail popover.</summary>
internal sealed class ClickTarget : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
                                    IScrollHandler{
    private const float MinHitSize = 28f;

    private PopoverHost       _popoverHost;
    private RaidMovementValue _marker;
    private RaidRecord        _raid;
    private Outline           _outline;
    private GameObject        _zoomBackdrop;

    public void Configure(RaidMovementValue marker, RaidRecord raid, PopoverHost popoverHost){
        _marker      = marker;
        _raid        = raid;
        _popoverHost = popoverHost;

        var image = GetComponent<Image>();

        if(image) image.raycastTarget = true;

        _outline = GetComponent<Outline>();
        if(_outline) _outline.enabled = false;

        var zoom = GetComponentInParent<ViewportZoom>();
        _zoomBackdrop = zoom
                            ? zoom.transform.Find(ViewportZoom.BackdropObjectName)?.
                                   gameObject
                            : null;

        EnsureHitSize();
    }

    public void OnPointerClick(PointerEventData eventData){
        if(eventData is not{
                               button: PointerEventData.InputButton.Left
                           })
            return;

        eventData.Use();
        _popoverHost?.Toggle(transform as RectTransform, _marker, _raid);
    }

    public void OnPointerEnter(PointerEventData eventData){
        if(_outline) _outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData){
        if(_outline) _outline.enabled = false;
    }

    public void OnScroll(PointerEventData eventData){
        // Markers should not block zooming (forward wheel to the zoom input surface).
        if(_zoomBackdrop) ExecuteEvents.Execute(_zoomBackdrop, eventData, ExecuteEvents.scrollHandler);
    }

    private void EnsureHitSize(){
        var rect = transform as RectTransform;

        if(!rect) return;

        var size = Mathf.Max(rect.sizeDelta.x, rect.sizeDelta.y, MinHitSize);
        rect.sizeDelta = new Vector2(size, size);
    }
}
