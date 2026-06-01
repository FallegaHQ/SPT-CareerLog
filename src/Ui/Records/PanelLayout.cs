using System;
using Softwyx.CareerLog.Ui.Records.Layout;
using Softwyx.CareerLog.Ui.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records;

/// <summary>Forces layout on the Records panel chain after dynamic content is built.</summary>
internal static class PanelLayout{
    public static void RunContentRebuild(Transform hostRoot, Transform tabContentRoot, Action build){
        var scrollRoot = GetScrollRoot(tabContentRoot);
        var wasActive  = scrollRoot && scrollRoot.gameObject.activeSelf;

        if(scrollRoot) scrollRoot.gameObject.SetActive(false);

        build();

        if(tabContentRoot is RectTransform contentRect){
            ScrollContentBuilder.RebuildLayout(contentRect);
            ResetScrollToTop(contentRect);
        }

        RefreshRightTab(hostRoot, tabContentRoot);

        if(scrollRoot && wasActive) scrollRoot.gameObject.SetActive(true);
    }

    public static void Refresh(PanelRegions panels){
        if(panels.LeftStatsRoot is RectTransform leftRect) ScrollContentBuilder.RebuildLayout(leftRect);

        RefreshRightTab(panels.RightTabHostRoot, panels.RightTabContentRoot);
    }

    private static void RefreshRightTab(Transform hostRoot, Transform tabContentRoot){
        if(hostRoot is RectTransform hostRect) RebuildUpward(hostRect);

        if(tabContentRoot is not RectTransform contentRect) return;

        ScrollContentBuilder.RebuildLayout(contentRect);
        ResetScrollToTop(contentRect);
    }

    private static Transform GetScrollRoot(Transform content){
        return content?.parent?.parent;
    }

    private static void RebuildUpward(RectTransform start){
        var current = start;

        while(current){
            LayoutRebuilder.ForceRebuildLayoutImmediate(current);
            current = current.parent as RectTransform;
        }
    }

    private static void ResetScrollToTop(Transform content){
        if(!content?.parent?.parent) return;

        if(!content) return;

        var scroll = content.parent.parent.GetComponent<ScrollRect>();

        if(!scroll) return;

        scroll.StopMovement();
        scroll.verticalNormalizedPosition = 1f;
    }
}
