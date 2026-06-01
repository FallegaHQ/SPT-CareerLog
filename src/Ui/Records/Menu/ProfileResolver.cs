using EFT;
using EFT.UI;
using Softwyx.CareerLog.Infrastructure;

namespace Softwyx.CareerLog.Ui.Records.Menu;

internal static class ProfileResolver{
    public static Profile FromMenuScreen(MenuScreen menuScreen = null){
        menuScreen ??= ResolveMenuScreen();

        if(!menuScreen) return null;

        return MenuScreenReflection.GetController(menuScreen)?.
                                    Profile;
    }

    private static MenuScreen ResolveMenuScreen(){
        return !MonoBehaviourSingleton<CommonUI>.Instantiated
                   ? null
                   : MonoBehaviourSingleton<CommonUI>.Instance.MenuScreen;
    }
}
