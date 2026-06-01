using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT.UI.SessionEnd;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Ui.SessionEnd;
using SPT.Reflection.Patching;
using HarmonyLib;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class SessionEndUiAwakePatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return AccessTools.Method(typeof(SessionEndUI), nameof(SessionEndUI.Awake));
    }

    [PatchPostfix]
    private static void Postfix(SessionEndUI __instance){
        if(!Settings.Enabled.Value) return;

        SessionEndUiBootstrap.Register(__instance);
    }
}
