using System.Diagnostics.CodeAnalysis;
using EFT.UI;
using Softwyx.CareerLog.Ui.Records.Menu;
using SPT.Reflection.Patching;
using System.Reflection;
using HarmonyLib;

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
