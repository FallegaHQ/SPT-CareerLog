using System;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Chrome;

internal sealed class PlaybackBarView : MonoBehaviour{
    private const float BarHeight      = 44f;
    private const float ScrubberHeight = 10f;
    private const float HandleWidth    = 8f;
    private const float HandleHeight   = 12f;
    private const float ControlBtnSize = 28f;

    private RaidPlaybackController _playback;
    private TextMeshProUGUI        _timeLabel;
    private TextMeshProUGUI        _playLabel;
    private Slider                 _scrubber;
    private Button                 _playButton;
    private Button[]               _speedButtons;
    private bool                   _syncingScrubber;

    public static PlaybackBarView Ensure(RectTransform parent, TMP_FontAsset font){
        var existing = parent.Find("MapPlaybackBar")?.
                              GetComponent<PlaybackBarView>();

        if(existing) Destroy(existing.gameObject);

        var root = new GameObject("MapPlaybackBar", typeof(RectTransform), typeof(Image)){
                       layer = parent.gameObject.layer
                   };

        var rect = root.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin        = new Vector2(0f,   0f);
        rect.anchorMax        = new Vector2(1f,   0f);
        rect.pivot            = new Vector2(0.5f, 0f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta        = new Vector2(0f, BarHeight);

        var bg = root.GetComponent<Image>();
        bg.color         = new Color(0.04f, 0.04f, 0.04f, 0.82f);
        bg.raycastTarget = true;

        var view = root.AddComponent<PlaybackBarView>();
        view.Build(rect, font);

        return view;
    }

    public void Bind(RaidPlaybackController playback){
        if(_playback) _playback.StateChanged -= OnStateChanged;

        _playback = playback;

        if(_playback) _playback.StateChanged += OnStateChanged;

        Refresh();
    }

    private void OnDestroy(){
        if(_playback != null) _playback.StateChanged -= OnStateChanged;
    }

    private void Build(RectTransform root, TMP_FontAsset font){
        var row     = new GameObject("Row", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        var rowRect = row.GetComponent<RectTransform>();
        rowRect.SetParent(root, false);
        Stretch(rowRect);

        var layout = row.GetComponent<HorizontalLayoutGroup>();
        layout.padding                = new RectOffset(12, 12, 8, 8);
        layout.spacing                = 8f;
        layout.childAlignment         = TextAnchor.MiddleLeft;
        layout.childControlWidth      = true;
        layout.childControlHeight     = true;
        layout.childForceExpandWidth  = false;
        layout.childForceExpandHeight = false;

        _timeLabel = CreateLabel(row.transform, "00:00", 52f, font, TextAlignmentOptions.MidlineLeft);

        CreateIconButton(row.transform, "❙◀", 36f, OnRewindClicked);
        _playButton = CreateIconButton(row.transform, "▶", 32f, OnPlayClicked);
        _playLabel  = _playButton.GetComponentInChildren<TextMeshProUGUI>();

        _scrubber = CreateScrubber(row.transform);
        BuildSpeedButtons(row.transform);
    }

    private void BuildSpeedButtons(Transform parent){
        var speedRoot = new GameObject("SpeedButtons", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        speedRoot.transform.SetParent(parent, false);

        var speedLayout = speedRoot.GetComponent<HorizontalLayoutGroup>();
        speedLayout.spacing                = 4f;
        speedLayout.childAlignment         = TextAnchor.MiddleRight;
        speedLayout.childControlWidth      = true;
        speedLayout.childControlHeight     = true;
        speedLayout.childForceExpandWidth  = false;
        speedLayout.childForceExpandHeight = false;

        var speedElement = speedRoot.AddComponent<LayoutElement>();
        speedElement.minWidth       = 126f;
        speedElement.preferredWidth = 126f;

        _speedButtons = new Button[PlaybackSpeeds.All.Length];

        for(var i = 0; i < PlaybackSpeeds.All.Length; i++){
            var speed = PlaybackSpeeds.All[i];
            var label = $"{speed:0}x";
            var btn   = CreateTextButton(speedRoot.transform, label, 38f, () => OnSpeedClicked(speed));
            _speedButtons[i] = btn;
        }
    }

    private Slider CreateScrubber(Transform parent){
        var scrubRoot = new GameObject("Scrubber", typeof(RectTransform), typeof(Slider), typeof(LayoutElement));
        scrubRoot.transform.SetParent(parent, false);

        var element = scrubRoot.GetComponent<LayoutElement>();
        element.flexibleWidth   = 1f;
        element.minWidth        = 120f;
        element.preferredHeight = ScrubberHeight;
        element.minHeight       = ScrubberHeight;

        var slider = scrubRoot.GetComponent<Slider>();
        slider.minValue     = 0f;
        slider.maxValue     = 1f;
        slider.wholeNumbers = false;

        var background = CreateSliderPart(scrubRoot.transform, "Background", new Color(0.2f, 0.2f, 0.2f, 0.9f));
        Stretch(background.GetComponent<RectTransform>());

        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        var fillRect = fillArea.GetComponent<RectTransform>();
        fillRect.SetParent(scrubRoot.transform, false);
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(4f,  0f);
        fillRect.offsetMax = new Vector2(-4f, 0f);

        var fill = CreateSliderPart(fillArea.transform, "Fill", new Color(0.55f, 0.52f, 0.38f, 0.95f));
        Stretch(fill.GetComponent<RectTransform>());

        var handleArea     = new GameObject("Handle Slide Area", typeof(RectTransform));
        var handleAreaRect = handleArea.GetComponent<RectTransform>();
        handleAreaRect.SetParent(scrubRoot.transform, false);
        Stretch(handleAreaRect);

        var handle     = CreateSliderPart(handleArea.transform, "Handle", new Color(0.92f, 0.88f, 0.72f, 1f));
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(HandleWidth, HandleHeight);

        slider.fillRect      = fill.GetComponent<RectTransform>();
        slider.handleRect    = handleRect;
        slider.targetGraphic = handle.GetComponent<Image>();
        slider.onValueChanged.AddListener(OnScrubChanged);

        return slider;
    }

    private static GameObject CreateSliderPart(Transform parent, string name, Color color){
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color         = color;
        image.raycastTarget = true;

        return go;
    }

    private void OnRewindClicked(){
        _playback?.Rewind();
    }

    private void OnPlayClicked(){
        _playback?.TogglePlay();
    }

    private void OnSpeedClicked(float speed){
        _playback?.SetSpeed(speed);
        RefreshSpeedButtons();
    }

    private void OnScrubChanged(float value){
        if(!_playback || _syncingScrubber) return;

        _playback.SeekNormalized(value);
    }

    private void OnStateChanged(){
        Refresh();
    }

    private void Refresh(){
        if(!_playback) return;

        if(_timeLabel) _timeLabel.text = TimeFormat(_playback.CurrentTime);

        if(_playLabel) _playLabel.text = _playback.IsPlaying ? "❚❚" : "▶";

        if(_scrubber){
            var duration = _playback.Duration;
            var value    = duration > 0f ? _playback.CurrentTime / duration : 0f;

            _syncingScrubber = true;
            _scrubber.value  = value;
            _syncingScrubber = false;
        }

        ChromeButtonStyle.Apply(_playButton, _playback.IsPlaying);
        RefreshSpeedButtons();
    }

    private void RefreshSpeedButtons(){
        if(_speedButtons == null || !_playback) return;

        for(var i = 0; i < _speedButtons.Length; i++){
            var active = Mathf.Approximately(_playback.Speed, PlaybackSpeeds.All[i]);
            ChromeButtonStyle.Apply(_speedButtons[i], active);

            var label = _speedButtons[i].
                GetComponentInChildren<TextMeshProUGUI>();

            if(label) label.color = active ? Colors.Text.SectionAccent : Colors.Text.SectionTitle;
        }
    }

    private static TextMeshProUGUI CreateLabel(
        Transform parent, string text, float width, TMP_FontAsset font, TextAlignmentOptions alignment
    ){
        var go = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
        go.transform.SetParent(parent, false);

        if(width > 0f){
            var element = go.GetComponent<LayoutElement>();
            element.minWidth       = width;
            element.preferredWidth = width;
        }

        var label = go.GetComponent<TextMeshProUGUI>();
        ScrollRowBuilder.ConfigureText(label, text, 15f, Colors.Text.Caption, font, alignment);
        label.raycastTarget = false;

        return label;
    }

    private static Button CreateIconButton(
        Transform parent, string label, float size, UnityEngine.Events.UnityAction onClick
    ){
        var button = CreateTextButton(parent, label, size, onClick);
        var text   = button.GetComponentInChildren<TextMeshProUGUI>();
        text.fontSize = label.Length > 1 ? 13f : 16f;
        ChromeButtonStyle.Apply(button);

        return button;
    }

    private static Button CreateTextButton(
        Transform parent, string label, float width, UnityEngine.Events.UnityAction onClick
    ){
        var go = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        go.transform.SetParent(parent, false);

        var element = go.GetComponent<LayoutElement>();
        element.minWidth        = width;
        element.preferredWidth  = width;
        element.preferredHeight = ControlBtnSize;
        element.minHeight       = ControlBtnSize;

        var image = go.GetComponent<Image>();
        image.color         = Colors.Control.ButtonSurface;
        image.raycastTarget = true;

        var button = go.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);
        ChromeButtonStyle.Apply(button);

        var labelGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelGo.transform.SetParent(go.transform, false);
        ConfigureButtonLabel(labelGo.GetComponent<RectTransform>(), labelGo.GetComponent<TextMeshProUGUI>(), label);

        return button;
    }

    private static void ConfigureButtonLabel(RectTransform rect, TextMeshProUGUI text, string label){
        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.offsetMin        = Vector2.zero;
        rect.offsetMax        = Vector2.zero;
        rect.pivot            = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        ScrollRowBuilder.ConfigureText(text, label, 14f, Colors.Text.SectionTitle, null, TextAlignmentOptions.Center);
        text.raycastTarget      = false;
        text.lineSpacing        = 0f;
        text.margin             = Vector4.zero;
        text.overflowMode       = TextOverflowModes.Overflow;
        text.enableWordWrapping = false;
    }

    private static void Stretch(RectTransform rect){
        rect.anchorMin     = Vector2.zero;
        rect.anchorMax     = Vector2.one;
        rect.offsetMin     = Vector2.zero;
        rect.offsetMax     = Vector2.zero;
        rect.localScale    = Vector3.one;
        rect.localPosition = Vector3.zero;
    }

    private static string TimeFormat(float secondsFromStart){
        if(secondsFromStart < 0f) secondsFromStart = 0f;

        var span = TimeSpan.FromSeconds(secondsFromStart);

        return span.TotalHours >= 1
                   ? $"{(int) span.TotalHours}:{span.Minutes:D2}:{span.Seconds:D2}"
                   : $"{span.Minutes}:{span.Seconds:D2}";
    }
}
