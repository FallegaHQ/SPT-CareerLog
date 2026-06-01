using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Ui.SessionEnd;
using SPT.Reflection.Patching;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Softwyx.CareerLog.Interop;

namespace Softwyx.CareerLog.Patches;

/// <summary>
/// After kill list: optional vanilla statistics screen, or skip straight to XP when disabled.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class PostRaidBeforeStatisticsPatch : ModulePatch{
    private static bool _continueVanillaStatistics;

    protected override MethodBase GetTargetMethod(){
        return PostRaidHealthScreenAccess.ContinueToStatisticsMethod;
    }

    [PatchPrefix]
    private static bool Prefix(PostRaidHealthScreenClass __instance){
        if(_continueVanillaStatistics || !PostRaidSessionEndChain.ModChainCompleted) return true;

        if(Settings.RestoreVanillaStatistics.Value) return true;

        _continueVanillaStatistics = true;

        try{
            PostRaidHealthScreenAccess.ContinueFromStatistics(__instance);
        }
        finally{
            _continueVanillaStatistics = false;
        }

        return false;
    }
}
