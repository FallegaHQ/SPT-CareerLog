using EFT.InventoryLogic;

namespace Softwyx.CareerLog.Collectors.Loot;

/// <summary>Handbook-aligned item values used by loot markers, loadout totals and stash snapshots.</summary>
internal static class LootHandbookPricing{
    /// <summary>Loot unit picked up during a raid (container only; contents tracked separately).</summary>
    public static long EstimateLootUnit(Item item){
        if(item == null) return 0L;

        return ItemHandbookValue.HasStorageGrids(item)
                   ? ItemHandbookValue.BuyoutUnit(item)
                   : ItemHandbookValue.Assembled(item);
    }

    /// <summary>
    ///     Handbook value for an inventory root (equipment slot or stash cell):
    ///     assembled parts plus nested storage-grid contents, valued recursively.
    /// </summary>
    public static long EstimateInventoryRoot(Item item){
        return ItemHandbookValue.Aggregate(item);
    }

    /// <summary>Rank/display value without nested storage-grid contents (top-items lists).</summary>
    public static long EstimateStandaloneValue(Item item){
        return ItemHandbookValue.Assembled(item);
    }
}
