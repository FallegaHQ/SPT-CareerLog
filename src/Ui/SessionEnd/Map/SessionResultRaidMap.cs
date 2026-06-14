using Comfort.Common;
using EFT.InputSystem;
using EFT.UI;
using EFT.UI.Screens;
using EFT.UI.SessionEnd;
using Softwyx.CareerLog.Infrastructure;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Map;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Formatting;
using TMPro;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.SessionEnd.Map;

internal sealed class SessionResultRaidMap : EftScreen<MapScreenController, SessionResultRaidMap>{
    private const string          MapViewportName = "RaidMapViewport";
    private       DefaultUIButton _nextButton;
    private       DefaultUIButton _backButton;
    private       TextMeshProUGUI _locationName;
    private       Transform       _scrollContent;
    private       RectTransform   _mapHost;
    private       RectTransform   _mapViewport;
    private       PanelView       _mapPanel;

    private void Awake(){
        _nextButton.OnClick.AddListener(OnNextClicked);
        _backButton.OnClick.AddListener(OnBackClicked);
    }

    internal static SessionResultRaidMap CreateFromTemplate(SessionResultStatistics template){
        if(!template) return null;

        var clone = Instantiate(template.gameObject, template.transform.parent);
        clone.name = "SessionResultRaidMap";
        clone.SetActive(false);

        var statistics = clone.GetComponent<SessionResultStatistics>();
        var mapScreen  = clone.AddComponent<SessionResultRaidMap>();

        mapScreen.BindFrom(statistics);
        Destroy(statistics);

        return mapScreen;
    }

    private void BindFrom(SessionResultStatistics template){
        _nextButton    = SessionEndScreenReflection.GetNextButton(template);
        _backButton    = SessionEndScreenReflection.GetBackButton(template);
        _locationName  = SessionEndScreenReflection.GetLocationName(template);
        _scrollContent = SessionEndScrollPaths.FindScrollContent(template.transform);
        _mapHost       = transform as RectTransform;

        SessionEndScreenReflection.GetStatsSpawn(template).
                                   enabled = false;
        SessionEndScrollPaths.HideBlockingPreview(template.transform);

        if(_scrollContent) SessionEndScrollPaths.SetScrollChainActive(_scrollContent, false);
    }

    public override void Show(MapScreenController controller){
        Show(controller.Location, controller.RaidRecord);
    }

    private void Show(LocationSettingsClass.Location location, RaidRecord raidRecord){
        ShowGameObject();
        _nextButton.Interactable = true;
        _backButton.Interactable = true;

        if(_locationName){
            var locationLabel = LocationNameResolver.Resolve(raidRecord?.LocationId, location);
            _locationName.text = LocaleLoader.Format(LocaleKeys.MapTitle, locationLabel);
        }

        if(!_mapHost) return;

        _mapViewport ??= EnsureViewport(_mapHost);
        _mapPanel    =   PanelView.Ensure(_mapViewport);
        _mapPanel?.SetLabelFont(_locationName ? _locationName.font : null);
        _mapPanel?.Show(raidRecord);
    }

    private static RectTransform EnsureViewport(RectTransform host){
        var existing = host.Find(MapViewportName) as RectTransform;

        if(existing) return existing;

        var go = new GameObject(MapViewportName, typeof(RectTransform)){
                                                                           layer = host.gameObject.layer
                                                                       };
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(host, false);
        rect.anchorMin        = new Vector2(0.05f, 0.18f);
        rect.anchorMax        = new Vector2(0.95f, 0.88f);
        rect.offsetMin        = Vector2.zero;
        rect.offsetMax        = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        return rect;
    }

    public override ETranslateResult TranslateCommand(ECommand command){
        if(!command.IsCommand(ECommand.Escape)) return GetDefaultBlockResult(command);

        Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuEscape);
        ScreenController.CloseScreen();

        return ETranslateResult.BlockAll;
    }

    public override void Close(){
        _mapPanel?.Hide();
        base.Close();
    }

    private void OnBackClicked(){
        _mapPanel?.SaveMarkerFilterPrefs();
        ScreenController.CloseScreen();
    }

    private void OnNextClicked(){
        _mapPanel?.SaveMarkerFilterPrefs();
        _nextButton.Interactable = false;
        _backButton.Interactable = false;
        ScreenController.ShowNextScreen();
    }
}
