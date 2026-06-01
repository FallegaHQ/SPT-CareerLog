using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Shared;

internal static class ScrollSettings{
    public static void ApplyClamped(ScrollRect scroll){
        if(!scroll) return;

        scroll.movementType = ScrollRect.MovementType.Clamped;
    }

    public static void ApplyClampedToDescendants(Transform root){
        if(!root) return;

        foreach(var scroll in root.GetComponentsInChildren<ScrollRect>(true)) ApplyClamped(scroll);
    }
}
