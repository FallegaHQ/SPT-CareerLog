using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Map;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Formatting;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Map;

/// <summary>Full-screen modal overlay for viewing a raid map from RECORDS raid detail.</summary>
internal static class MapModal{
    private static GameObject _overlayRoot;
    private static PanelView  _mapPanel;

    public static bool IsOpen => _overlayRoot && _overlayRoot.activeSelf;

    public static void Show(Transform screenRoot, RaidRecord raid, TextMeshProUGUI styleSource){
        if(!screenRoot || raid == null) return;

        Hide();

        _overlayRoot = BuildOverlay(screenRoot, raid, styleSource);
        _overlayRoot.SetActive(true);
    }

    public static void Hide(){
        _mapPanel?.SaveMarkerFilterPrefs();

        if(!_overlayRoot) return;

        Object.Destroy(_overlayRoot);
        _overlayRoot = null;
        _mapPanel    = null;
    }

    private static GameObject BuildOverlay(Transform screenRoot, RaidRecord raid, TextMeshProUGUI styleSource){
        var overlay = new GameObject(UiHierarchy.MapModal.Overlay, typeof(RectTransform), typeof(Image));
        overlay.transform.SetParent(screenRoot, false);
        overlay.transform.SetAsLastSibling();

        var overlayRect = overlay.GetComponent<RectTransform>();
        Stretch(overlayRect);

        var scrim = overlay.GetComponent<Image>();
        scrim.color         = new Color(0f, 0f, 0f, 0.72f);
        scrim.raycastTarget = true;

        var dialog = new GameObject(
                                    UiHierarchy.MapModal.Dialog,
                                    typeof(RectTransform),
                                    typeof(Image),
                                    typeof(VerticalLayoutGroup),
                                    typeof(LayoutElement)
                                   );
        dialog.transform.SetParent(overlay.transform, false);

        var dialogRect = dialog.GetComponent<RectTransform>();
        dialogRect.anchorMin        = new Vector2(0.05f, 0.06f);
        dialogRect.anchorMax        = new Vector2(0.95f, 0.94f);
        dialogRect.offsetMin        = Vector2.zero;
        dialogRect.offsetMax        = Vector2.zero;
        dialogRect.anchoredPosition = Vector2.zero;

        var dialogImage = dialog.GetComponent<Image>();
        dialogImage.color         = Colors.Surface.PanelBackground;
        dialogImage.raycastTarget = true;

        var dialogLayout = dialog.GetComponent<VerticalLayoutGroup>();
        dialogLayout.spacing                = 8f;
        dialogLayout.padding                = new RectOffset(14, 14, 12, 14);
        dialogLayout.childAlignment         = TextAnchor.UpperLeft;
        dialogLayout.childControlWidth      = true;
        dialogLayout.childControlHeight     = true;
        dialogLayout.childForceExpandWidth  = true;
        dialogLayout.childForceExpandHeight = false;

        CreateHeaderRow(dialog.transform, raid, styleSource);
        _mapPanel = CreateMapViewport(dialog.transform, styleSource);

        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogRect);
        _mapPanel?.Show(raid);

        return overlay;
    }

    private static void CreateHeaderRow(Transform parent, RaidRecord raid, TextMeshProUGUI styleSource){
        var header = new GameObject(
                                    "Header",
                                    typeof(RectTransform),
                                    typeof(HorizontalLayoutGroup),
                                    typeof(LayoutElement)
                                   );
        header.transform.SetParent(parent, false);

        var headerLayout = header.GetComponent<HorizontalLayoutGroup>();
        headerLayout.spacing                = 10f;
        headerLayout.childAlignment         = TextAnchor.MiddleLeft;
        headerLayout.childControlWidth      = true;
        headerLayout.childControlHeight     = true;
        headerLayout.childForceExpandWidth  = false;
        headerLayout.childForceExpandHeight = false;

        var headerElement = header.GetComponent<LayoutElement>();
        headerElement.minHeight       = Spacing.SectionHeaderMinHeight;
        headerElement.preferredHeight = Spacing.SectionHeaderMinHeight;
        headerElement.flexibleWidth   = 1f;
        headerElement.flexibleHeight  = 0f;

        var titleObject = new GameObject(
                                         "Title",
                                         typeof(RectTransform),
                                         typeof(LayoutElement),
                                         typeof(TextMeshProUGUI)
                                        );
        titleObject.transform.SetParent(header.transform, false);

        var titleElement = titleObject.GetComponent<LayoutElement>();
        titleElement.flexibleWidth = 1f;

        var locationLabel = LocationNameResolver.Resolve(raid.LocationId);
        var titleText     = titleObject.GetComponent<TextMeshProUGUI>();
        ScrollRowBuilder.ConfigureText(
                                       titleText,
                                       LocaleLoader.Format(LocaleKeys.MapTitle, locationLabel),
                                       Typography.SectionTitle,
                                       Colors.Text.SectionAccent,
                                       styleSource ? styleSource.font : null,
                                       TextAlignmentOptions.MidlineLeft
                                      );
        titleText.fontStyle = FontStyles.Bold;

        CreateCloseButton(header.transform, styleSource);
    }

    private static void CreateCloseButton(Transform header, TextMeshProUGUI styleSource){
        var buttonObject = new GameObject(
                                          UiHierarchy.MapModal.CloseButton,
                                          typeof(RectTransform),
                                          typeof(Image),
                                          typeof(Button),
                                          typeof(LayoutElement)
                                         );
        buttonObject.transform.SetParent(header, false);

        var layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.minWidth        = Spacing.BackButtonSize + 12f;
        layoutElement.preferredWidth  = Spacing.BackButtonSize + 12f;
        layoutElement.minHeight       = Spacing.BackButtonSize;
        layoutElement.preferredHeight = Spacing.BackButtonSize;

        var image = buttonObject.GetComponent<Image>();
        image.color         = Colors.Control.ButtonSurface;
        image.raycastTarget = true;

        var button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(Hide);

        var labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);
        Stretch(labelObject.GetComponent<RectTransform>());

        var label = labelObject.GetComponent<TextMeshProUGUI>();
        ScrollRowBuilder.ConfigureText(
                                       label,
                                       LocaleLoader.Format(LocaleKeys.MapModalClose),
                                       16f,
                                       Colors.Text.SectionTitle,
                                       styleSource ? styleSource.font : null,
                                       TextAlignmentOptions.Center
                                      );
        label.raycastTarget = false;
    }

    private static PanelView CreateMapViewport(Transform dialog, TextMeshProUGUI styleSource){
        var viewportObject = new GameObject(
                                            UiHierarchy.MapModal.MapViewport,
                                            typeof(RectTransform),
                                            typeof(LayoutElement),
                                            typeof(Image)
                                           );
        viewportObject.transform.SetParent(dialog, false);

        var viewportElement = viewportObject.GetComponent<LayoutElement>();
        viewportElement.flexibleHeight = 1f;
        viewportElement.minHeight      = 320f;

        var viewportImage = viewportObject.GetComponent<Image>();
        viewportImage.color         = Colors.Surface.ItemsPanelBackdrop;
        viewportImage.raycastTarget = false;

        var panel = PanelView.Ensure(viewportObject.transform);
        panel?.SetLabelFont(styleSource ? styleSource.font : null);

        return panel;
    }

    private static void Stretch(RectTransform rect){
        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.offsetMin        = Vector2.zero;
        rect.offsetMax        = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }
}
