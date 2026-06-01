using UnityEngine;

namespace Softwyx.CareerLog.Ui.Utilities;

internal static class RectLayout{
    public static void StretchToParent(RectTransform rect){
        if(!rect) return;

        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.pivot            = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.offsetMin        = Vector2.zero;
        rect.offsetMax        = Vector2.zero;
    }

    public static void SetChildActive(Transform parent, string childName, bool active){
        if(!parent || string.IsNullOrEmpty(childName)) return;

        var child = parent.Find(childName);

        if(child) child.gameObject.SetActive(active);
    }
}
