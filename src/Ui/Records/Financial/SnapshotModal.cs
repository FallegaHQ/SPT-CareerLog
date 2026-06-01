using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal static class SnapshotModal{
    private static GameObject _overlayRoot;

    public static bool IsOpen => _overlayRoot && _overlayRoot.activeSelf;

    public static void Show(Transform screenRoot, StashSnapshot snapshot, TextMeshProUGUI styleSource){
        if(!screenRoot || snapshot == null) return;

        Hide();

        _overlayRoot = BuildOverlay(screenRoot, snapshot, styleSource);
        _overlayRoot.SetActive(true);
    }

    public static void Hide(){
        if(!_overlayRoot) return;

        Object.Destroy(_overlayRoot);
        _overlayRoot = null;
    }

    private static GameObject BuildOverlay(Transform screenRoot, StashSnapshot snapshot, TextMeshProUGUI styleSource){
        var style = ScrollContentBuilder.ResolveStyle(styleSource);

        var overlay = new GameObject("SnapshotModal", typeof(RectTransform), typeof(Image));
        overlay.transform.SetParent(screenRoot, false);
        overlay.transform.SetAsLastSibling();

        Stretch(overlay.GetComponent<RectTransform>());

        var scrim = overlay.GetComponent<Image>();
        scrim.color         = new Color(0f, 0f, 0f, 0.72f);
        scrim.raycastTarget = true;

        var dialog = new GameObject(
                                    "Dialog",
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

        dialog.GetComponent<Image>().
               color = Colors.Surface.PanelBackground;

        var dialogLayout = dialog.GetComponent<VerticalLayoutGroup>();
        dialogLayout.spacing                = 8f;
        dialogLayout.padding                = new RectOffset(14, 14, 12, 14);
        dialogLayout.childAlignment         = TextAnchor.UpperLeft;
        dialogLayout.childControlWidth      = true;
        dialogLayout.childControlHeight     = true;
        dialogLayout.childForceExpandWidth  = true;
        dialogLayout.childForceExpandHeight = false;

        CreateHeaderRow(dialog.transform, snapshot, style);
        CreateBody(dialog.transform, snapshot, style);

        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogRect);

        return overlay;
    }

    private static void CreateHeaderRow(
        Transform parent, StashSnapshot snapshot, ScrollContentBuilder.ScrollTextStyle style
    ){
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

        var titleObject = new GameObject(
                                         "Title",
                                         typeof(RectTransform),
                                         typeof(LayoutElement),
                                         typeof(TextMeshProUGUI)
                                        );
        titleObject.transform.SetParent(header.transform, false);

        titleObject.GetComponent<LayoutElement>().
                    flexibleWidth = 1f;

        var titleText = titleObject.GetComponent<TextMeshProUGUI>();
        ScrollRowBuilder.ConfigureText(
                                       titleText,
                                       LocaleLoader.Format(
                                                           LocaleKeys.FinancialSnapshotTitle,
                                                           ValueFormatter.LocalTime(snapshot.Utc)
                                                          ),
                                       Typography.SectionTitle,
                                       Colors.Text.SectionAccent,
                                       style.Font,
                                       TextAlignmentOptions.MidlineLeft
                                      );
        titleText.fontStyle = FontStyles.Bold;

        CreateCloseButton(header.transform, style);
    }

    private static void CreateCloseButton(Transform header, ScrollContentBuilder.ScrollTextStyle style){
        var buttonObject = new GameObject(
                                          "Close",
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

        var image  = buttonObject.GetComponent<Image>();
        var button = buttonObject.GetComponent<Button>();
        image.color          = Colors.Control.ButtonSurface;
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
                                       style.Font,
                                       TextAlignmentOptions.Center
                                      );
        label.raycastTarget = false;
    }

    private static void CreateBody(
        Transform parent, StashSnapshot snapshot, ScrollContentBuilder.ScrollTextStyle style
    ){
        var scrollObject = new GameObject("Scroll", typeof(RectTransform), typeof(ScrollRect), typeof(LayoutElement));
        scrollObject.transform.SetParent(parent, false);

        var scrollLayout = scrollObject.GetComponent<LayoutElement>();
        scrollLayout.flexibleHeight  = 1f;
        scrollLayout.minHeight       = 240f;
        scrollLayout.preferredHeight = 360f;

        var viewportObject = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewportObject.transform.SetParent(scrollObject.transform, false);
        Stretch(viewportObject.GetComponent<RectTransform>());
        viewportObject.GetComponent<Image>().
                       color = Colors.Surface.ScrollViewport;

        var contentObject = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup));
        contentObject.transform.SetParent(viewportObject.transform, false);
        var contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin        = new Vector2(0f,   1f);
        contentRect.anchorMax        = new Vector2(1f,   1f);
        contentRect.pivot            = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta        = new Vector2(0f, 0f);

        ScrollContentBuilder.EnsureContentLayout(contentObject.transform);

        var items = ScrollContentBuilder.AddItemsPanel(contentObject.transform);

        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialTotalWorth,
                                                 ValueFormatter.Rubles(snapshot.TotalWorth),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialLiquidRubles,
                                                 ValueFormatter.Rubles(snapshot.Rubles),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialItemsValue,
                                                 ValueFormatter.Rubles(snapshot.ItemsValue),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialEquippedValue,
                                                 ValueFormatter.Rubles(snapshot.EquippedValue),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialInventoryValue,
                                                 ValueFormatter.Rubles(snapshot.InventoryValue),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialItemCount,
                                                 snapshot.ItemCount.ToString(),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialPeriodDelta,
                                                 ValueFormatter.SignedRubles(snapshot.DeltaRubles),
                                                 style
                                                );

        if(snapshot.TopItems is{
                                   Count: > 0
                               }){
            ScrollContentBuilder.AddSubHeader(items, LocaleLoader.Format(LocaleKeys.FinancialTopItemsHeader), style);
            TopItemRows.AddAll(items, snapshot.TopItems, style);
        }

        if(snapshot.Notes is{
                                Count: > 0
                            }){
            ScrollContentBuilder.AddSubHeader(
                                              items,
                                              LocaleLoader.Format(LocaleKeys.FinancialSnapshotNotesHeader),
                                              style
                                             );

            foreach(var note in snapshot.Notes)
                if(!string.IsNullOrEmpty(note))
                    ScrollContentBuilder.AddBodyLine(items, note, style);
        }

        ScrollContentBuilder.RebuildLayout(contentObject.transform);

        var scrollRect = scrollObject.GetComponent<ScrollRect>();
        scrollRect.viewport     = viewportObject.GetComponent<RectTransform>();
        scrollRect.content      = contentRect;
        scrollRect.horizontal   = false;
        scrollRect.vertical     = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
    }

    private static void Stretch(RectTransform rect){
        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.offsetMin        = Vector2.zero;
        rect.offsetMax        = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }
}
