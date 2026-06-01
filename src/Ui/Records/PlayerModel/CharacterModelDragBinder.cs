using EFT.UI;
using EFT.Utilities;
using HarmonyLib;
using Softwyx.CareerLog.Infrastructure;
using Softwyx.CareerLog.Interop;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Softwyx.CareerLog.Ui.Records.PlayerModel;

internal static class CharacterModelDragBinder{
    private const float DefaultRotationSpeed = 1f;

    private static XCoordRotation _activeRotator;

    public static void WireStatsWindow(InventoryPlayerModelWithStatsWindow statsWindow, PlayerModelView modelView){
        if(!statsWindow || !modelView) return;

        var rotator = EftScreenFieldBinder.GetField<XCoordRotation>(
                                                                    statsWindow,
                                                                    GameAssemblyNames.
                                                                        InventoryPlayerModelWithStatsFields.Rotator
                                                                   );
        var drag = EftScreenFieldBinder.GetField<DragTrigger>(
                                                              statsWindow,
                                                              GameAssemblyNames.InventoryPlayerModelWithStatsFields.
                                                                  DragTrigger
                                                             );

        if(!rotator || !drag){
            InstallDragZoneFallback(modelView);

            return;
        }

        var target = ResolveRotationTarget(modelView);

        if(!target) return;

        CopyRotationSpeedFromInventoryTemplate(rotator);
        rotator.Init(target);
        _activeRotator = rotator;

        Traverse.Create(drag).
                 Field("onDrag").
                 SetValue(null);
        drag.onDrag += HandleDrag;

        var dragRect = drag.transform as RectTransform;

        if(dragRect){
            dragRect.anchorMin        = Vector2.zero;
            dragRect.anchorMax        = Vector2.one;
            dragRect.offsetMin        = Vector2.zero;
            dragRect.offsetMax        = Vector2.zero;
            dragRect.anchoredPosition = Vector2.zero;
        }

        drag.transform.SetAsLastSibling();
        drag.gameObject.SetActive(true);

        var graphic = drag.GetComponent<UnityEngine.UI.Graphic>();

        if(graphic) graphic.raycastTarget = true;
    }

    private static void HandleDrag(PointerEventData data){
        if(data == null || _activeRotator == null) return;

        _activeRotator.Rotate(-data.delta.x);
    }

    private static Transform ResolveRotationTarget(PlayerModelView modelView){
        if(!modelView) return null;

        if(modelView.ModelPlayerPoser) return modelView.ModelPlayerPoser.transform;

        return modelView.transform.Find(
                                        $"{UiHierarchy.InventoryCharacterTab.PlayerMvObject}/"
                                      + $"{UiHierarchy.InventoryCharacterTab.MenuPlayer}"
                                       );
    }

    private static void InstallDragZoneFallback(PlayerModelView modelView){
        if(!modelView) return;

        var root    = modelView.gameObject;
        var rotator = root.GetComponent<XCoordRotation>() ?? root.AddComponent<XCoordRotation>();
        var target  = ResolveRotationTarget(modelView);

        if(target) rotator.Init(target);

        _activeRotator = rotator;

        var dragTransform = root.transform.Find(UiHierarchy.InventoryCharacterTab.DragTrigger);

        if(dragTransform){
            var zone = dragTransform.GetComponent<CharacterModelDragZone>()
                    ?? dragTransform.gameObject.AddComponent<CharacterModelDragZone>();
            zone.Configure(rotator);

            return;
        }

        var overlay = root.GetComponent<CharacterModelDragZone>() ?? root.AddComponent<CharacterModelDragZone>();
        overlay.Configure(rotator);
    }

    private static void CopyRotationSpeedFromInventoryTemplate(XCoordRotation target){
        if(!target) return;

        var template = InventoryScreenAccess.GetInventoryScreen()?.
                                             GetComponentInChildren<InventoryPlayerModelWithStatsWindow>(true);

        if(!template) return;

        var source = EftScreenFieldBinder.GetField<XCoordRotation>(template, "_rotator");

        if(!source) return;

        var speed = Traverse.Create(source).
                             Field("_rotationSpeed").
                             GetValue<float>();

        Traverse.Create(target).
                 Field("_rotationSpeed").
                 SetValue(speed > 0f ? speed : DefaultRotationSpeed);
    }
}
