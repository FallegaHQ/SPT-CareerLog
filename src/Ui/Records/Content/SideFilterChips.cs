using System;
using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using Softwyx.CareerLog.Ui.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Content;

internal static class SideFilterChips{
    private static readonly SideFilter[] Filters =[
                                                      SideFilter.Combined,
                                                      SideFilter.Pmc,
                                                      SideFilter.Scav
                                                  ];

    private static readonly string[] LabelKeys =[
                                                    LocaleKeys.FilterCombined,
                                                    LocaleKeys.FilterPmc,
                                                    LocaleKeys.FilterScav
                                                ];

    public static void Add(Transform parent, Action<SideFilter> onFilterSelected){
        if(!parent || onFilterSelected == null) return;

        var rowObject = new GameObject(
                                       UiHierarchy.SideFilter.RowObjectName,
                                       typeof(RectTransform),
                                       typeof(HorizontalLayoutGroup),
                                       typeof(LayoutElement)
                                      );
        rowObject.transform.SetParent(parent, false);

        var rowLayout = rowObject.GetComponent<LayoutElement>();
        rowLayout.minHeight       = 36f;
        rowLayout.preferredHeight = 36f;
        rowLayout.flexibleWidth   = 1f;

        var layout = rowObject.GetComponent<HorizontalLayoutGroup>();
        layout.childAlignment        = TextAnchor.MiddleLeft;
        layout.spacing               = 6f;
        layout.childControlWidth     = true;
        layout.childControlHeight    = true;
        layout.childForceExpandWidth = true;

        for(var i = 0; i < Filters.Length; i++){
            var filter = Filters[i];
            CreateChip(rowObject.transform, LabelKeys[i], filter, onFilterSelected);
        }
    }

    private static void CreateChip(
        Transform parent, string labelKey, SideFilter filter, Action<SideFilter> onFilterSelected
    ){
        var chipObject = new GameObject(
                                        labelKey,
                                        typeof(RectTransform),
                                        typeof(Image),
                                        typeof(Button),
                                        typeof(LayoutElement)
                                       );
        chipObject.transform.SetParent(parent, false);

        var layoutElement = chipObject.GetComponent<LayoutElement>();
        layoutElement.minWidth        = 0f;
        layoutElement.preferredWidth  = -1f;
        layoutElement.flexibleWidth   = 1f;
        layoutElement.minHeight       = 30f;
        layoutElement.preferredHeight = 30f;

        var image = chipObject.GetComponent<Image>();
        image.color = Colors.Control.GraphicWhite;

        var button = chipObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.transition    = Selectable.Transition.ColorTint;
        button.onClick.AddListener(() => onFilterSelected(filter));

        ApplyChipState(button, NavigationState.ActiveSideFilter == filter);

        var textObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(chipObject.transform, false);
        RectLayout.StretchToParent(textObject.GetComponent<RectTransform>());

        var tmp = textObject.GetComponent<TextMeshProUGUI>();
        tmp.fontSize           = Typography.Body;
        tmp.color              = Colors.Text.Caption;
        tmp.alignment          = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.raycastTarget      = false;

        LocalizedTextView.BindKey(textObject.transform, labelKey, null);
    }

    private static void ApplyChipState(Button button, bool active){
        if(!button) return;

        var colors = button.colors;
        colors.colorMultiplier = 1f;
        colors.fadeDuration    = 0.08f;

        if(active){
            colors.normalColor      = Colors.FilterChip.Active;
            colors.highlightedColor = Colors.FilterChip.ActiveHover;
            colors.pressedColor     = Colors.FilterChip.ActivePressed;
            colors.selectedColor    = Colors.FilterChip.Active;
        }
        else{
            colors.normalColor      = Colors.FilterChip.Idle;
            colors.highlightedColor = Colors.FilterChip.Hover;
            colors.pressedColor     = Colors.FilterChip.Pressed;
            colors.selectedColor    = Colors.FilterChip.Idle;
        }

        button.colors = colors;
    }
}
