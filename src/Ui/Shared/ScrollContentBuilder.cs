using System;
using System.Collections.Generic;
using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Ui.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Softwyx.CareerLog.Ui.Shared;

/// <summary>Facade for scroll content -- sections/rows are built by internal builders.</summary>
internal static class ScrollContentBuilder{
    public const string SectionObjectName = UiHierarchy.ScrollContent.Section;
    public const string ItemsObjectName   = UiHierarchy.ScrollContent.Items;
    public const string HeaderObjectName  = UiHierarchy.ScrollContent.Header;
    public const string BodyObjectName    = UiHierarchy.ScrollContent.Body;
    public const string RowObjectName     = UiHierarchy.ScrollContent.Row;

    public static void Clear(Transform contentRoot){
        if(!contentRoot) return;

        for(var i = contentRoot.childCount - 1; i >= 0; i--){
            var child = contentRoot.GetChild(i);

            if(child) Object.Destroy(child.gameObject);
        }
    }

    public static void EnsureContentLayout(Transform contentRoot){
        var contentRect = contentRoot as RectTransform;

        if(!contentRect) contentRect = contentRoot.gameObject.AddComponent<RectTransform>();

        var layout = contentRoot.GetComponent<VerticalLayoutGroup>();

        if(!layout) layout = contentRoot.gameObject.AddComponent<VerticalLayoutGroup>();

        var fitter = contentRoot.GetComponent<ContentSizeFitter>();

        if(!fitter) fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();

        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

        ScrollLayout.ApplyContentLayout(contentRect, layout);
    }

    public static ScrollTextStyle ResolveStyle(TextMeshProUGUI styleSource){
        return new ScrollTextStyle{
                                      Font         = styleSource ? styleSource.font : null,
                                      CaptionColor = styleSource ? styleSource.color : Colors.Text.Caption,
                                      ValueColor   = Colors.Text.Value
                                  };
    }

    public static Transform AddSection(Transform contentRoot, string title, ScrollTextStyle style){
        return ScrollSectionBuilder.AddSection(contentRoot, title, style);
    }

    public static Transform AddItemsPanel(Transform contentRoot){
        return ScrollSectionBuilder.AddItemsPanel(contentRoot);
    }

    public static Transform GetItemsContainer(Transform section){
        return ScrollSectionBuilder.GetItemsContainer(section);
    }

    public static void AddSubHeader(Transform items, string line, ScrollTextStyle style){
        ScrollRowBuilder.AddSubHeader(items, line, style);
    }

    public static void AddBodyLine(Transform items, string line, ScrollTextStyle style){
        ScrollRowBuilder.AddBodyLine(items, line, style);
    }

    public static void AddLocalizedBodyLine(Transform items, string localizationKey, ScrollTextStyle style){
        ScrollRowBuilder.AddLocalizedBodyLine(items, localizationKey, style);
    }

    public static void AddStatRow(Transform items, string caption, string value, ScrollTextStyle style){
        ScrollRowBuilder.AddStatRow(items, caption, value, style);
    }

    public static void AddLocalizedStatRow(Transform items, string captionKey, string value, ScrollTextStyle style){
        ScrollRowBuilder.AddLocalizedStatRow(items, captionKey, value, style);
    }

    public static void RebuildLayout(Transform contentRoot){
        if(contentRoot is RectTransform contentRect) LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }

    public static IEnumerable<KeyValuePair<string, TValue>> SortOrdinal<TValue>(Dictionary<string, TValue> counters){
        var keys = new List<string>(counters.Keys);
        keys.Sort(StringComparer.Ordinal);

        foreach(var key in keys) yield return new KeyValuePair<string, TValue>(key, counters[key]);
    }

    public struct ScrollTextStyle{
        public TMP_FontAsset Font;
        public Color         CaptionColor;
        public Color         ValueColor;
    }
}
