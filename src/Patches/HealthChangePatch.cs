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
internal sealed class HealthChangePatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(ActiveHealthController).GetMethod(
                                                        nameof(ActiveHealthController.ChangeHealth),
                                                        BindingFlags.Instance | BindingFlags.Public
                                                       );
    }

    [PatchPostfix]
    private static void Postfix(ActiveHealthController __instance, EBodyPart bodyPart, float value){
        if(!Settings.Enabled.Value || !CareerLogSession.CollectorsActive) return;

        if(value <= 0f) return;

        var baseline = RaidHealthBaseline.MaxBodyHealth;

        if(baseline <= 0f) return;

        var threshold = baseline * (Settings.HealingLargeRestorePercent.Value / 100f);

        if(value < threshold) return;

        HealingMarkerCollector.RecordLargeRestore(__instance.Player, value);
    }
}
