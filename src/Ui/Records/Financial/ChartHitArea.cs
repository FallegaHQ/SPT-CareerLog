using UnityEngine;
using UnityEngine.EventSystems;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal sealed class ChartHitArea : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler, IPointerClickHandler{
    private ChartPanel _panel;

    public void Bind(ChartPanel panel){
        _panel = panel;
    }

    public void OnPointerMove(PointerEventData eventData){
        _panel?.OnPointerMove(eventData);
    }

    public void OnPointerExit(PointerEventData eventData){
        _panel?.OnPointerExit();
    }

    public void OnPointerClick(PointerEventData eventData){
        _panel?.OnPointerClick();
    }
}
