using EFT.Utilities;
using Softwyx.CareerLog.Ui.Design;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.PlayerModel;

/// <summary>
/// Transparent drag surface; inventory <see cref="EFT.UI.DragTrigger"/> delegates are unreliable on clones.
/// </summary>
internal sealed class CharacterModelDragZone : MonoBehaviour, IDragHandler, IBeginDragHandler{
    private XCoordRotation _rotator;

    public void Configure(XCoordRotation rotator){
        _rotator = rotator;

        var image = GetComponent<Image>() ?? gameObject.AddComponent<Image>();
        image.color         = Colors.Control.DragHitTransparent;
        image.raycastTarget = true;
    }

    public void OnBeginDrag(PointerEventData eventData){}

    public void OnDrag(PointerEventData eventData){
        if(_rotator == null || eventData == null) return;

        _rotator.Rotate(eventData.delta.x);
    }
}
