using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Layout;

internal static class LeftColumnLayout{
    internal static void EnsurePlayerModelSlot(Transform leftPanel){
        if(!leftPanel) return;

        EnsureColumnLayout(leftPanel);

        var slot = leftPanel.Find(UiHierarchy.RecordsScreen.PlayerModelSlot);

        if(!slot){
            var slotObject = new GameObject(UiHierarchy.RecordsScreen.PlayerModelSlot, typeof(RectTransform));
            slot = slotObject.transform;
        }

        slot.SetParent(leftPanel, false);

        slot.SetAsLastSibling();

        var rect = slot as RectTransform;

        if(rect){
            rect.anchorMin        = new Vector2(0f,   1f);
            rect.anchorMax        = new Vector2(1f,   1f);
            rect.pivot            = new Vector2(0.5f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta        = new Vector2(0f, Spacing.RecordsModelPreferredHeight);
        }

        var layoutElement = slot.GetComponent<LayoutElement>() ?? slot.gameObject.AddComponent<LayoutElement>();
        layoutElement.minHeight       = Spacing.RecordsModelMinHeight;
        layoutElement.preferredHeight = Spacing.RecordsModelPreferredHeight;
        layoutElement.flexibleHeight  = 1f;
        layoutElement.flexibleWidth   = 1f;

        slot.gameObject.SetActive(true);
    }

    internal static Transform EnsureStatsArea(Transform leftPanel){
        if(!leftPanel) return null;

        var statsRoot = leftPanel.Find(UiHierarchy.RecordsScreen.LeftStatsContent);

        if(statsRoot) return ConfigureStatsRoot(statsRoot);

        var statsObject = new GameObject(
                                         UiHierarchy.RecordsScreen.LeftStatsContent,
                                         typeof(RectTransform),
                                         typeof(VerticalLayoutGroup),
                                         typeof(ContentSizeFitter)
                                        );
        statsObject.transform.SetParent(leftPanel, false);

        return ConfigureStatsRoot(statsObject.transform);
    }

    internal static void EnsureColumnLayout(Transform leftPanel){
        if(!leftPanel) return;

        var column = leftPanel.GetComponent<VerticalLayoutGroup>();

        if(!column) column = leftPanel.gameObject.AddComponent<VerticalLayoutGroup>();

        column.spacing = 8f;
        column.padding = new RectOffset(
                                        (int) Spacing.InnerPadding,
                                        (int) Spacing.InnerPadding,
                                        (int) Spacing.InnerPadding,
                                        (int) Spacing.InnerPadding
                                       );
        column.childAlignment         = TextAnchor.UpperLeft;
        column.childControlWidth      = true;
        column.childControlHeight     = true;
        column.childForceExpandWidth  = true;
        column.childForceExpandHeight = false;
    }

    private static Transform ConfigureStatsRoot(Transform statsRoot){
        statsRoot.SetAsFirstSibling();

        var statsRect = statsRoot as RectTransform;

        if(!statsRect) return statsRoot;

        statsRect.anchorMin        = new Vector2(0f,   0f);
        statsRect.anchorMax        = new Vector2(1f,   0f);
        statsRect.pivot            = new Vector2(0.5f, 0f);
        statsRect.anchoredPosition = Vector2.zero;
        statsRect.sizeDelta        = Vector2.zero;

        var statsLayoutElement = statsRoot.GetComponent<LayoutElement>()
                              ?? statsRoot.gameObject.AddComponent<LayoutElement>();
        statsLayoutElement.flexibleHeight = 0f;
        statsLayoutElement.flexibleWidth  = 1f;
        statsLayoutElement.minHeight      = Spacing.RecordsOverviewMinHeight;

        var statsLayout = statsRoot.GetComponent<VerticalLayoutGroup>();
        var statsFitter = statsRoot.GetComponent<ContentSizeFitter>();
        statsFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        statsFitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;
        ScrollLayout.ApplyContentLayout(statsRect, statsLayout);

        return statsRoot;
    }
}
