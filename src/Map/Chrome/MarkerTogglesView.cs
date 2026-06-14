using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Map.Markers;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Chrome;

internal sealed class MarkerTogglesView : MonoBehaviour{
    private const float    ToggleSize = 18f;
    private       Toggle[] _markerToggles;

    private RaidPlaybackController _playback;
    private Toggle                 _rememberToggle;
    private bool                   _syncing;

    public static MarkerTogglesView Ensure(RectTransform parent, TMP_FontAsset font){
        var existing = parent.Find("MapMarkerToggles")?.
                              GetComponent<MarkerTogglesView>();

        if(existing) Destroy(existing.gameObject);

        var root = new GameObject(
                                  "MapMarkerToggles",
                                  typeof(RectTransform),
                                  typeof(Image),
                                  typeof(VerticalLayoutGroup)
                                 );
        var rect = root.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin        = new Vector2(1f,   0.5f);
        rect.anchorMax        = new Vector2(1f,   0.5f);
        rect.pivot            = new Vector2(1f,   0.5f);
        rect.anchoredPosition = new Vector2(-10f, 28f);
        rect.sizeDelta        = new Vector2(210f, 296f);

        var bg = root.GetComponent<Image>();
        bg.color         = new Color(0.04f, 0.04f, 0.04f, 0.82f);
        bg.raycastTarget = false;

        var layout = root.GetComponent<VerticalLayoutGroup>();
        layout.spacing                = 5f;
        layout.padding                = new RectOffset(10, 10, 10, 10);
        layout.childAlignment         = TextAnchor.UpperRight;
        layout.childControlWidth      = true;
        layout.childControlHeight     = true;
        layout.childForceExpandWidth  = false;
        layout.childForceExpandHeight = false;

        var view = root.AddComponent<MarkerTogglesView>();
        view.Build(root.transform, font);

        return view;
    }

    public void Bind(RaidPlaybackController playback){
        _playback = playback;
        SyncFromPlayback();
    }

    private void Build(Transform root, TMP_FontAsset font){
        var defs = new[]{
                            LocaleKeys.MapFilterSpawn, LocaleKeys.MapFilterKills, LocaleKeys.MapFilterLoot,
                            LocaleKeys.MapFilterInjury, LocaleKeys.MapFilterHealing, LocaleKeys.MapFilterAchievements,
                            LocaleKeys.MapFilterDoorUnlocks, LocaleKeys.MapFilterExtract, LocaleKeys.MapFilterDeath
                        };

        _markerToggles = new Toggle[defs.Length];

        for(var i = 0; i < defs.Length; i++) _markerToggles[i] = CreateToggle(root, defs[i], font, OnToggleChanged);

        var spacer = new GameObject("Spacer", typeof(RectTransform), typeof(LayoutElement));
        spacer.transform.SetParent(root, false);
        spacer.GetComponent<LayoutElement>().
               minHeight = 4f;

        _rememberToggle = CreateToggle(root, LocaleKeys.MapFilterRemember, font, OnToggleChanged);
    }

    private void OnToggleChanged(bool _){
        if(_syncing || !_playback) return;

        _playback.SetVisibility(ReadVisibility());
    }

    private Visibility ReadVisibility(){
        return new Visibility{
                                 Spawn         = ToggleOn(0),
                                 Kills         = ToggleOn(1),
                                 Loot          = ToggleOn(2),
                                 Injury        = ToggleOn(3),
                                 Healing       = ToggleOn(4),
                                 Achievements  = ToggleOn(5),
                                 DoorUnlocks   = ToggleOn(6),
                                 Extract       = ToggleOn(7),
                                 Death         = ToggleOn(8),
                                 RememberPrefs = _rememberToggle && _rememberToggle.isOn
                             };
    }

    private bool ToggleOn(int index){
        return _markerToggles != null
            && index          >= 0
            && index          < _markerToggles.Length
            && _markerToggles[index]
            && _markerToggles[index].isOn;
    }

