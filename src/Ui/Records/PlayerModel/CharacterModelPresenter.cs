using System;
using System.Collections;
using EFT;
using EFT.UI;
using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Softwyx.CareerLog.Ui.Records.PlayerModel;

/// <summary>
///     Clones the inventory <see cref="InventoryPlayerModelWithStatsWindow" /> host (character tab) into
///     <c>PlayerModelSlot</c> so drag rotation, weapons and bottom-field stats match vanilla behavior.
/// </summary>
internal static class CharacterModelPresenter{
    private static InventoryPlayerModelWithStatsWindow _statsWindow;
    private static PlayerModelView                     _modelView;
    private static Transform                           _cloneRoot;
    private static Transform                           _modelSlot;
    private static int                                 _hostInstanceId;
    private static string                              _shownProfileId;

    public static void EnsureInstalled(Transform recordsRoot){
        if(!recordsRoot) return;

        var hostId = recordsRoot.GetInstanceID();

        if(_statsWindow && _hostInstanceId == hostId) return;

        Release();

        _modelSlot = FindPlayerModelSlot(recordsRoot);

        if(!_modelSlot){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format("Records player model -- PlayerModelSlot not found."));

            return;
        }

        var templateStats = FindTemplateStatsWindow();

        if(!templateStats){
            CareerLogPlugin.Log?.LogWarning(
                                            PluginInfo.Format(
                                                              "Records player model -- InventoryPlayerModelWithStatsWindow not found."
                                                             )
                                           );

            return;
        }

        _cloneRoot = Object.Instantiate(templateStats.gameObject, _modelSlot, false).
                            transform;
        _cloneRoot.name = UiHierarchy.RecordsScreen.CharacterModelClone;

        HideNonModelSiblings(_cloneRoot, templateStats.transform);

        StretchToParent(_cloneRoot as RectTransform);

        _statsWindow = _cloneRoot.GetComponent<InventoryPlayerModelWithStatsWindow>()
                    ?? _cloneRoot.GetComponentInChildren<InventoryPlayerModelWithStatsWindow>(true);

        _modelView = _statsWindow?._playerModelView;

        _modelView      ??= _cloneRoot.GetComponentInChildren<PlayerModelView>(true);
        _hostInstanceId =   hostId;

        if(!_modelView || !_statsWindow){
            Release();

            CareerLogPlugin.Log?.LogWarning(
                                            PluginInfo.Format(
                                                              "Records player model -- stats window or model view missing on clone."
                                                             )
                                           );

            return;
        }

        ConfigureModelVisuals(_modelView.gameObject);

        _modelSlot.gameObject.SetActive(true);
        _cloneRoot.gameObject.SetActive(true);

