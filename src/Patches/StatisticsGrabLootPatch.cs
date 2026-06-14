using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT.InventoryLogic;
using Softwyx.CareerLog.Collectors.Loot;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class StatisticsGrabLootPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(LocationStatisticsCollectorAbstractClass).GetMethod(
                                                                          nameof(
                                                                              LocationStatisticsCollectorAbstractClass.
                                                                                  OnGrabLoot)
                                                                         );
    }

    [PatchPostfix]
    private static void Postfix(LocationStatisticsCollectorAbstractClass __instance, Item item){
        if(!Settings.Enabled.Value || !CareerLogSession.CollectorsActive) return;

        if(!CollectorProfileGuard.IsLocalStatisticsProfile(__instance.Profile_0)) return;

        if(item == null) return;

        // LootMarkerCollector handles container recursion and merged marker creation.
        LootMarkerCollector.RecordPickup(item);
    }
}
