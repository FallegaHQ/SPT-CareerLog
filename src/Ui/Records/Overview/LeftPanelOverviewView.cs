using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Records.Content;
using Softwyx.CareerLog.Ui.Shared;
using Softwyx.CareerLog.Ui.Utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Overview;

internal static class LeftPanelOverviewView{
    public static void Build(
        Transform          leftRoot,            LifetimeStats lifetime, ScrollContentBuilder.ScrollTextStyle style,
        Action<SideFilter> onSideFilterChanged, Action        onOverviewOpen
    ){
        if(!leftRoot) return;

        ScrollContentBuilder.EnsureContentLayout(leftRoot);
        ScrollContentBuilder.Clear(leftRoot);

        var section = ScrollContentBuilder.AddSection(leftRoot, LocaleKeys.SectionOverview, style);
        var items   = ScrollContentBuilder.GetItemsContainer(section);

        if(onSideFilterChanged != null) SideFilterChips.Add(items, onSideFilterChanged);

        if(lifetime == null){
            ScrollContentBuilder.AddLocalizedBodyLine(items, LocaleKeys.ProfileUnavailable, style);
            ScrollContentBuilder.RebuildLayout(leftRoot);

            return;
        }

        if(lifetime.RaidsRecorded <= 0)
            ScrollContentBuilder.AddLocalizedBodyLine(items, LocaleKeys.NoLifetime, style);
        else
            LifetimeStatsRows.AddBasicSummary(items, lifetime, style);

        if(onOverviewOpen != null) AddOverviewButton(items, onOverviewOpen);

        ScrollContentBuilder.RebuildLayout(leftRoot);
    }

    private static void AddOverviewButton(Transform parent, Action onClick){
        var buttonObject = new GameObject(
                                          "OverviewOpenButton",
                                          typeof(RectTransform),
                                          typeof(Image),
                                          typeof(Button),
                                          typeof(LayoutElement)
                                         );
        buttonObject.transform.SetParent(parent, false);

        var layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.minHeight       = Spacing.RowMinHeight;
        layoutElement.preferredHeight = Spacing.RowMinHeight;
        layoutElement.flexibleWidth   = 1f;

        var image  = buttonObject.GetComponent<Image>();
        var button = buttonObject.GetComponent<Button>();
        image.color          = Colors.Control.ButtonSurface;
        button.targetGraphic = image;
        button.onClick.AddListener(() => onClick());

        var labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);
        RectLayout.StretchToParent(labelObject.GetComponent<RectTransform>());

        var tmp = labelObject.GetComponent<TextMeshProUGUI>();
        tmp.fontSize           = Typography.Body;
        tmp.color              = Colors.Text.SectionTitle;
        tmp.alignment          = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.raycastTarget      = false;

        LocalizedTextView.BindKey(labelObject.transform, LocaleKeys.OverviewOpenButton, null);
    }
}
