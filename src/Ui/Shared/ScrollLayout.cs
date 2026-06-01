using Softwyx.CareerLog.Ui.Design;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Shared;

/// <summary>Scroll content layout helpers.</summary>
internal static class ScrollLayout{
    public static void ApplyContentLayout(RectTransform contentRect, VerticalLayoutGroup contentLayout){
        if(!contentRect) return;

        contentRect.anchorMin        = new Vector2(0f, 1f);
        contentRect.anchorMax        = new Vector2(1f, 1f);
        contentRect.pivot            = new Vector2(0f, 1f);
        contentRect.anchoredPosition = Vector2.zero;

        if(!contentLayout) return;

        contentLayout.spacing = Spacing.ContentVerticalSpacing;
        contentLayout.padding = new RectOffset(
                                               Spacing.ContentPaddingLeft,
                                               Spacing.ContentPaddingRight,
                                               Spacing.ContentPaddingTop,
                                               Spacing.ContentPaddingBottom
                                              );
        contentLayout.childAlignment         = TextAnchor.UpperLeft;
        contentLayout.childControlWidth      = true;
        contentLayout.childControlHeight     = true;
        contentLayout.childForceExpandWidth  = true;
        contentLayout.childForceExpandHeight = false;
    }

    public static void ApplyScrollContentRect(RectTransform contentRect){
        if(!contentRect) return;

        contentRect.offsetMin          = Vector2.zero;
        contentRect.offsetMax          = Vector2.zero;
        contentRect.anchoredPosition3D = Vector3.zero;
    }
}
