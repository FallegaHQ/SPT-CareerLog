using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT;
using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Ui.SessionEnd;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

/// <summary>
///     After exit status: raid map, then debrief, then vanilla kill list.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class PostRaidBeforeKillListPatch : ModulePatch{
    private static bool _showVanillaKillList;

    protected override MethodBase GetTargetMethod(){
        return PostRaidHealthScreenAccess.ContinueToKillListMethod;
    }

    [PatchPrefix]
    private static bool Prefix(SessionResultShowOperation __instance){
        if(_showVanillaKillList || !PostRaidSessionEndChain.CanRunModChain()) return true;

        PostRaidSessionEndChain.ShowMapThenDebrief(__instance, () => ContinueToKillList(__instance));

        return false;
    }

    private static void ContinueToKillList(SessionResultShowOperation instance){
        PostRaidSessionEndChain.MarkModChainCompleted();

        _showVanillaKillList = true;

        try{
            PostRaidHealthScreenAccess.ShowKillList(instance);
        }
        finally{
            _showVanillaKillList = false;
        }
    }
}
