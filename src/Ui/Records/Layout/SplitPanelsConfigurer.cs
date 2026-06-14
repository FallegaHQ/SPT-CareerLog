using System;
using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Ui.Design;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Layout;

internal static class SplitPanelsConfigurer{
    internal static Transform ConfigureSplitPanel(
        Transform recordsRoot, Transform panelsBackground, string panelName, float anchorMinX, float anchorMaxX
    ){
        var panel = PanelDiscovery.FindPanel(panelsBackground, recordsRoot, panelName);

        if(!panel){
            var panelObject = new GameObject(panelName, typeof(RectTransform));
            panel = panelObject.transform;
        }

        panel.SetParent(panelsBackground, false);
        panel.SetAsLastSibling();

        var rect = panel as RectTransform ?? panel.gameObject.AddComponent<RectTransform>();
        rect.localScale       = Vector3.one;
        rect.anchorMin        = new Vector2(anchorMinX, 0f);
        rect.anchorMax        = new Vector2(anchorMaxX, 1f);
        rect.pivot            = new Vector2(0.5f,       0.5f);
        rect.anchoredPosition = Vector2.zero;

        var         leftInset  = Spacing.InnerPadding;
        var         rightInset = -Spacing.InnerPadding;
        const float bottom     = Spacing.InnerPadding;
        const float top        = -Spacing.InnerPadding;

        if(anchorMinX > 0f) leftInset = Spacing.ColumnGap;

        if(anchorMaxX < 1f) rightInset = -Spacing.ColumnGap;

        rect.offsetMin = new Vector2(leftInset,  bottom);
        rect.offsetMax = new Vector2(rightInset, top);

        if(string.Equals(panelName, UiHierarchy.RecordsScreen.RightPanel, StringComparison.Ordinal))
            EnsurePanelClipping(panel);

        panel.gameObject.SetActive(true);

        return panel;
    }

    private static void EnsurePanelClipping(Transform panel){
        if(!panel) return;

        if(!panel.GetComponent<RectMask2D>()) panel.gameObject.AddComponent<RectMask2D>();
    }
}
