using EFT;
using EFT.UI;
using EFT.UI.Screens;

namespace Softwyx.CareerLog.Ui.Records.Menu;

internal static class MenuContext{
    private static Profile CurrentProfile{
        get;
        set;
    }

    public static void Set(Profile profile){
        CurrentProfile = profile;
    }

    public static void Open(){
        var profile = CurrentProfile ?? ProfileResolver.FromMenuScreen();

        if(profile == null){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format("Cannot open Records -- profile unavailable."));

            return;
        }

        CurrentProfile = profile;

        var commonUi = MonoBehaviourSingleton<CommonUI>.Instantiated ? MonoBehaviourSingleton<CommonUI>.Instance : null;

        if(!ScreenBootstrap.TryRegister(commonUi)){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format("Records screen is not registered."));

            return;
        }

        var controller = new RecordsScreenController(profile);
        controller.ShowScreen(EScreenState.Queued);

        CareerLogPlugin.Log?.LogInfo(PluginInfo.Format("Opening Records screen."));
    }
}
