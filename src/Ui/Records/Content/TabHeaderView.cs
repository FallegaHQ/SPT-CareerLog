using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Content;

internal static class TabHeaderView{
    private const float BackButtonWidth  = Spacing.BackButtonSize;
    private const float BackButtonHeight = Spacing.BackButtonSize;

    public static void Apply(
        Transform hostRoot, string titleKey, TextMeshProUGUI styleSource, bool showBack, UnityAction onBack,
        bool      showMapButton = false, UnityAction onMap = null
    ){
        if(!hostRoot) return;

        var header = hostRoot.Find(UiHierarchy.TabHost.FixedHeader);

        if(!header) return;

        EnsureChildren(header, styleSource);

        var backButton = header.Find(UiHierarchy.TabHost.BackButton);

        if(backButton){
            backButton.gameObject.SetActive(showBack);
            WireBackButton(backButton, showBack ? onBack : null);
        }

        var mapButton = header.Find(UiHierarchy.TabHost.MapButton);

        if(mapButton){
            mapButton.gameObject.SetActive(showMapButton);
            WireMapButton(mapButton, showMapButton ? onMap : null);
        }

        var title = header.Find(UiHierarchy.TabHost.TitleText);

        if(!title) return;

        LocalizedTextView.BindKey(title, titleKey, null);
        ApplyTitleStyle(title.GetComponent<TextMeshProUGUI>());
    }

    private static void ApplyTitleStyle(TextMeshProUGUI title){
        if(!title) return;

        title.color            = Colors.Text.SectionAccent;
        title.fontStyle        = FontStyles.Bold;
        title.characterSpacing = 4f;
    }

    private static void EnsureChildren(Transform header, TextMeshProUGUI styleSource){
        if(!header) return;

        var back = header.Find(UiHierarchy.TabHost.BackButton);

        if(back && !back.Find(UiHierarchy.TabHost.Label)) Object.Destroy(back.gameObject);

        if(!header.Find(UiHierarchy.TabHost.BackButton)) CreateBackButton(header, styleSource);

        if(!header.Find(UiHierarchy.TabHost.TitleText)) CreateTitleText(header, styleSource);

        if(!header.Find(UiHierarchy.TabHost.MapButton)) CreateMapButton(header, styleSource);
    }

    private static void WireBackButton(Transform backButton, UnityAction onBack){
        var btn = backButton.GetComponent<Button>();

        if(!btn) return;

        btn.onClick.RemoveAllListeners();
        btn.interactable = onBack != null;

        if(onBack != null) btn.onClick.AddListener(onBack);
    }

    private static void WireMapButton(Transform mapButton, UnityAction onMap){
        var btn = mapButton.GetComponent<Button>();

        if(!btn) return;

        btn.onClick.RemoveAllListeners();
        btn.interactable = onMap != null;

        if(onMap != null) btn.onClick.AddListener(onMap);

        var label = mapButton.Find(UiHierarchy.TabHost.Label)?.
                              GetComponent<TextMeshProUGUI>();

        if(label) label.text = LocaleLoader.Format(LocaleKeys.MapOpenButton);
    }

    private static void CreateBackButton(Transform header, TextMeshProUGUI styleSource){
        var backObject = new GameObject(
                                        UiHierarchy.TabHost.BackButton,
                                        typeof(RectTransform),
                                        typeof(Image),
                                        typeof(Button),
                                        typeof(LayoutElement)
                                       );
        backObject.transform.SetParent(header, false);
        backObject.transform.SetAsFirstSibling();

        var le = backObject.GetComponent<LayoutElement>();
        le.minWidth        = BackButtonWidth;
        le.preferredWidth  = BackButtonWidth;
        le.minHeight       = BackButtonHeight;
        le.preferredHeight = BackButtonHeight;
        le.flexibleWidth   = 0f;
        le.flexibleHeight  = 0f;

        var img = backObject.GetComponent<Image>();
        img.color         = Colors.Control.ButtonSurface;
        img.raycastTarget = true;

        var btn = backObject.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.transition    = Selectable.Transition.ColorTint;

        var colors = btn.colors;
        colors.normalColor      = Colors.Control.ButtonSurface;
        colors.highlightedColor = Colors.Control.ButtonHighlight;
        colors.pressedColor     = Colors.Control.ButtonPressed;
        colors.fadeDuration     = 0.08f;
        btn.colors              = colors;

        var labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(backObject.transform, false);
        StretchLabel(labelObject.GetComponent<RectTransform>());

        var tmp = labelObject.GetComponent<TextMeshProUGUI>();
        tmp.text               = "◀";
        tmp.fontSize           = 22f;
        tmp.color              = Colors.Text.SectionTitle;
        tmp.alignment          = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.raycastTarget      = false;

        if(styleSource && styleSource.font) tmp.font = styleSource.font;
    }

