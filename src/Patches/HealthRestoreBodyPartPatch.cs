using System.Diagnostics.CodeAnalysis;
using EFT.HealthSystem;
using Softwyx.CareerLog.Collectors.Health;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;
using System.Reflection;
using BindingFlags = System.Reflection.BindingFlags;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class HealthRestoreBodyPartPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(ActiveHealthController).GetMethod(
                                                        nameof(ActiveHealthController.RestoreBodyPart),
                                                        BindingFlags.Instance | BindingFlags.Public
                                                       );
    }

    [PatchPostfix]
    private static void Postfix(ActiveHealthController __instance, bool __result){
        if(!Settings.Enabled.Value || !CareerLogSession.CollectorsActive || !__result) return;

        HealingMarkerCollector.RecordSurgery(__instance.Player);
    }
}
