using Softwyx.CareerLog.Map.Viewport;
using TMPro;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Chrome;

internal sealed class ChromeHost : MonoBehaviour{
    private PlaybackBarView   _playbackBar;
    private ZoomRailView      _zoomRail;
    private MarkerTogglesView _markerToggles;

    public static ChromeHost Ensure(RectTransform viewport, TMP_FontAsset font){
        if(!viewport) return null;

        var existing = viewport.Find("MapChrome")?.
                                GetComponent<ChromeHost>();

        if(existing) Destroy(existing.gameObject);

        var root = new GameObject("MapChrome", typeof(RectTransform)){
                                                                         layer = viewport.gameObject.layer
                                                                     };

        var rect = root.GetComponent<RectTransform>();
        rect.SetParent(viewport, false);
        rect.SetAsLastSibling();
        ViewportLayout.StretchFill(rect, viewport);

        var host = root.AddComponent<ChromeHost>();
        host._playbackBar   = PlaybackBarView.Ensure(rect, font);
        host._zoomRail      = ZoomRailView.Ensure(rect);
        host._markerToggles = MarkerTogglesView.Ensure(rect, font);

        return host;
    }

    public void Bind(RaidPlaybackController playback, ViewportZoom zoom){
        _playbackBar?.Bind(playback);
        _zoomRail?.Bind(zoom);
        _markerToggles?.Bind(playback);
        gameObject.SetActive(true);
    }

    public void Hide(){
        gameObject.SetActive(false);
    }
}
