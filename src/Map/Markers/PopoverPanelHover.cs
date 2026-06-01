using Softwyx.CareerLog.Ui.Design;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Markers;

/// <summary>Fades the popover panel background on hover so the map becomes visible underneath.</summary>
internal sealed class PopoverPanelHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    private Image _panel;

    private void Awake(){
        _panel = GetComponent<Image>();

        if(_panel) _panel.color = Colors.Map.PopoverBackground;
    }

    public void OnPointerEnter(PointerEventData eventData){
        if(_panel) _panel.color = Colors.Map.PopoverBackgroundHover;
    }

    public void OnPointerExit(PointerEventData eventData){
        if(_panel) _panel.color = Colors.Map.PopoverBackground;
    }

    public void ResetBackground(){
        if(_panel) _panel.color = Colors.Map.PopoverBackground;
    }
}
