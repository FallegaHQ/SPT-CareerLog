using Softwyx.CareerLog.Map.Viewport;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Chrome;

internal sealed class ZoomRailView : MonoBehaviour{
    private const float RailWidth         = 32f;
    private const float RailHeight        = 216f;
    private const float HandleSize        = 8f;
    private const float ButtonHeight      = 22f;
    private const float RailPadding       = 2f;
    private const float SliderInsetTop    = ButtonHeight + RailPadding + 4f;
    private const float SliderInsetBottom = ButtonHeight + RailPadding + 4f;

    private ViewportZoom _zoom;
    private Slider       _slider;
    private bool         _syncing;

    public static ZoomRailView Ensure(RectTransform parent){
        var existing = parent.Find("MapZoomRail")?.
                              GetComponent<ZoomRailView>();

        if(existing) Destroy(existing.gameObject);

        var root = new GameObject("MapZoomRail", typeof(RectTransform), typeof(Image));
        var rect = root.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin        = new Vector2(0f,        0.5f);
        rect.anchorMax        = new Vector2(0f,        0.5f);
        rect.pivot            = new Vector2(0f,        0.5f);
        rect.anchoredPosition = new Vector2(8f,        24f);
        rect.sizeDelta        = new Vector2(RailWidth, RailHeight);

        var bg = root.GetComponent<Image>();
        bg.color         = new Color(0.04f, 0.04f, 0.04f, 0.82f);
        bg.raycastTarget = false;

        var view = root.AddComponent<ZoomRailView>();
        view.Build(rect);

        return view;
    }

    public void Bind(ViewportZoom zoom){
        _zoom = zoom;
        SyncFromZoom();
    }

    private void Update(){
        if(!_zoom || _syncing || !_slider) return;

        var normalized = _zoom.UserZoomNormalized;

        if(Mathf.Approximately(_slider.value, normalized)) return;

        _syncing      = true;
        _slider.value = normalized;
        _syncing      = false;
    }

    private void Build(RectTransform root){
        CreateRailButton(root, "ZoomIn",  "+", true,  () => _zoom?.StepZoomIn());
        CreateRailButton(root, "ZoomOut", "-", false, () => _zoom?.StepZoomOut());
        _slider = CreateVerticalSlider(root);
    }

    private Slider CreateVerticalSlider(RectTransform root){
        var go   = new GameObject("Slider", typeof(RectTransform), typeof(Slider));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(root, false);
        rect.SetAsFirstSibling();
        rect.anchorMin        = new Vector2(0.22f, 0f);
        rect.anchorMax        = new Vector2(0.78f, 1f);
        rect.offsetMin        = new Vector2(0f,    SliderInsetBottom);
        rect.offsetMax        = new Vector2(0f,    -SliderInsetTop);
        rect.anchoredPosition = Vector2.zero;

        var slider = go.GetComponent<Slider>();
        slider.direction    = Slider.Direction.BottomToTop;
        slider.minValue     = 0f;
        slider.maxValue     = 1f;
        slider.wholeNumbers = false;
        slider.value        = 0f;

        var track = CreatePart(go.transform, "Track", new Color(0.2f, 0.2f, 0.2f, 0.9f));
        Stretch(track.GetComponent<RectTransform>());

        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        var fillRect = fillArea.GetComponent<RectTransform>();
        fillRect.SetParent(go.transform, false);
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(1f,  1f);
        fillRect.offsetMax = new Vector2(-1f, -1f);

        var fill = CreatePart(fillArea.transform, "Fill", new Color(0.55f, 0.52f, 0.38f, 0.95f));
        Stretch(fill.GetComponent<RectTransform>());

        var handleArea     = new GameObject("Handle Slide Area", typeof(RectTransform));
        var handleAreaRect = handleArea.GetComponent<RectTransform>();
        handleAreaRect.SetParent(go.transform, false);
        Stretch(handleAreaRect);

        var handle     = CreatePart(handleArea.transform, "Handle", new Color(0.92f, 0.88f, 0.72f, 1f));
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(HandleSize, HandleSize);

        slider.fillRect      = fill.GetComponent<RectTransform>();
        slider.handleRect    = handleRect;
        slider.targetGraphic = handle.GetComponent<Image>();
        slider.onValueChanged.AddListener(OnSliderChanged);

        return slider;
    }

    private void OnSliderChanged(float value){
        if(_syncing || !_zoom) return;

        _zoom.SetUserZoomNormalized(value);
    }

    private void SyncFromZoom(){
        if(!_zoom || !_slider) return;

        _syncing      = true;
        _slider.value = _zoom.UserZoomNormalized;
        _syncing      = false;
    }

    private static void CreateRailButton(
        RectTransform root, string objectName, string label, bool top, System.Action onClick
    ){
        var go   = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(root, false);

        if(top){
            rect.anchorMin = new Vector2(0f,           1f);
            rect.anchorMax = new Vector2(1f,           1f);
            rect.pivot     = new Vector2(0.5f,         1f);
            rect.offsetMin = new Vector2(RailPadding,  -(ButtonHeight + RailPadding));
            rect.offsetMax = new Vector2(-RailPadding, -RailPadding);
        }
        else{
            rect.anchorMin = new Vector2(0f,           0f);
            rect.anchorMax = new Vector2(1f,           0f);
            rect.pivot     = new Vector2(0.5f,         0f);
            rect.offsetMin = new Vector2(RailPadding,  RailPadding);
            rect.offsetMax = new Vector2(-RailPadding, ButtonHeight + RailPadding);
        }

        var image = go.GetComponent<Image>();
        image.color         = Colors.Control.ButtonSurface;
        image.raycastTarget = true;

        var button = go.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => onClick?.Invoke());
        ChromeButtonStyle.Apply(button);

        var labelGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelGo.transform.SetParent(go.transform, false);
        ConfigureButtonLabel(labelGo.GetComponent<RectTransform>(), labelGo.GetComponent<TextMeshProUGUI>(), label);
    }

    private static void ConfigureButtonLabel(RectTransform rect, TextMeshProUGUI text, string label){
        Stretch(rect);

        ScrollRowBuilder.ConfigureText(text, label, 16f, Colors.Text.SectionTitle, null, TextAlignmentOptions.Center);
        text.raycastTarget      = false;
        text.lineSpacing        = 0f;
        text.margin             = Vector4.zero;
        text.overflowMode       = TextOverflowModes.Overflow;
        text.enableWordWrapping = false;
    }

    private static GameObject CreatePart(Transform parent, string name, Color color){
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().
           color = color;

        return go;
    }

    private static void Stretch(RectTransform rect){
        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.offsetMin        = Vector2.zero;
        rect.offsetMax        = Vector2.zero;
        rect.localScale       = Vector3.one;
        rect.anchoredPosition = Vector2.zero;
    }
}