        CareerLogPlugin.Log?.LogInfo(PluginInfo.Format("Records player model installed (stats window host)."));
    }

    public static void RequestShow(RecordsScreen host, Profile profile){
        if(!host || profile == null) return;

        host.StartCoroutine(ShowRoutine(profile));
    }

    public static void Hide(){
        if(_statsWindow)
            _statsWindow.Close();
        else
            _modelView?.Close();

        if(_cloneRoot) _cloneRoot.gameObject.SetActive(false);

        _shownProfileId = null;
    }

    public static void Release(){
        Hide();

        if(_cloneRoot){
            Object.Destroy(_cloneRoot.gameObject);
            _cloneRoot   = null;
            _statsWindow = null;
            _modelView   = null;
        }

        _modelSlot      = null;
        _hostInstanceId = 0;
        _shownProfileId = null;
    }

    private static IEnumerator ShowRoutine(Profile profile){
        if(!_statsWindow || !_modelView || profile == null) yield break;

        if(string.Equals(_shownProfileId, profile.ProfileId, StringComparison.Ordinal)
        && _cloneRoot.gameObject.activeSelf
        && _modelView.LoadingComplete)
            yield break;

        var controller = InventoryScreenAccess.ResolvePreviewController(profile);

        if(controller == null){
            CareerLogPlugin.Log?.LogWarning(
                                            PluginInfo.Format(
                                                              "Records player model -- could not resolve InventoryController."
                                                             )
                                           );

            yield break;
        }

        SetModelHierarchyActive(true);
        _statsWindow.Close();

        try{
            _statsWindow.Show(profile, controller, true);
        }
        catch(Exception ex){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Records player model Show failed: {ex}"));

            yield break;
        }

        yield return FinishAfterShow(profile);
    }

    private static IEnumerator FinishAfterShow(Profile profile){
        ApplyAfterShow(profile);

        var frames = 0;

        while(_modelView && frames < 180){
            if(_modelView.LoadingComplete) break;

            frames++;

            yield return null;
        }

        RefreshAfterLoad(profile);

        yield return null;

        RefreshAfterLoad(profile);

        _shownProfileId = profile.ProfileId;
    }

    private static void ApplyAfterShow(Profile profile){
        SetModelHierarchyActive(true);
        AdjustInnerPlayerModelPosition(_modelView.gameObject);
        RefreshAfterLoad(profile);
    }

    private static void RefreshAfterLoad(Profile profile){
        if(!_statsWindow || !_modelView || profile == null) return;

        CharacterModelDragBinder.WireStatsWindow(_statsWindow, _modelView);
        CharacterModelBottomField.Bind(_modelView.transform, profile);
        ApplyHostPanelToggles(_cloneRoot);
        InlineLevelIntoBottomField(_cloneRoot);
    }

    private static void ApplyHostPanelToggles(Transform cloneRoot){
        if(!cloneRoot) return;

        RectLayout.SetChildActive(cloneRoot, UiHierarchy.InventoryCharacterTab.ClothingPanel, false);

        RectLayout.SetChildActive(cloneRoot, UiHierarchy.InventoryCharacterTab.CharacterPanel, true);

        var characterPanel = cloneRoot.Find(UiHierarchy.InventoryCharacterTab.CharacterPanel);

        if(characterPanel)
            RectLayout.SetChildActive(characterPanel, UiHierarchy.InventoryCharacterTab.LevelPanel, false);
    }

    private static void InlineLevelIntoBottomField(Transform cloneRoot){
        if(!cloneRoot) return;

        const string levelPanelPath = $"{UiHierarchy.InventoryCharacterTab.CharacterPanel}/"
                                    + $"{UiHierarchy.InventoryCharacterTab.LevelPanel}";

        var levelText = cloneRoot.Find($"{levelPanelPath}/{UiHierarchy.InventoryCharacterTab.LevelText}");
        var levelIcon = cloneRoot.Find($"{levelPanelPath}/{UiHierarchy.InventoryCharacterTab.LevelIcon}");

        if(!levelText && !levelIcon) return;

        var bottomField = cloneRoot.Find(
                                         $"{UiHierarchy.InventoryCharacterTab.CharacterPanel}/"
                                       + $"{UiHierarchy.InventoryCharacterTab.PlayerModelView}/"
                                       + $"{UiHierarchy.InventoryCharacterTab.BottomField}"
                                        );

        if(!bottomField)
            bottomField = cloneRoot.Find(
                                         $"{UiHierarchy.InventoryCharacterTab.PlayerModelView}/"
                                       + $"{UiHierarchy.InventoryCharacterTab.BottomField}"
                                        );

        if(!bottomField) return;

        var experience = bottomField.Find(UiHierarchy.InventoryCharacterTab.Experience)
                      ?? bottomField.Find(UiHierarchy.InventoryCharacterTab.ExperienceRow);

        if(!experience) return;

        var row = bottomField.Find(UiHierarchy.InventoryCharacterTab.InlineLevelRow);

        if(!row){
            var rowObject = new GameObject(
                                           UiHierarchy.InventoryCharacterTab.InlineLevelRow,
                                           typeof(RectTransform),
                                           typeof(HorizontalLayoutGroup),
                                           typeof(LayoutElement)
                                          );
            row = rowObject.transform;
            row.SetParent(bottomField, false);
        }

        row.gameObject.SetActive(true);

        row.SetSiblingIndex(experience.GetSiblingIndex() + 1);

        var layout = row.GetComponent<HorizontalLayoutGroup>();
        layout.spacing                = Spacing.FilterChipSpacing;
        layout.padding                = new RectOffset(0, 0, 0, 0);
        layout.childAlignment         = TextAnchor.MiddleRight;
        layout.childControlWidth      = true;
        layout.childControlHeight     = true;
        layout.childForceExpandWidth  = false;
        layout.childForceExpandHeight = false;

        var le = row.GetComponent<LayoutElement>();
        le.flexibleWidth  = 0f;
        le.flexibleHeight = 0f;

        if(levelIcon){
            levelIcon.SetParent(row, false);
            levelIcon.gameObject.SetActive(true);
        }

        if(!levelText) return;

        levelText.SetParent(row, false);
        levelText.gameObject.SetActive(true);
    }

    private static InventoryPlayerModelWithStatsWindow FindTemplateStatsWindow(){
        var inventoryScreen = InventoryScreenAccess.GetInventoryScreen();

        if(!inventoryScreen) return null;

        const string path = $"{UiHierarchy.InventoryCharacterTab.OverallPanel}/"
                          + $"{UiHierarchy.InventoryCharacterTab.LeftSide}/"
                          + $"{UiHierarchy.InventoryCharacterTab.CharacterPanel}/"
                          + $"{UiHierarchy.InventoryCharacterTab.PlayerModelView}";

        var playerModel = inventoryScreen.transform.
                                          Find(path)?.
                                          GetComponent<PlayerModelView>();

        if(!playerModel) return inventoryScreen.GetComponentInChildren<InventoryPlayerModelWithStatsWindow>(true);

        var onParent = playerModel.GetComponent<InventoryPlayerModelWithStatsWindow>();

        return onParent ? onParent : playerModel.GetComponentInParent<InventoryPlayerModelWithStatsWindow>();
    }

    private static void HideNonModelSiblings(Transform cloneRoot, Transform templateRoot){
        if(!cloneRoot || !templateRoot) return;

        var templateModel = templateRoot.GetComponent<PlayerModelView>()
                         ?? templateRoot.GetComponentInChildren<PlayerModelView>(true);

        if(!templateModel) return;

        var modelTransform = cloneRoot.GetComponent<PlayerModelView>()
                                 ? cloneRoot
                                 : cloneRoot.GetComponentInChildren<PlayerModelView>(true)?.
                                             transform;

        if(!modelTransform) return;

        foreach(Transform child in cloneRoot){
            if(child == modelTransform) continue;

            if(child.IsChildOf(modelTransform)) continue;

            child.gameObject.SetActive(false);
        }
    }

    private static Transform FindPlayerModelSlot(Transform recordsRoot){
        return recordsRoot.Find(
                                $"{UiHierarchy.RecordsScreen.Panels}/"
                              + $"{UiHierarchy.RecordsScreen.PanelsBackground}/"
                              + $"{UiHierarchy.RecordsScreen.LeftPanel}/"
                              + $"{UiHierarchy.RecordsScreen.PlayerModelSlot}"
                               )
            ?? recordsRoot.Find(
                                $"{UiHierarchy.RecordsScreen.LeftPanel}/"
                              + $"{UiHierarchy.RecordsScreen.PlayerModelSlot}"
                               )
            ?? recordsRoot.Find(UiHierarchy.RecordsScreen.PlayerModelSlot);
    }

    private static void SetModelHierarchyActive(bool active){
        if(_modelSlot) _modelSlot.gameObject.SetActive(active);

        if(_cloneRoot) _cloneRoot.gameObject.SetActive(active);
    }

    private static void StretchToParent(RectTransform rect){
        if(!rect) return;

        RectLayout.StretchToParent(rect);
        rect.localScale = Vector3.one;
    }

    private static void ConfigureModelVisuals(GameObject modelRoot){
        RectLayout.SetChildActive(modelRoot.transform, UiHierarchy.InventoryCharacterTab.BottomField,    true);
        RectLayout.SetChildActive(modelRoot.transform, UiHierarchy.InventoryCharacterTab.IconsContainer, true);
        RectLayout.SetChildActive(modelRoot.transform, UiHierarchy.InventoryCharacterTab.DragTrigger,    true);

        var rawImage = modelRoot.GetComponent<RawImage>();

        if(rawImage){
            rawImage.raycastTarget = false;
            rawImage.color         = Colors.Control.GraphicWhite;
        }

        var playerMv = modelRoot.transform.Find(UiHierarchy.InventoryCharacterTab.PlayerMvObject);

        if(!playerMv) return;

        var camera = playerMv.Find(UiHierarchy.InventoryCharacterTab.CameraInventory);

        if(camera) camera.localPosition = new Vector3(0f, 0f, 1f);

        var lightsRoot = playerMv.Find(UiHierarchy.InventoryCharacterTab.PlayerMvObjectLights);

        if(!lightsRoot) return;

        foreach(var light in lightsRoot.GetComponentsInChildren<Light>(true)) light.enabled = true;
    }

    private static void AdjustInnerPlayerModelPosition(GameObject clone){
        var playerMv = clone.transform.Find(UiHierarchy.InventoryCharacterTab.PlayerMvObject);

        if(!playerMv) return;

        var menuPlayer = playerMv.Find(UiHierarchy.InventoryCharacterTab.MenuPlayer);

        if(menuPlayer) menuPlayer.localPosition = new Vector3(0f, -1.1f, 5f);
    }
}
