using System.Diagnostics.CodeAnalysis;
using EFT.UI;
using Softwyx.CareerLog.Ui.Records.Menu;
using SPT.Reflection.Patching;
using System.Reflection;
using HarmonyLib;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class MenuScreenShowPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return AccessTools.Method(typeof(MenuScreen), nameof(MenuScreen.Show), [typeof(MenuScreenController)]);
    }

    [PatchPostfix]
    private static void Postfix(MenuScreen __instance, MenuScreenController controller){
        MenuBootstrap.OnMenuShown(__instance, controller?.Profile);
    }
}
