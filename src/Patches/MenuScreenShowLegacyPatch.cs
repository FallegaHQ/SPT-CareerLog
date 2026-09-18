using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT;
using EFT.UI;
using EFT.UI.Matchmaker;
using HarmonyLib;
using Softwyx.CareerLog.Ui.Records.Menu;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

/// <summary>
///     Primary hook -- same Show overload used by SPT-Menu-Overhaul. Deferred layout runs after other
///     menu Show postfixes (including Menu Overhaul) finish repositioning buttons.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class MenuScreenShowLegacyPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return AccessTools.Method(
                                  typeof(MenuScreen),
                                  nameof(MenuScreen.Show),
                                  [
                                      typeof(Profile),
                                      typeof(MatchmakerPlayersController),
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
