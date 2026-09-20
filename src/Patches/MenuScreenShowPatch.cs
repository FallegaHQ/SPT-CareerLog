using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT.UI;
using HarmonyLib;
using Softwyx.CareerLog.Ui.Records.Menu;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class MenuScreenShowPatch : ModulePatch{
    /// <summary>Method: <see cref="EFT.UI.MenuScreen.Show(MenuScreen.MainMenuBaseScreenController)" /></summary>
    protected override MethodBase GetTargetMethod(){
        return AccessTools.Method(typeof(MenuScreen), nameof(MenuScreen.Show), [typeof(MenuScreenController)]);
    }

    [PatchPostfix]
    private static void Postfix(MenuScreen __instance, MenuScreenController controller){
        MenuBootstrap.OnMenuShown(__instance, controller?.Profile);
    }
}
