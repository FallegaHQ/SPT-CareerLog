using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT.HealthSystem;
using Softwyx.CareerLog.Collectors.Health;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;
using BindingFlags = System.Reflection.BindingFlags;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class HealthBodyPartDestroyedPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(ActiveHealthController).GetMethod(
                                                        nameof(ActiveHealthController.DestroyBodyPart),
                                                        BindingFlags.Instance | BindingFlags.Public
                                                       );
    }

    [PatchPostfix]
    private static void Postfix(ActiveHealthController __instance, EBodyPart bodyPart){
        if(!Settings.Enabled.Value || !CareerLogSession.CollectorsActive) return;

        InjuryMarkerCollector.Record(__instance, bodyPart);
    }
}
