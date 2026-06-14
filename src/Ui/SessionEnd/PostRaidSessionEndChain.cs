using System;
using EFT.UI.Screens;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Session;
using Softwyx.CareerLog.Ui.SessionEnd.Debrief;
using Softwyx.CareerLog.Ui.SessionEnd.Map;

namespace Softwyx.CareerLog.Ui.SessionEnd;

internal static class PostRaidSessionEndChain{
    public static bool ModChainCompleted{
        get;
        private set;
    }

    public static bool CanRunModChain(){
        return Settings.Enabled.Value && SessionEndUiBootstrap.RaidMapScreen && SessionEndUiBootstrap.DebriefScreen;
    }

    public static void MarkModChainCompleted(){
        ModChainCompleted = true;
    }

    public static void ShowMapThenDebrief(PostRaidHealthScreenClass instance, Action continueVanillaFlow){
        ModChainCompleted = false;

        if(instance == null){
            continueVanillaFlow?.Invoke();

            return;
        }

        var profile  = PostRaidHealthScreenAccess.GetProfile(instance);
        var location = PostRaidHealthScreenAccess.GetLocation(instance);

        if(profile == null){
            continueVanillaFlow?.Invoke();

            return;
        }

        var raidRecord = CareerLogSession.LastSavedRaid;

        if(raidRecord == null){
            continueVanillaFlow?.Invoke();

            return;
        }

        var mapController = new MapScreenController(location, raidRecord);
        mapController.OnShowNextScreen += () => {
                                              var debriefController = new DebriefScreenController(location, raidRecord);
                                              debriefController.OnShowNextScreen += () => continueVanillaFlow?.Invoke();
                                              debriefController.ShowScreen(EScreenState.Queued);
                                          };
        mapController.ShowScreen(EScreenState.Queued);
    }
}