    private void SyncFromPlayback(){
        if(_playback?.Visibility == null || _markerToggles == null) return;

        var v = _playback.Visibility;

        _syncing = true;

        SetToggle(0, v.Spawn);
        SetToggle(1, v.Kills);
        SetToggle(2, v.Loot);
        SetToggle(3, v.Injury);
        SetToggle(4, v.Healing);
        SetToggle(5, v.Achievements);
        SetToggle(6, v.DoorUnlocks);
        SetToggle(7, v.Extract);
        SetToggle(8, v.Death);

        if(_rememberToggle) _rememberToggle.isOn = v.RememberPrefs;

        _syncing = false;
    }

    private void SetToggle(int index, bool on){
        if(_markerToggles == null || index < 0 || index >= _markerToggles.Length || !_markerToggles[index]) return;

        _markerToggles[index].isOn = on;
    }

    private static Toggle CreateToggle(
        Transform parent, string localeKey, TMP_FontAsset font, UnityAction<bool> onChanged
    ){
        var row = new GameObject(
                                 "ToggleRow",
                                 typeof(RectTransform),
                                 typeof(HorizontalLayoutGroup),
                                 typeof(LayoutElement)
                                );
        row.transform.SetParent(parent, false);

        var rowLayout = row.GetComponent<HorizontalLayoutGroup>();
        rowLayout.spacing                = 8f;
        rowLayout.childAlignment         = TextAnchor.MiddleRight;
        rowLayout.childControlWidth      = true;
        rowLayout.childControlHeight     = true;
        rowLayout.childForceExpandWidth  = false;
        rowLayout.childForceExpandHeight = false;

        var rowElement = row.GetComponent<LayoutElement>();
        rowElement.minHeight       = ToggleSize;
        rowElement.preferredHeight = ToggleSize;

        var labelGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
        labelGo.transform.SetParent(row.transform, false);

        var labelElement = labelGo.GetComponent<LayoutElement>();
        labelElement.minWidth       = 150f;
        labelElement.preferredWidth = 150f;

        var label = labelGo.GetComponent<TextMeshProUGUI>();
        ScrollRowBuilder.ConfigureText(
                                       label,
                                       LocaleLoader.Format(localeKey),
                                       14f,
                                       Colors.Text.Caption,
                                       font,
                                       TextAlignmentOptions.MidlineRight
                                      );
        label.raycastTarget = false;

        var toggleGo = new GameObject(
                                      "Toggle",
                                      typeof(RectTransform),
                                      typeof(Toggle),
                                      typeof(Image),
                                      typeof(LayoutElement)
                                     );
        toggleGo.transform.SetParent(row.transform, false);

        var toggleRect = toggleGo.GetComponent<RectTransform>();
        toggleRect.sizeDelta = new Vector2(ToggleSize, ToggleSize);

        var toggleElement = toggleGo.GetComponent<LayoutElement>();
        toggleElement.minWidth        = ToggleSize;
        toggleElement.preferredWidth  = ToggleSize;
        toggleElement.minHeight       = ToggleSize;
        toggleElement.preferredHeight = ToggleSize;
        toggleElement.flexibleWidth   = 0f;
        toggleElement.flexibleHeight  = 0f;

        var box = toggleGo.GetComponent<Image>();
        box.color         = new Color(0.12f, 0.12f, 0.12f, 0.95f);
        box.raycastTarget = true;

        var checkGo = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
        checkGo.transform.SetParent(toggleGo.transform, false);

        var checkRect = checkGo.GetComponent<RectTransform>();
        checkRect.anchorMin        = new Vector2(0.5f,            0.5f);
        checkRect.anchorMax        = new Vector2(0.5f,            0.5f);
        checkRect.pivot            = new Vector2(0.5f,            0.5f);
        checkRect.sizeDelta        = new Vector2(ToggleSize - 6f, ToggleSize - 6f);
        checkRect.anchoredPosition = Vector2.zero;

        var checkImage = checkGo.GetComponent<Image>();
        checkImage.color         = Colors.Text.SectionAccent;
        checkImage.raycastTarget = false;

        var toggle = toggleGo.GetComponent<Toggle>();
        toggle.targetGraphic = box;
        toggle.graphic       = checkImage;
        toggle.isOn          = true;
        toggle.onValueChanged.AddListener(onChanged);

        return toggle;
    }
}
