using System.IO;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Map.Chrome;
using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Map.Markers;
using Softwyx.CareerLog.Map.Panel;
using Softwyx.CareerLog.Map.Trail;
using Softwyx.CareerLog.Map.Viewport;
using Softwyx.CareerLog.Persistence.Models;
using TMPro;
using UnityEngine;

namespace Softwyx.CareerLog.Map;

internal sealed class PanelView : MonoBehaviour{
    private const string             HostName = "RaidMapHost";
    private       RaidRecord         _activeRecord;
    private       ChromeHost         _chrome;
    private       LocationDefinition _definition;
    private       TextMeshProUGUI    _fallbackLabel;

    private HierarchyBuilder       _hierarchy;
    private TMP_FontAsset          _labelFont;
    private ScreenOverlay          _markerOverlay;
    private RaidMovementIndex      _movementIndex;
    private RaidPlaybackController _playback;
    private PopoverHost            _popoverHost;
    private TrailPresenter         _trailPresenter;
    private TrailZoomSync          _trailZoomSync;
    private VictimOverlay          _victimOverlay;

    private void Awake(){
        _trailPresenter ??= new TrailPresenter(this);
        _markerOverlay  ??= GetComponent<ScreenOverlay>()          ?? gameObject.AddComponent<ScreenOverlay>();
        _trailZoomSync  ??= GetComponent<TrailZoomSync>()          ?? gameObject.AddComponent<TrailZoomSync>();
        _playback       ??= GetComponent<RaidPlaybackController>() ?? gameObject.AddComponent<RaidPlaybackController>();
    }

    private void OnDestroy(){
        _trailPresenter?.Stop();

        if(_hierarchy?.TrailTexture) Destroy(_hierarchy.TrailTexture);
    }

    public static PanelView Ensure(Transform viewport){
        if(!viewport) return null;

        var existing = viewport.Find(HostName)?.
                                GetComponent<PanelView>();

        if(existing) return existing;

        var hostGo = new GameObject(HostName, typeof(RectTransform)){
                                                                        layer = viewport.gameObject.layer
                                                                    };

        var hostRect = hostGo.GetComponent<RectTransform>();
        ViewportLayout.StretchFill(hostRect, viewport);

        var popover = hostGo.AddComponent<PopoverHost>();
        var zoom    = hostGo.AddComponent<ViewportZoom>();
        hostGo.AddComponent<ScreenOverlay>();
        hostGo.AddComponent<VictimOverlay>();
        var panel = hostGo.AddComponent<PanelView>();
        panel.BindHostComponents(popover, zoom);

        return panel;
    }

    private void BindHostComponents(PopoverHost popover, ViewportZoom zoom){
        _popoverHost    =   popover;
        _victimOverlay  =   GetComponent<VictimOverlay>();
        _trailPresenter ??= new TrailPresenter(this);
        _markerOverlay  ??= GetComponent<ScreenOverlay>();
        zoom.SetDismissPopover(
                               () => ResolvePopoverHost()?.
                                   Hide()
                              );
    }

    public void SetLabelFont(TMP_FontAsset font){
        if(font) _labelFont = font;
    }

    public void Show(RaidRecord record){
        _trailPresenter?.Stop();
        ClearMapContent();

        _activeRecord  = record;
        _movementIndex = RaidMovementIndex.Build(record?.Movement, record?.DurationSeconds ?? 0f);
        _definition    = LocationRegistry.TryGet(record?.LocationId, out var def) ? def : null;

        if(_definition == null || !File.Exists(_definition.SvgPath)){
            ShowFallback(LocaleLoader.Format(LocaleKeys.MapUnavailable));

            return;
        }

        BuildMapVisual();
        PrepareMapMarkers();

        var zoom     = GetComponent<ViewportZoom>();
        var ctx      = CreateTrailContext();
        var viewport = transform.parent as RectTransform;

        DestroyStaleChrome(viewport);
        _chrome = ChromeHost.Ensure(viewport, _labelFont);
        zoom?.SetControlsHintVisible(true);
        zoom?.SetControlsHintBottomOffset(58f);
        _playback?.Bind(_trailPresenter, ctx, zoom, _movementIndex);
        _chrome?.Bind(_playback, zoom);
        _playback?.Seek(_movementIndex?.DurationSec ?? 0f);

        BindTrailZoom(zoom, ctx);
    }

