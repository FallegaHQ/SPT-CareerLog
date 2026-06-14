using Comfort.Common;
using EFT.InputSystem;
using EFT.UI;
using EFT.UI.Screens;
using EFT.UI.SessionEnd;
using Softwyx.CareerLog.Infrastructure;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Formatting;
using TMPro;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.SessionEnd.Debrief;

internal sealed class SessionResultCareerLog : EftScreen<DebriefScreenController, SessionResultCareerLog>{
    private DefaultUIButton _nextButton;
    private DefaultUIButton _backButton;
    private StatisticsSpawn _statsSpawn;
    private TextMeshProUGUI _locationName;
    private Transform       _scrollContent;

    private void Awake(){
        _nextButton.OnClick.AddListener(OnNextClicked);
        _backButton.OnClick.AddListener(OnBackClicked);
    }

    internal static SessionResultCareerLog CreateFromTemplate(SessionResultStatistics template){
        if(!template) return null;

        var clone = Instantiate(template.gameObject, template.transform.parent);
        clone.name = "SessionResultCareerLog";
        clone.SetActive(false);

        var statistics = clone.GetComponent<SessionResultStatistics>();
        var debrief    = clone.AddComponent<SessionResultCareerLog>();

        debrief.BindFrom(statistics);
        Destroy(statistics);

        return debrief;
    }

    private void BindFrom(SessionResultStatistics template){
        _nextButton    = SessionEndScreenReflection.GetNextButton(template);
        _backButton    = SessionEndScreenReflection.GetBackButton(template);
        _statsSpawn    = SessionEndScreenReflection.GetStatsSpawn(template);
        _locationName  = SessionEndScreenReflection.GetLocationName(template);
        _scrollContent = SessionEndScrollPaths.FindScrollContent(template.transform);
        SessionEndScrollPaths.HideBlockingPreview(template.transform);

        if(_statsSpawn) _statsSpawn.enabled = false;

        if(_scrollContent){
            DebriefScrollContentBuilder.Clear(_scrollContent);
            SessionEndScrollPaths.SetScrollChainActive(_scrollContent, true);
        }
        else{
            CareerLogPlugin.Log?.LogWarning(
                                            PluginInfo.Format(
                                                              "Raid debrief: vanilla Scroll View/Content not found on statistics clone."
                                                             )
                                           );
        }
    }

    public override void Show(DebriefScreenController controller){
        Show(controller.Location, controller.RaidRecord);
    }

    private void Show(LocationSettingsClass.Location location, RaidRecord raidRecord){
        ShowGameObject();
        _nextButton.Interactable = true;
        _backButton.Interactable = true;

        if(_locationName){
            var locationLabel = LocationNameResolver.Resolve(raidRecord?.LocationId, location);
            _locationName.text = LocaleLoader.Format(LocaleKeys.DebriefTitle, locationLabel);
        }

        if(!_scrollContent) return;

        SessionEndScrollPaths.SetScrollChainActive(_scrollContent, true);
        SessionEndScrollPaths.BringScrollToFront(_scrollContent);
        DebriefScrollContentBuilder.Rebuild(_scrollContent, raidRecord, _locationName);
    }

    public override ETranslateResult TranslateCommand(ECommand command){
        if(!command.IsCommand(ECommand.Escape)) return GetDefaultBlockResult(command);

        Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuEscape);
        ScreenController.CloseScreen();

        return ETranslateResult.BlockAll;
    }

    public override void Close(){
        DebriefScrollContentBuilder.Clear(_scrollContent);
        base.Close();
    }

    private void OnBackClicked(){
        ScreenController.CloseScreen();
    }

    private void OnNextClicked(){
        _nextButton.Interactable = false;
        _backButton.Interactable = false;
        ScreenController.ShowNextScreen();
    }
}
