using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using Softwyx.CareerLog.Collectors.Loot;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class StatisticsGrabLootPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(BaseStatisticsManager).GetMethod(
                                                       nameof(
                                                           BaseStatisticsManager.
                                                               OnGrabLoot)
                                                      );
    }

    [PatchPostfix]
    private static void Postfix(BaseStatisticsManager __instance, Item item){
        if(!Settings.Enabled.Value || !CareerLogSession.CollectorsActive) return;

        if(!CollectorProfileGuard.IsLocalStatisticsProfile(__instance._profile)) return;

        if(item == null) return;

        // LootMarkerCollector handles container recursion and merged marker creation.
        LootMarkerCollector.RecordPickup(item);
    }
}
