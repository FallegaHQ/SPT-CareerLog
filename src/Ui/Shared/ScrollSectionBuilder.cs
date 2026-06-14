using System;
using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Ui.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Shared;

/// <summary>Scroll section shells (header + items panel). Used only via <see cref="ScrollContentBuilder" />.</summary>
internal static class ScrollSectionBuilder{
    internal static Transform AddSection(
        Transform contentRoot, string title, ScrollContentBuilder.ScrollTextStyle style
    ){
        return AddSectionInternal(contentRoot, title, style, null);
    }

    internal static Transform AddItemsPanel(Transform contentRoot){
        var itemsObject = new GameObject(
                                         ScrollContentBuilder.ItemsObjectName,
                                         typeof(RectTransform),
                                         typeof(VerticalLayoutGroup),
                                         typeof(LayoutElement),
                                         typeof(Image)
                                        );
        itemsObject.transform.SetParent(contentRoot, false);
        ConfigureItemsPanel(itemsObject);

        return itemsObject.transform;
    }

    internal static Transform GetItemsContainer(Transform section){
        return section ? section.Find(ScrollContentBuilder.ItemsObjectName) : null;
    }

    private static Transform AddSectionInternal(
        Transform contentRoot, string title, ScrollContentBuilder.ScrollTextStyle style, Action onBack
    ){
        var sectionObject = new GameObject(
                                           ScrollContentBuilder.SectionObjectName,
                                           typeof(RectTransform),
                                           typeof(VerticalLayoutGroup),
                                           typeof(LayoutElement)
                                          );
        sectionObject.transform.SetParent(contentRoot, false);

        var sectionLayout = sectionObject.GetComponent<VerticalLayoutGroup>();
        sectionLayout.spacing                = Spacing.SectionInnerSpacing;
        sectionLayout.childAlignment         = TextAnchor.UpperLeft;
        sectionLayout.childControlWidth      = true;
        sectionLayout.childControlHeight     = true;
        sectionLayout.childForceExpandWidth  = true;
        sectionLayout.childForceExpandHeight = false;

        var sectionElement = sectionObject.GetComponent<LayoutElement>();
        sectionElement.flexibleWidth = 1f;

        if(onBack == null)
            CreateLocalizedHeaderText(sectionObject.transform, title, style);
        else
            CreateLocalizedHeaderWithBack(sectionObject.transform, title, style, onBack);

        var itemsObject = new GameObject(
                                         ScrollContentBuilder.ItemsObjectName,
                                         typeof(RectTransform),
                                         typeof(VerticalLayoutGroup),
                                         typeof(LayoutElement),
                                         typeof(Image)
                                        );
        itemsObject.transform.SetParent(sectionObject.transform, false);
        ConfigureItemsPanel(itemsObject);

        return sectionObject.transform;
    }

    private static void ConfigureItemsPanel(GameObject itemsObject){
        var itemsBackdrop = itemsObject.GetComponent<Image>();
        itemsBackdrop.color         = Colors.Surface.ItemsPanelBackdrop;
        itemsBackdrop.raycastTarget = false;

        var itemsLayout = itemsObject.GetComponent<VerticalLayoutGroup>();
        itemsLayout.spacing                = Spacing.ItemsVerticalSpacing;
        itemsLayout.padding                = new RectOffset(8, 8, 6, 8);
        itemsLayout.childAlignment         = TextAnchor.UpperLeft;
        itemsLayout.childControlWidth      = true;
        itemsLayout.childControlHeight     = true;
        itemsLayout.childForceExpandWidth  = true;
        itemsLayout.childForceExpandHeight = false;

        var itemsElement = itemsObject.GetComponent<LayoutElement>();
        itemsElement.flexibleWidth = 1f;
    }

    private static void CreateLocalizedHeaderText(
        Transform section, string localizationKey, ScrollContentBuilder.ScrollTextStyle style
    ){
        var headerObject = CreateHeaderShell(section);
        CreateHeaderTitle(headerObject.transform, localizationKey, style);
    }

    private static void CreateLocalizedHeaderWithBack(
        Transform section, string localizationKey, ScrollContentBuilder.ScrollTextStyle style, Action onBack
    ){
        var headerObject = CreateHeaderShell(section);
        CreateBackButton(headerObject.transform, onBack);
        CreateHeaderTitle(headerObject.transform, localizationKey, style);
    }