    private static void CreateTitleText(Transform header, TextMeshProUGUI styleSource){
        var titleObject = new GameObject(
                                         UiHierarchy.TabHost.TitleText,
                                         typeof(RectTransform),
                                         typeof(TextMeshProUGUI),
                                         typeof(LayoutElement)
                                        );
        titleObject.transform.SetParent(header, false);
        titleObject.transform.SetAsLastSibling();

        var le = titleObject.GetComponent<LayoutElement>();
        le.flexibleWidth   = 1f;
        le.minHeight       = Spacing.SectionHeaderMinHeight;
        le.preferredHeight = Spacing.SectionHeaderMinHeight;

        var tmp = titleObject.GetComponent<TextMeshProUGUI>();
        tmp.text     = string.Empty;
        tmp.fontSize = Typography.SectionTitle;
        ApplyTitleStyle(tmp);
        tmp.alignment          = TextAlignmentOptions.MidlineLeft;
        tmp.enableWordWrapping = false;
        tmp.raycastTarget      = false;
        tmp.margin             = Vector4.zero;

        if(styleSource && styleSource.font) tmp.font = styleSource.font;
    }

    private static void CreateMapButton(Transform header, TextMeshProUGUI styleSource){
        var mapObject = new GameObject(
                                       UiHierarchy.TabHost.MapButton,
                                       typeof(RectTransform),
                                       typeof(Image),
                                       typeof(Button),
                                       typeof(LayoutElement)
                                      );
        mapObject.transform.SetParent(header, false);
        mapObject.transform.SetAsLastSibling();

        var le = mapObject.GetComponent<LayoutElement>();
        le.minWidth        = BackButtonWidth + 28f;
        le.preferredWidth  = BackButtonWidth + 28f;
        le.minHeight       = BackButtonHeight;
        le.preferredHeight = BackButtonHeight;
        le.flexibleWidth   = 0f;
        le.flexibleHeight  = 0f;

        var img = mapObject.GetComponent<Image>();
        img.color         = Colors.Control.ButtonSurface;
        img.raycastTarget = true;

        var btn = mapObject.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.transition    = Selectable.Transition.ColorTint;

        var colors = btn.colors;
        colors.normalColor      = Colors.Control.ButtonSurface;
        colors.highlightedColor = Colors.Control.ButtonHighlight;
        colors.pressedColor     = Colors.Control.ButtonPressed;
        colors.fadeDuration     = 0.08f;
        btn.colors              = colors;

        var labelObject = new GameObject(UiHierarchy.TabHost.Label, typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(mapObject.transform, false);
        StretchLabel(labelObject.GetComponent<RectTransform>());

        var tmp = labelObject.GetComponent<TextMeshProUGUI>();
        tmp.text               = LocaleLoader.Format(LocaleKeys.MapOpenButton);
        tmp.fontSize           = 14f;
        tmp.color              = Colors.Text.SectionAccent;
        tmp.alignment          = TextAlignmentOptions.Center;
        tmp.fontStyle          = FontStyles.Bold;
        tmp.enableWordWrapping = false;
        tmp.raycastTarget      = false;

        if(styleSource && styleSource.font) tmp.font = styleSource.font;
    }

    private static void StretchLabel(RectTransform rect){
        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.offsetMin        = Vector2.zero;
        rect.offsetMax        = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }
}
