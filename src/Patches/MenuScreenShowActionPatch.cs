using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT.UI;
using HarmonyLib;
using Softwyx.CareerLog.Ui.Records.Menu;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class MenuScreenShowActionPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return AccessTools.Method(typeof(MenuScreenController), nameof(MenuScreenController.ShowAction));
    }

    [PatchPostfix]
    private static void Postfix(MenuScreen screen){
        MenuBootstrap.OnMenuShowAction(screen);
    }
}
