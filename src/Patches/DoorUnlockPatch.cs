using EFT;
using EFT.Interactive;
using Softwyx.CareerLog.Collectors.Doors;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class DoorUnlockPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(WorldInteractiveObject).GetMethod(
                                                        nameof(WorldInteractiveObject.Unlock),
                                                        BindingFlags.Instance | BindingFlags.Public
                                                       );
    }

    [PatchPostfix]
    private static void Postfix(WorldInteractiveObject __instance){
        if(!Settings.Enabled.Value || !CareerLogSession.CollectorsActive) return;

        if(__instance?.InteractingPlayer is not Player{
                                                    IsYourPlayer: true
                                                }) return;

        DoorUnlockMarkerCollector.Record(__instance);
    }
}
