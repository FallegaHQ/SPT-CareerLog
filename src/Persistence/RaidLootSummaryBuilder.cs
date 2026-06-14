using System;
using EFT;
using Softwyx.CareerLog.Collectors.Loot;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal static class RaidLootSummaryBuilder{
    public static RaidLootSummary Build(string exitStatus, Player player){
        var (items, value) = LootMarkerCollector.TakeAggregates();
        var (start, end)   = LoadoutValueCollector.TakeValues();

        var summary = new RaidLootSummary{
                                             TopLooted            = LootMarkerCollector.TakeTopLooted(8),
                                             ItemsTaken           = items,
                                             ValueTakenRub        = value,
                                             LoadoutValueStartRub = start,
                                             LoadoutValueEndRub   = end
                                         };

        if(string.Equals(exitStatus, "Killed", StringComparison.Ordinal)){
            summary.ItemsLost    = 1;
            summary.ValueLostRub = start;

            return summary;
        }

        var (bringInItemsLost, bringInValueLost) = StartingLootBaseline.TakeLoss(player);

        summary.BringInItemsLost    = bringInItemsLost;
        summary.BringInValueLostRub = bringInValueLost;

        return summary;
    }
}
