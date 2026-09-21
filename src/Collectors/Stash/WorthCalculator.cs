using System;
using System.Collections.Generic;
using EFT;
using EFT.InventoryLogic;
using Softwyx.CareerLog.Collectors.Loot;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Collectors.Stash;

internal static class WorthCalculator{
    private const EPlayerItems WorthMask = EPlayerItems.Stash | EPlayerItems.Equipment;

    public static WorthResult Calculate(Profile profile){
        var result = new WorthResult();

        if(profile?.Inventory == null) return result;

        var inventory = profile.Inventory;
        var seenIds   = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var topItems  = new List<ValuationEntry>();

        var rubles = 0L;

        foreach(var item in inventory.GetPlayerItems(WorthMask)){
            if(item == null) continue;

            var id = item.Id;

            if(string.IsNullOrEmpty(id) || !seenIds.Add(id)) continue;

            if(!IsCurrency(item)) continue;

            rubles += CurrencyRubles(item);
        }

        if(inventory.Equipment?.Slots != null)
            foreach(var slot in inventory.Equipment.Slots){
                var root = slot?.ContainedItem;

                if(root == null || IsCurrency(root)) continue;

                if(TopItemExclusion.ShouldExclude(root)) continue;

                var value = LootHandbookPricing.EstimateStandaloneValue(root);
                result.EquippedValue += LootHandbookPricing.EstimateInventoryRoot(root);
                topItems.Add(ValuationEntry.FromItem(root, value));
            }

        var stash = inventory.Stash;

        if(stash != null)
            foreach(var item in GetStashTopLevelItems(stash)){
                if(item == null || IsCurrency(item)) continue;

                if(TopItemExclusion.ShouldExclude(item)) continue;

                var value = LootHandbookPricing.EstimateStandaloneValue(item);
                result.InventoryValue += LootHandbookPricing.EstimateInventoryRoot(item);
                topItems.Add(ValuationEntry.FromItem(item, value));
            }

        result.Rubles            = rubles;
        result.ItemsValue        = result.EquippedValue + result.InventoryValue;
        result.TotalWorth        = result.Rubles        + result.ItemsValue;
        result.ItemCount         = seenIds.Count;
        result.OccupiedSlotCount = CountOccupiedSlots(inventory);
        result.TopItems          = BuildTopItems(topItems, 10);

        return result;
    }

    private static IEnumerable<Item> GetStashTopLevelItems(EFT.InventoryLogic.Stash stash){
        var grid = stash?.Grid;

        if(grid?.ContainedItems == null) yield break;

        foreach(var pair in grid.ContainedItems)
            if(pair.Key != null)
                yield return pair.Key;
    }

    private static int CountOccupiedSlots(Inventory inventory){
        var count = 0;

        foreach(var item in inventory.GetPlayerItems(WorthMask)){
            if(item == null) continue;

            count += Math.Max(1, item.StackObjectsCount);
        }

        return count;
    }

    private static long CurrencyRubles(Item item){
        if(item == null) return 0L;

        var handbook = LootHandbookPricing.EstimateLootUnit(item);

        if(handbook > 0L) return handbook;

        if(CurrencyRegistry.TryGetCurrencyType(item.TemplateId, out var type) && type == ECurrencyType.RUB)
            return item.StackObjectsCount;

        return 0L;
    }

    private static bool IsCurrency(Item item){
        return item != null && CurrencyRegistry.TryGetCurrencyType(item.TemplateId, out _);
    }

    private static List<StashSnapshotTopItem> BuildTopItems(List<ValuationEntry> entries, int max){
        entries.Sort((a, b) => b.ValueRub.CompareTo(a.ValueRub));

        var result = new List<StashSnapshotTopItem>();
        var seen   = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach(var entry in entries){
            if(entry.ValueRub <= 0L) continue;

            var key = entry.TemplateId ?? entry.Name;

            if(!seen.Add(key)) continue;

            result.Add(
                       new StashSnapshotTopItem{
                                                   TemplateId = entry.TemplateId,
                                                   Name       = entry.Name,
                                                   ValueRub   = entry.ValueRub,
                                                   Count      = entry.Count
                                               }
                      );

            if(result.Count >= max) break;
        }

        return result;
    }

    private sealed class ValuationEntry{
        public int    Count = 1;
        public string Name;
        public string TemplateId;
        public long   ValueRub;

        public static ValuationEntry FromItem(Item item, long valueRub){
            var templateId = item?.TemplateId.ToString();
            var name       = ItemLocale.ShortName(templateId) ?? ItemLocale.Name(templateId) ?? templateId;

            return new ValuationEntry{
                                         TemplateId = templateId,
                                         Name       = name,
                                         ValueRub   = valueRub,
                                         Count      = Math.Max(1, item?.StackObjectsCount ?? 1)
                                     };
        }
    }
}
