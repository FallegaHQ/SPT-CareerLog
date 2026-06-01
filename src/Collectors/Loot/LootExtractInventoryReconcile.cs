using EFT;
using Softwyx.CareerLog.Persistence.Models;
using System;
using System.Collections.Generic;

namespace Softwyx.CareerLog.Collectors.Loot;

/// <summary>
/// At extract, reconcile loot markers and stats against items actually in the player's inventory
/// (to cover code paths that don't fire inventory remove events).
/// </summary>
internal static class LootExtractInventoryReconcile{
    public static void Apply(Player player){
        if(player?.InventoryController?.Inventory == null) return;

        var inventoryIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        StartingLootBaseline.CollectEquipmentItemIds(player, inventoryIds);

        LootMarkerCollector.SyncOwnedFromInventory(inventoryIds);
    }

    public static void PruneLootMarkers(List<RaidMovementValue> markers){
        LootMarkerCollector.PruneLootMarkers(markers);
    }
}
