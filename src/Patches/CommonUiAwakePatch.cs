using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT.UI;
using HarmonyLib;
using Softwyx.CareerLog.Ui.Records.Menu;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class CommonUiAwakePatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return AccessTools.Method(typeof(CommonUI), nameof(CommonUI.Awake));
    }

    [PatchPostfix]
    private static void Postfix(CommonUI __instance){
        MenuBootstrap.OnCommonUiReady(__instance);
    }
}
