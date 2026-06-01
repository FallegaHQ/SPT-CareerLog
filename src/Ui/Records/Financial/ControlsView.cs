using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Financial;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using Softwyx.CareerLog.Ui.Utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal static class ControlsView{
    private const float ToolbarHeight = 36f;

    public static void Add(
        Transform parent, StashSnapshotIndex index, ScrollContentBuilder.ScrollTextStyle style, Action onChanged
    ){
        if(!parent) return;

        var offset = NavigationState.FinancialPeriodOffset;
        PeriodNavigation.ClampOffset(index, NavigationState.FinancialPeriod, ref offset);

        if(offset != NavigationState.FinancialPeriodOffset) NavigationState.SetFinancialPeriodOffset(offset);

        var rowObject = new GameObject(
                                       "FinancialToolbar",
                                       typeof(RectTransform),
                                       typeof(HorizontalLayoutGroup),
                                       typeof(LayoutElement)
                                      );
        rowObject.transform.SetParent(parent, false);

        var rowLayout = rowObject.GetComponent<LayoutElement>();
        rowLayout.minHeight       = ToolbarHeight;
        rowLayout.preferredHeight = ToolbarHeight;
        rowLayout.flexibleWidth   = 1f;

        var layout = rowObject.GetComponent<HorizontalLayoutGroup>();
        layout.padding                = new RectOffset(0, 0, 2, 2);
        layout.spacing                = 8f;
        layout.childAlignment         = TextAnchor.MiddleCenter;
        layout.childControlWidth      = true;
        layout.childControlHeight     = true;
        layout.childForceExpandWidth  = false;
        layout.childForceExpandHeight = false;

        var left = CreateGroup(rowObject.transform, "FinancialViewGroup");
        AddChip(left, LocaleKeys.FinancialViewChart, () => SelectView(ViewMode.Chart, onChanged));

        AddChip(left, LocaleKeys.FinancialViewTable, () => SelectView(ViewMode.Table, onChanged));

        if(NavigationState.FinancialView == ViewMode.Chart){
            AddChip(left, LocaleKeys.FinancialStyleLine, () => SelectStyle(ChartStyle.Line, onChanged));
            AddChip(left, LocaleKeys.FinancialStyleBar,  () => SelectStyle(ChartStyle.Bar,  onChanged));
        }

        var spacer = new GameObject("Spacer", typeof(RectTransform), typeof(LayoutElement));
        spacer.transform.SetParent(rowObject.transform, false);
        spacer.GetComponent<LayoutElement>().
               flexibleWidth = 1f;

        var right = CreateGroup(rowObject.transform, "FinancialPeriodGroup");
        AddChip(right, LocaleKeys.FinancialPeriodDay,      () => SelectPeriod(PeriodKind.Day,      onChanged));
        AddChip(right, LocaleKeys.FinancialPeriodWeek,     () => SelectPeriod(PeriodKind.Week,     onChanged));
        AddChip(right, LocaleKeys.FinancialPeriodMonth,    () => SelectPeriod(PeriodKind.Month,    onChanged));
        AddChip(right, LocaleKeys.FinancialPeriodLifetime, () => SelectPeriod(PeriodKind.Lifetime, onChanged));

        var nav = CreateNavGroup(right);
        CreateNavButton(
                        nav,
                        LocaleKeys.FinancialPeriodPrev,
                        PeriodNavigation.CanShift(
                                                  index,
                                                  NavigationState.FinancialPeriod,
                                                  NavigationState.FinancialPeriodOffset,
                                                  -1
                                                 ),
                        () => ShiftPeriod(-1, index, onChanged)
                       );
        CreatePeriodLabel(nav, style);
        CreateNavButton(
                        nav,
                        LocaleKeys.FinancialPeriodNext,
                        PeriodNavigation.CanShift(
                                                  index,
                                                  NavigationState.FinancialPeriod,
                                                  NavigationState.FinancialPeriodOffset,
                                                  1
                                                 ),
                        () => ShiftPeriod(1, index, onChanged)
                       );
    }

    private static Transform CreateGroup(Transform parent, string name){
        var groupObject = new GameObject(
                                         name,
                                         typeof(RectTransform),
                                         typeof(HorizontalLayoutGroup),
                                         typeof(LayoutElement)
                                        );
        groupObject.transform.SetParent(parent, false);

        var groupLayout = groupObject.GetComponent<LayoutElement>();
        groupLayout.minHeight       = ToolbarHeight - 4f;
        groupLayout.preferredHeight = ToolbarHeight - 4f;

        var layout = groupObject.GetComponent<HorizontalLayoutGroup>();
        layout.spacing                = 4f;
        layout.childAlignment         = TextAnchor.MiddleCenter;
        layout.childControlWidth      = true;
        layout.childControlHeight     = true;
        layout.childForceExpandWidth  = false;
        layout.childForceExpandHeight = false;

        return groupObject.transform;
    }

    private static Transform CreateNavGroup(Transform parent){
        var navObject = new GameObject(
                                       "FinancialPeriodNav",
                                       typeof(RectTransform),
                                       typeof(HorizontalLayoutGroup),
                                       typeof(LayoutElement)
                                      );
        navObject.transform.SetParent(parent, false);

        var navLayout = navObject.GetComponent<LayoutElement>();
        navLayout.minWidth       = 180f;
        navLayout.preferredWidth = 220f;

        var layout = navObject.GetComponent<HorizontalLayoutGroup>();
        layout.spacing               = 4f;
        layout.childAlignment        = TextAnchor.MiddleCenter;
        layout.childControlWidth     = true;
        layout.childControlHeight    = true;
        layout.childForceExpandWidth = false;

        return navObject.transform;
    }

    private static void AddChip(Transform parent, string labelKey, Action onClick){
        var chipObject = new GameObject(
                                        labelKey,
                                        typeof(RectTransform),
                                        typeof(Image),
                                        typeof(Button),
                                        typeof(LayoutElement)
                                       );
        chipObject.transform.SetParent(parent, false);

        var layoutElement = chipObject.GetComponent<LayoutElement>();
        layoutElement.minWidth        = 52f;
        layoutElement.preferredWidth  = 58f;
        layoutElement.minHeight       = 26f;
        layoutElement.preferredHeight = 26f;

        var image  = chipObject.GetComponent<Image>();
        var button = chipObject.GetComponent<Button>();
        image.color          = Colors.Control.GraphicWhite;
        button.targetGraphic = image;
        button.onClick.AddListener(() => onClick());
        ApplyChipState(button, labelKey);

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

    private static void ApplyChipState(Button button, string labelKey){
        var active = labelKey switch{
                         LocaleKeys.FinancialPeriodDay      => NavigationState.FinancialPeriod == PeriodKind.Day,
                         LocaleKeys.FinancialPeriodWeek     => NavigationState.FinancialPeriod == PeriodKind.Week,
                         LocaleKeys.FinancialPeriodMonth    => NavigationState.FinancialPeriod == PeriodKind.Month,
                         LocaleKeys.FinancialPeriodLifetime => NavigationState.FinancialPeriod == PeriodKind.Lifetime,
                         LocaleKeys.FinancialViewChart      => NavigationState.FinancialView   == ViewMode.Chart,
                         LocaleKeys.FinancialViewTable      => NavigationState.FinancialView   == ViewMode.Table,
                         LocaleKeys.FinancialStyleLine      => NavigationState.ChartStyle      == ChartStyle.Line,
                         LocaleKeys.FinancialStyleBar       => NavigationState.ChartStyle      == ChartStyle.Bar,
                         _                                  => false
                     };

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

    private static void CreateNavButton(Transform parent, string labelKey, bool enabled, Action onClick){
        var buttonObject = new GameObject(
                                          labelKey,
                                          typeof(RectTransform),
                                          typeof(Image),
                                          typeof(Button),
                                          typeof(LayoutElement)
                                         );
        buttonObject.transform.SetParent(parent, false);

        var layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.minWidth        = 28f;
        layoutElement.preferredWidth  = 32f;
        layoutElement.minHeight       = 26f;
        layoutElement.preferredHeight = 26f;

        var image  = buttonObject.GetComponent<Image>();
        var button = buttonObject.GetComponent<Button>();
        image.color          = Colors.FilterChip.Idle;
        button.targetGraphic = image;
        button.interactable  = enabled;

        if(enabled) button.onClick.AddListener(() => onClick());

        ApplyNavButtonState(button);

        var textObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(buttonObject.transform, false);
        RectLayout.StretchToParent(textObject.GetComponent<RectTransform>());

        var tmp = textObject.GetComponent<TextMeshProUGUI>();
        tmp.fontSize      = Typography.Body;
        tmp.color         = Colors.Text.Caption;
        tmp.alignment     = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;

        LocalizedTextView.BindKey(textObject.transform, labelKey, null);
    }

    private static void ApplyNavButtonState(Button button){
        var colors = button.colors;
        colors.disabledColor = new Color(0.12f, 0.12f, 0.12f, 0.35f);
        button.colors        = colors;
    }

    private static void CreatePeriodLabel(Transform parent, ScrollContentBuilder.ScrollTextStyle style){
        var labelObject = new GameObject(
                                         "FinancialPeriodLabel",
                                         typeof(RectTransform),
                                         typeof(TextMeshProUGUI),
                                         typeof(LayoutElement)
                                        );
        labelObject.transform.SetParent(parent, false);

        var layoutElement = labelObject.GetComponent<LayoutElement>();
        layoutElement.flexibleWidth   = 1f;
        layoutElement.minWidth        = 96f;
        layoutElement.minHeight       = 26f;
        layoutElement.preferredHeight = 26f;

        var tmp = labelObject.GetComponent<TextMeshProUGUI>();
        tmp.fontSize           = Typography.Body;
        tmp.color              = Colors.Text.Value;
        tmp.alignment          = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.overflowMode       = TextOverflowModes.Ellipsis;
        tmp.text               = ResolvePeriodLabel();

        if(style.Font) tmp.font = style.Font;
    }

    private static string ResolvePeriodLabel(){
        var (_, _, label) = PeriodRange.Resolve(
                                                NavigationState.FinancialPeriod,
                                                NavigationState.FinancialPeriodOffset,
                                                DateTime.UtcNow
                                               );

        return label;
    }

    private static void SelectPeriod(PeriodKind period, Action onChanged){
        NavigationState.SetFinancialPeriod(period);
        onChanged?.Invoke();
    }

    private static void ShiftPeriod(int delta, StashSnapshotIndex index, Action onChanged){
        if(!PeriodNavigation.CanShift(
                                      index,
                                      NavigationState.FinancialPeriod,
                                      NavigationState.FinancialPeriodOffset,
                                      delta
                                     ))
            return;

        NavigationState.ShiftFinancialPeriod(delta);
        onChanged?.Invoke();
    }

    private static void SelectView(ViewMode view, Action onChanged){
        NavigationState.SetFinancialView(view);
        onChanged?.Invoke();
    }

    private static void SelectStyle(ChartStyle style, Action onChanged){
        NavigationState.SetChartStyle(style);
        onChanged?.Invoke();
    }
}