    private static GameObject CreateHeaderShell(Transform section){
        var headerObject = new GameObject(
                                          ScrollContentBuilder.HeaderObjectName,
                                          typeof(RectTransform),
                                          typeof(LayoutElement),
                                          typeof(Image),
                                          typeof(HorizontalLayoutGroup)
                                         );
        headerObject.transform.SetParent(section, false);

        var headerElement = headerObject.GetComponent<LayoutElement>();
        headerElement.minHeight       = Spacing.SectionHeaderMinHeight;
        headerElement.preferredHeight = Spacing.SectionHeaderMinHeight;
        headerElement.flexibleWidth   = 1f;

        var backdrop = headerObject.GetComponent<Image>();
        backdrop.color         = Colors.Surface.HeaderBackdrop;
        backdrop.raycastTarget = false;

        ApplyHeaderRowLayout(headerObject.GetComponent<HorizontalLayoutGroup>());

        return headerObject;
    }

    private static void ApplyHeaderRowLayout(HorizontalLayoutGroup layout){
        if(!layout) return;

        layout.spacing                = 8f;
        layout.padding                = new RectOffset(10, 10, 0, 0);
        layout.childAlignment         = TextAnchor.MiddleLeft;
        layout.childControlWidth      = true;
        layout.childControlHeight     = true;
        layout.childForceExpandWidth  = false;
        layout.childForceExpandHeight = true;
    }

    private static void CreateHeaderTitle(
        Transform parent, string localizationKey, ScrollContentBuilder.ScrollTextStyle style
    ){
        var titleObject = new GameObject(
                                         "Title",
                                         typeof(RectTransform),
                                         typeof(LayoutElement),
                                         typeof(TextMeshProUGUI)
                                        );
        titleObject.transform.SetParent(parent, false);

        var titleElement = titleObject.GetComponent<LayoutElement>();
        titleElement.flexibleWidth   = 1f;
        titleElement.minHeight       = Spacing.SectionHeaderMinHeight;
        titleElement.preferredHeight = Spacing.SectionHeaderMinHeight;

        var text = titleObject.GetComponent<TextMeshProUGUI>();
        ScrollRowBuilder.ConfigureText(
                                       text,
                                       string.Empty,
                                       Typography.SectionTitle,
                                       Colors.Text.SectionAccent,
                                       style.Font,
                                       TextAlignmentOptions.MidlineLeft
                                      );
        text.fontStyle        = FontStyles.Bold;
        text.characterSpacing = 4f;
        text.margin           = Vector4.zero;
        text.raycastTarget    = false;

        LocalizedTextView.BindKey(titleObject.transform, localizationKey, null);
    }

    private static void CreateBackButton(Transform parent, Action onBack){
        var buttonObject = new GameObject(
                                          UiHierarchy.TabHost.BackButton,
                                          typeof(RectTransform),
                                          typeof(Image),
                                          typeof(Button),
                                          typeof(LayoutElement)
                                         );
        buttonObject.transform.SetParent(parent, false);
        buttonObject.transform.SetAsFirstSibling();

        var layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.minWidth        = Spacing.BackButtonSize;
        layoutElement.preferredWidth  = Spacing.BackButtonSize;
        layoutElement.minHeight       = Spacing.BackButtonSize;
        layoutElement.preferredHeight = Spacing.BackButtonSize;
        layoutElement.flexibleWidth   = 0f;
        layoutElement.flexibleHeight  = 0f;

        var image = buttonObject.GetComponent<Image>();
        image.color         = Colors.Control.ButtonSurface;
        image.raycastTarget = true;

        var button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.transition    = Selectable.Transition.ColorTint;
        button.onClick.AddListener(() => onBack?.Invoke());

        var colors = button.colors;
        colors.normalColor      = Colors.Control.ButtonSurface;
        colors.highlightedColor = Colors.Control.ButtonHighlight;
        colors.pressedColor     = Colors.Control.ButtonPressed;
        colors.fadeDuration     = 0.08f;
        button.colors           = colors;

        var labelObject = new GameObject(UiHierarchy.TabHost.Label, typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);
        StretchButtonLabel(labelObject.GetComponent<RectTransform>());

        var tmp = labelObject.GetComponent<TextMeshProUGUI>();
        ScrollRowBuilder.ConfigureText(
                                       tmp,
                                       "◀",
                                       Typography.SectionTitle,
                                       Colors.Text.SectionTitle,
                                       null,
                                       TextAlignmentOptions.Center
                                      );
        tmp.raycastTarget      = false;
        tmp.enableWordWrapping = false;
    }

    private static void StretchButtonLabel(RectTransform rect){
        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.offsetMin        = Vector2.zero;
        rect.offsetMax        = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }
}
