using System;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal static class TablePaginationView{
    public static void Add(
        Transform   parent, int pageIndex, int pageCount, ScrollContentBuilder.ScrollTextStyle style,
        Action<int> onPageChanged
    ){
        if(!parent || pageCount <= 1) return;

        var barObject = new GameObject(
                                       "FinancialTablePagination",
                                       typeof(RectTransform),
                                       typeof(HorizontalLayoutGroup),
                                       typeof(LayoutElement)
                                      );
        barObject.transform.SetParent(parent, false);

        var barLayout = barObject.GetComponent<HorizontalLayoutGroup>();
        barLayout.spacing                = 8f;
        barLayout.padding                = new RectOffset(4, 4, 6, 6);
        barLayout.childAlignment         = TextAnchor.MiddleCenter;
        barLayout.childControlWidth      = true;
        barLayout.childControlHeight     = true;
        barLayout.childForceExpandWidth  = false;
        barLayout.childForceExpandHeight = false;

        var barElement = barObject.GetComponent<LayoutElement>();
        barElement.minHeight       = Spacing.RowMinHeight;
        barElement.preferredHeight = Spacing.RowMinHeight;
        barElement.flexibleWidth   = 1f;

        var canPrev = pageIndex > 0;
        var canNext = pageIndex < pageCount - 1;

        CreateNavButton(
                        barObject.transform,
                        LocaleKeys.FinancialTablePagePrev,
                        canPrev,
                        style,
                        () => onPageChanged?.Invoke(pageIndex - 1)
                       );
        CreatePageLabel(barObject.transform, pageIndex, pageCount, style);
        CreateNavButton(
                        barObject.transform,
                        LocaleKeys.FinancialTablePageNext,
                        canNext,
                        style,
                        () => onPageChanged?.Invoke(pageIndex + 1)
                       );
    }

    private static void CreateNavButton(
        Transform parent, string labelKey, bool enabled, ScrollContentBuilder.ScrollTextStyle style, Action onClick
    ){
        var buttonObject = new GameObject(
                                          labelKey,
                                          typeof(RectTransform),
                                          typeof(Image),
                                          typeof(Button),
                                          typeof(LayoutElement)
                                         );
        buttonObject.transform.SetParent(parent, false);

        var layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.minWidth        = 72f;
        layoutElement.preferredWidth  = 72f;
        layoutElement.minHeight       = Spacing.RowMinHeight - 4f;
        layoutElement.preferredHeight = Spacing.RowMinHeight - 4f;

        var image  = buttonObject.GetComponent<Image>();
        var button = buttonObject.GetComponent<Button>();
        image.color          = Color.white;
        button.targetGraphic = image;
        button.transition    = Selectable.Transition.ColorTint;
        button.interactable  = enabled;
        ControlButtonColors.ApplyNav(button, enabled);

        if(enabled) button.onClick.AddListener(() => onClick());

        var labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);
        Stretch(labelObject.GetComponent<RectTransform>());

        var tmp = labelObject.GetComponent<TextMeshProUGUI>();
        tmp.fontSize           = Typography.Row;
        tmp.color              = Colors.Text.Caption;
        tmp.alignment          = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.raycastTarget      = false;

        if(style.Font) tmp.font = style.Font;

        LocalizedTextView.BindKey(labelObject.transform, labelKey, null);
    }

    private static void CreatePageLabel(
        Transform parent, int pageIndex, int pageCount, ScrollContentBuilder.ScrollTextStyle style
    ){
        var labelObject = new GameObject(
                                         "FinancialTablePageLabel",
                                         typeof(RectTransform),
                                         typeof(TextMeshProUGUI),
                                         typeof(LayoutElement)
                                        );
        labelObject.transform.SetParent(parent, false);

        var layoutElement = labelObject.GetComponent<LayoutElement>();
        layoutElement.flexibleWidth   = 1f;
        layoutElement.minWidth        = 80f;
        layoutElement.minHeight       = Spacing.RowMinHeight - 4f;
        layoutElement.preferredHeight = Spacing.RowMinHeight - 4f;

        var tmp = labelObject.GetComponent<TextMeshProUGUI>();
        tmp.text               = LocaleLoader.Format(LocaleKeys.FinancialTablePageIndicator, pageIndex + 1, pageCount);
        tmp.fontSize           = Typography.Row;
        tmp.color              = Colors.Text.Value;
        tmp.alignment          = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.raycastTarget      = false;

        if(style.Font) tmp.font = style.Font;
    }

    private static void Stretch(RectTransform rect){
        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.offsetMin        = Vector2.zero;
        rect.offsetMax        = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }
}
