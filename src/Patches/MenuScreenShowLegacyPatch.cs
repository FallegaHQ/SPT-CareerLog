using System.Diagnostics.CodeAnalysis;
using EFT;
using EFT.UI;
using Softwyx.CareerLog.Ui.Records.Menu;
using SPT.Reflection.Patching;
using System.Reflection;
using HarmonyLib;

namespace Softwyx.CareerLog.Patches;

/// <summary>
/// Primary hook -- same Show overload used by SPT-Menu-Overhaul. Deferred layout runs after other
/// menu Show postfixes (including Menu Overhaul) finish repositioning buttons.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class MenuScreenShowLegacyPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return AccessTools.Method(
                                  typeof(MenuScreen),
                                  nameof(MenuScreen.Show),
                                  [
                                      typeof(Profile),
                                      typeof(MatchmakerPlayerControllerClass),
                                      typeof(ESessionMode)
                                  ]
                                 );
    }

    [PatchPostfix]
    [HarmonyPriority(Priority.First)]
    private static void Postfix(MenuScreen __instance, Profile profile){
        MenuBootstrap.OnMenuShown(__instance, profile);
    }
}