    public void RestartPlayback(){
        if(_playback == null || _movementIndex == null) return;

        _playback.Restart();
    }

    public void SaveMarkerFilterPrefs(){
        _playback?.PersistMarkerPrefs();
    }

    private void BindTrailZoom(ViewportZoom zoom, TrailContext ctx){
        if(!_trailZoomSync
        || ctx?.TrailPoints is not{
                                      Count: > 1
                                  })
            return;

        _trailZoomSync.Bind(zoom, ctx, () => _trailPresenter.DrawTimeSec);
    }

    public void Hide(){
        SaveMarkerFilterPrefs();
        _playback?.StopPlayback();
        _playback?.Unbind();
        _chrome?.Hide();
        _trailPresenter?.Stop();
        ClearMapContent();
    }

    private void BuildMapVisual(){
        _hierarchy = new HierarchyBuilder();
        _hierarchy.Build(transform, _definition);
        _hierarchy.FitToViewport(transform as RectTransform, _definition);

        var zoom = GetComponent<ViewportZoom>();
        zoom?.Bind(transform as RectTransform, _hierarchy.Host);

        var sizer = GetComponent<ViewportSizer>() ?? gameObject.AddComponent<ViewportSizer>();
        sizer.SetTarget(_hierarchy.Host, _definition);

        _markerOverlay?.Bind(_hierarchy.TrailOverlay.rectTransform, _definition);
        _victimOverlay?.Bind(_hierarchy.TrailOverlay.rectTransform, _definition);
        ResolvePopoverHost()?.
            BindVictimOverlay(_victimOverlay);
    }

    private void PrepareMapMarkers(){
        var popover = ResolvePopoverHost();

        if(!popover) return;

        popover.PreparePresentation(transform.parent as RectTransform, _labelFont);
    }

    private TrailContext CreateTrailContext(){
        var zoom = GetComponent<ViewportZoom>();

        return new TrailContext{
                                   Texture                = _hierarchy?.TrailTexture,
                                   Definition             = _definition,
                                   MovementIndex          = _movementIndex,
                                   TrailPoints            = _movementIndex?.TrailPoints,
                                   TrailAnchor            = _hierarchy?.TrailOverlay?.rectTransform,
                                   MarkerOverlay          = _markerOverlay,
                                   Record                 = _activeRecord,
                                   PopoverHost            = ResolvePopoverHost(),
                                   LineWidthBaselineScale = zoom ? zoom.CombinedScale : 1f
                               };
    }

    private PopoverHost ResolvePopoverHost(){
        if(_popoverHost) return _popoverHost;

        _popoverHost = GetComponent<PopoverHost>();

        return _popoverHost;
    }

    private static void DestroyStaleChrome(RectTransform viewport){
        if(!viewport) return;

        var chrome = viewport.Find("MapChrome");

        if(chrome) Destroy(chrome.gameObject);
    }

    private void ShowFallback(string message){
        var labelGo = new GameObject("MapFallback", typeof(RectTransform), typeof(TextMeshProUGUI)){
                          layer = gameObject.layer
                      };

        var rect = labelGo.GetComponent<RectTransform>();
        ViewportLayout.StretchFill(rect, transform);

        _fallbackLabel               = labelGo.GetComponent<TextMeshProUGUI>();
        _fallbackLabel.alignment     = TextAlignmentOptions.Center;
        _fallbackLabel.fontSize      = 22f;
        _fallbackLabel.color         = Color.white;
        _fallbackLabel.text          = message;
        _fallbackLabel.raycastTarget = false;
    }

    private void ClearMapContent(){
        _playback?.StopPlayback();
        ResolvePopoverHost()?.
            Hide();
        _markerOverlay?.ClearIcons();
        _trailZoomSync?.Clear();

        for(var i = transform.childCount - 1; i >= 0; i--){
            var child = transform.GetChild(i);

            if(child.name == PopoverHost.PopoverObjectName
            || child.name == ViewportZoom.BackdropObjectName
            || child.GetComponent<ScreenOverlay>())
                continue;

            Destroy(child.gameObject);
        }

        if(_hierarchy?.TrailTexture) Destroy(_hierarchy.TrailTexture);

        _hierarchy     = null;
        _movementIndex = null;
        _definition    = null;
        _activeRecord  = null;
        _fallbackLabel = null;
    }
}
