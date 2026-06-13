using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Markers;

/// <summary>Marker detail popover anchored to the clicked icon (viewport space, zoom-safe).</summary>
internal sealed class PopoverHost : MonoBehaviour{
    internal const string PopoverObjectName = "MapMarkerPopover";

    private const float PopoverMinWidth = 180f;
    private const float PopoverMaxWidth = 280f;

    private static readonly Vector2 DefaultPopoverSize = new(PopoverMinWidth, 48f);

    private RectTransform   _viewport;
    private RectTransform   _presentationRoot;
    private RectTransform   _popover;
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _bodyText;
    private TMP_FontAsset   _labelFont;
    private RectTransform      _activeMarker;
    private VictimOverlay      _victimOverlay;
    private PopoverPanelHover  _panelHover;

    private void Awake(){
        _viewport = transform as RectTransform;
        EnsurePopoverBuilt();
        Hide();
    }

    public void PreparePresentation(RectTransform presentationRoot, TMP_FontAsset labelFont){
        if(presentationRoot) _presentationRoot = presentationRoot;

        if(labelFont) _labelFont = labelFont;

        EnsurePopoverBuilt();
        ApplyLabelFont();
        Hide();
    }

    public void Toggle(RectTransform markerRect, RaidMovementValue marker, RaidRecord raid){
        EnsurePopoverBuilt();

        if(markerRect && _activeMarker == markerRect && _popover && _popover.gameObject.activeSelf){
            Hide();

            return;
        }

        Show(markerRect, marker, raid);
    }

    public void BindVictimOverlay(VictimOverlay overlay){
        _victimOverlay = overlay;
    }

    public void Hide(){
        if(_popover) _popover.gameObject.SetActive(false);

        _activeMarker = null;
        _victimOverlay?.Hide();
    }

    private void Show(RectTransform markerRect, RaidMovementValue marker, RaidRecord raid){
        EnsurePopoverBuilt();

        if(!markerRect
        || marker == null
        || !DetailText.TryGet(marker, raid, out var title, out var body)
        || !_popover
        || !_titleText
        || !_bodyText){
            Hide();

            return;
        }

        _activeMarker = markerRect;

        _titleText.text = title;
        _bodyText.text  = body;

        _panelHover?.ResetBackground();

        _popover.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_popover);

        PopoverPlacement.Apply(_popover, markerRect, PlacementRoot, DefaultPopoverSize);

        if(DetailText.IsKillFamily(marker))
            _victimOverlay?.Show(markerRect, marker);
        else
            _victimOverlay?.Hide();
    }

    private RectTransform PlacementRoot => _presentationRoot ? _presentationRoot : _viewport;

    private void EnsurePopoverBuilt(){
        if(!_viewport) _viewport = transform as RectTransform;

        var parent = PlacementRoot;

        if(_popover && _titleText && _bodyText && _popover.parent == parent) return;

        BuildPopover();
    }

    private void BuildPopover(){
        if(_popover) Destroy(_popover.gameObject);

        _popover    = null;
        _titleText  = null;
        _bodyText   = null;
        _panelHover = null;

        var parent = PlacementRoot;

        if(!parent) return;

        var root = new GameObject(
                                  PopoverObjectName,
                                  typeof(RectTransform),
                                  typeof(Image),
                                  typeof(VerticalLayoutGroup),
                                  typeof(ContentSizeFitter),
                                  typeof(LayoutElement)
                                 );
        root.transform.SetParent(parent, false);
        root.transform.SetAsLastSibling();

        _popover                  = root.GetComponent<RectTransform>();
        _popover.anchorMin        = new Vector2(0.5f, 0.5f);
        _popover.anchorMax        = new Vector2(0.5f, 0.5f);
        _popover.pivot            = new Vector2(0.5f, 0f);
        _popover.anchoredPosition = Vector2.zero;

        var panel = root.GetComponent<Image>();
        panel.color         = Colors.Map.PopoverBackground;
        panel.raycastTarget = true;

        _panelHover = root.AddComponent<PopoverPanelHover>();

        var layout = root.GetComponent<VerticalLayoutGroup>();
        layout.padding                = new RectOffset(12, 12, 10, 10);
        layout.spacing                = 4f;
        layout.childAlignment         = TextAnchor.UpperLeft;
        layout.childControlWidth      = true;
        layout.childControlHeight     = true;
        layout.childForceExpandWidth  = true;
        layout.childForceExpandHeight = false;

        var fitter = root.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

        var layoutElement = root.GetComponent<LayoutElement>();
        layoutElement.minWidth       = PopoverMinWidth;
        layoutElement.preferredWidth = PopoverMaxWidth;

        _titleText = CreateLabel(root.transform, 15f, Colors.Text.SectionAccent, FontStyles.Bold);
        _bodyText  = CreateLabel(root.transform, 13f, Colors.Text.Caption,       FontStyles.Normal);
        ApplyLabelFont();
        Hide();
    }

    private void ApplyLabelFont(){
        if(!_labelFont) return;

        if(_titleText) _titleText.font = _labelFont;

        if(_bodyText) _bodyText.font = _labelFont;
    }

    private static TextMeshProUGUI CreateLabel(Transform parent, float fontSize, Color color, FontStyles style){
        var go = new GameObject("Line", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
        go.transform.SetParent(parent, false);

        var text = go.GetComponent<TextMeshProUGUI>();
        text.fontSize           = fontSize;
        text.color              = color;
        text.fontStyle          = style;
        text.alignment          = TextAlignmentOptions.TopLeft;
        text.raycastTarget      = false;
        text.enableWordWrapping = true;

        var element = go.GetComponent<LayoutElement>();
        element.preferredWidth = PopoverMaxWidth - 24f;

        return text;
    }
}
