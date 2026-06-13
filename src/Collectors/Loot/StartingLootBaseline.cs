using EFT;
using EFT.InventoryLogic;
using System;
using System.Collections.Generic;

namespace Softwyx.CareerLog.Collectors.Loot;

/// <summary>
/// Bring-in gear snapshot at raid start. Used to reject bring-in items from the loot ledger and to
/// measure bring-in value lost (consumed, used, dropped, etc.) at extract.
/// </summary>
internal static class StartingLootBaseline{
    private static readonly HashSet<string>          ItemIds              = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, long> ValueAtStartByItemId = new(StringComparer.OrdinalIgnoreCase);

    public static void Capture(Player player){
        ItemIds.Clear();
        ValueAtStartByItemId.Clear();

        var inventory = player?.InventoryController?.Inventory;

        if(inventory == null) return;

        foreach(var item in inventory.GetPlayerItems(EPlayerItems.Equipment)){
            var id = item?.Id;

            if(string.IsNullOrEmpty(id)) continue;

            ItemIds.Add(id);
            ValueAtStartByItemId[id] = LootHandbookPricing.EstimateLootUnit(item);
        }
    }

    public static void CollectEquipmentItemIds(Player player, ISet<string> into){
        if(into == null) return;

        var inventory = player?.InventoryController?.Inventory;

        if(inventory == null) return;

        foreach(var item in inventory.GetPlayerItems(EPlayerItems.Equipment)){
            var id = item?.Id;

            if(!string.IsNullOrEmpty(id)) into.Add(id);
        }
    }

    public static bool Contains(string itemId){
        return !string.IsNullOrEmpty(itemId) && ItemIds.Contains(itemId);
    }

    /// <summary>
    /// Bring-in gear no longer on the player, plus partial stack devaluation (meds, food, ammo, etc.).
    /// </summary>
    public static (int itemsFullyLost, long valueLostRub) TakeLoss(Player player){
        if(ValueAtStartByItemId.Count == 0) return (0, 0L);

        var currentValueById = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        var inventory = player?.InventoryController?.Inventory;

        if(inventory != null)
            foreach(var item in inventory.GetPlayerItems(EPlayerItems.Equipment)){
                var id = item?.Id;

                if(!string.IsNullOrEmpty(id)) currentValueById[id] = LootHandbookPricing.EstimateLootUnit(item);
            }

        var valueLost      = 0L;
        var itemsFullyLost = 0;

        foreach(var (itemKey, itemStartValue) in ValueAtStartByItemId){
            if(itemStartValue <= 0L) continue;

            if(!currentValueById.TryGetValue(itemKey, out var currentValue)){
                valueLost      += itemStartValue;
                itemsFullyLost += 1;

                continue;
            }

            if(currentValue < itemStartValue) valueLost += itemStartValue - currentValue;
        }

        return (itemsFullyLost, valueLost);
    }

    public static void Clear(){
        ItemIds.Clear();
        ValueAtStartByItemId.Clear();
    }
}
