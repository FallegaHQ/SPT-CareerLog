using EFT.HandBook;
using EFT.UI;
using Softwyx.CareerLog.Infrastructure;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records;

/// <summary>
/// Registers the RECORDS <see cref="RecordsScreen"/> clone. Called from <see cref="Menu.MenuBootstrap"/>
/// (<c>CommonUI.Awake</c> and <c>MenuScreen.Awake</c>) -- both paths are idempotent.
/// </summary>
internal static class ScreenBootstrap{
    private static Transform _hostTransform;

    private static RecordsScreen Screen{
        get;
        set;
    }

    public static void EnsureRegistered(){
        TryRegister();
    }

    public static bool TryRegister(CommonUI commonUi = null){
        if(Screen) return true;

        commonUi ??= MonoBehaviourSingleton<CommonUI>.Instantiated ? MonoBehaviourSingleton<CommonUI>.Instance : null;

        if(!commonUi?.HandbookScreen) return Screen;

        RegisterFromHandbook(commonUi.HandbookScreen);

        return Screen;
    }

    private static void RegisterFromHandbook(HandbookScreen handbookTemplate){
        _hostTransform ??= handbookTemplate.transform.parent;
        Screen         =   RecordsScreen.CreateFromHandbookTemplate(handbookTemplate);

        if(!Screen){
            CareerLogPlugin.Log?.LogError(PluginInfo.Format("Failed to create Records screen from Handbook template."));

            return;
        }

        if(_hostTransform) Screen.transform.SetParent(_hostTransform, false);

        CurrentScreenSingletonClass.Instance.RegisterScreen(ScreenTypes.Records, Screen);

        CareerLogPlugin.Log?.LogInfo(PluginInfo.Format("Registered Records screen (CommonUI.HandbookScreen)."));
    }
}
