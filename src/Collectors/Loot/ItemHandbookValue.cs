using System;
using System.Collections.Generic;
using Comfort.Common;
using EFT.HandBook;
using EFT.InventoryLogic;

namespace Softwyx.CareerLog.Collectors.Loot;

/// <summary>Handbook buyout helpers aligned with EFT container merge rules.</summary>
internal static class ItemHandbookValue{
    private static readonly List<Item> AssembledPartsBuffer = new(64);

    /// <summary>
    ///     Handbook buyout for one item.
    ///     Pass <c>itemsCount = 0</c> to use the item's
    ///     <see cref="Item.StackObjectsCount" /> (vanilla flea pricing behaviour).
    /// </summary>
    public static long BuyoutUnit(Item item, int itemsCount = 0){
        if(item?.TemplateId == null) return 0L;

        var handbook = Singleton<Handbook>.Instance;

        if(handbook == null) return 0L;

        try{
            var price = FleaItemPricing.CalculateBuyoutBasePriceForSingleItem(item, itemsCount, handbook, false);

            return price > 0d ? ToRub(price) : 0L;
        }
        catch{
            return 0L;
        }
    }

    /// <summary>Self plus slot-attached components (weapon mods, rig plates, etc.). Excludes storage grids.</summary>
    public static long Assembled(Item item){
        if(item?.TemplateId == null) return 0L;

        if(Singleton<Handbook>.Instance == null) return 0L;

        AssembledPartsBuffer.Clear();
        item.GetAllItemsNonAlloc(AssembledPartsBuffer, true);

        if(AssembledPartsBuffer.Count == 0) return BuyoutUnit(item);

        var total = 0L;

        foreach(var part in AssembledPartsBuffer){
            if(part == null) continue;

            total += BuyoutUnit(part);
        }

        return total;
    }

    /// <summary>Assembled value plus each storage-grid child valued recursively (inventory roots).</summary>
    public static long Aggregate(Item item){
        if(item == null) return 0L;

        var total = Assembled(item);

        if(!HasStorageGrids(item)) return total;

        foreach(var child in GetDirectGridItems(item)) total += Aggregate(child);

        return total;
    }

    internal static bool HasStorageGrids(Item item){
        return item is SearchableItem;
    }

    private static IEnumerable<Item> GetDirectGridItems(Item item){
        if(item is not CompoundItem compound || compound.Grids == null) yield break;

        foreach(var grid in compound.Grids){
            if(grid?.ContainedItems == null) continue;

            foreach(var pair in grid.ContainedItems)
                if(pair.Key != null)
                    yield return pair.Key;
        }
    }

    private static long ToRub(double price){
        return (long) Math.Floor(price);
    }
}
