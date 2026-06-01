using EFT.InventoryLogic;
using Softwyx.CareerLog.Persistence.Models;
using System;
using System.Collections.Generic;

namespace Softwyx.CareerLog.Collectors.Loot;

internal static class LootMarkerCollector{
    /// <summary>
    /// Loot ledger -- instance ids first picked up during this raid (excludes bring-in gear).
    /// </summary>
    private static readonly HashSet<string> CountedItems = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Value/template facts for ledger ids only.
    /// </summary>
    private static readonly Dictionary<string, LootFact> FactsByItemId = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Current ownership -- updated by inventory add/remove events and <see cref="LootExtractInventoryReconcile"/>.
    /// </summary>
    private static readonly HashSet<string> OwnedItemIds = new(StringComparer.OrdinalIgnoreCase);

    private sealed class LootFact{
        public string TemplateId;
        public long   ValueRub;
    }

    public static void Clear(){
        CountedItems.Clear();
        FactsByItemId.Clear();
        OwnedItemIds.Clear();
    }

    public static void RecordAdded(Item item){
        if(item == null) return;

        var id = item.Id;

        if(string.IsNullOrEmpty(id)) return;

        OwnedItemIds.Add(id);
    }

    public static void RecordRemoved(Item item){
        if(item == null) return;

        var id = item.Id;

        if(string.IsNullOrEmpty(id)) return;

        OwnedItemIds.Remove(id);
    }

    private static void TrackOwned(string itemId){
        if(!string.IsNullOrEmpty(itemId)) OwnedItemIds.Add(itemId);
    }

    /// <summary>
    /// First-time loot registration only. Re-pickups after a drop return false but leave ownership to
    /// <see cref="TrackOwned"/> / inventory events. Bring-in gear is never registered.
    /// </summary>
    private static bool TryRegisterNewLoot(Item item, out long valueRub){
        valueRub = 0L;

        if(item == null) return false;

        var itemId = item.Id;

        if(string.IsNullOrEmpty(itemId)) return false;

        if(StartingLootBaseline.Contains(itemId)) return false;

        if(!CountedItems.Add(itemId)) return false;

        valueRub = LootHandbookPricing.EstimateLootUnit(item);

        TrackFact(item, valueRub);

        return true;
    }

    public static void RecordPickup(Item item){
        switch(item){
            case null:
                return;
            case SearchableItemItemClass searchableItemItem:
                RecordContainerPickup(searchableItemItem);

                return;
        }

        var itemId = item.Id;

        if(string.IsNullOrEmpty(itemId)) return;

        TrackOwned(itemId);

        if(!TryRegisterNewLoot(item, out var value)) return;

        if(!LootMarkerFilter.ShouldMark(item, value)) return;

        var player = LocalRaidPlayer.Instance;

        if(player == null) return;

        RaidEventMarkerBuffer.Add(
                                  new RaidMovementValue{
                                                           Point          = MarkerMapPosition.FromPlayer(player),
                                                           Type           = RaidMarkerTypes.Loot,
                                                           UtcOffsetSec   = RaidEventClock.ElapsedSeconds(),
                                                           ItemTemplateId = item.TemplateId.ToString(),
                                                           ItemTemplateIds =[
                                                                                item.TemplateId.ToString()
                                                                            ],
                                                           ItemIds =[
                                                                        itemId
                                                                    ],
                                                           ValueRub = value
                                                       }
                                 );
    }

    private static void RecordContainerPickup(SearchableItemItemClass container){
        var player = LocalRaidPlayer.Instance;

        if(player == null) return;

        // Always count the container itself once.
        RecordPickupSingle(container);

        // Then recursively walk nested containers and collect non-container "loot units".
        var lootUnits = CollectRecursiveLootUnits(container);

        // Build ONE marker for all qualifying items (for accessibility), at the container pickup location.
        var qualifyingIds   = new List<string>();
        var qualifyingTpls  = new List<string>();
        var qualifyingValue = 0L;
        var qualifyingCount = 0;

        foreach(var unit in lootUnits){
            if(unit == null) continue;

            var id = unit.Id;

            if(string.IsNullOrEmpty(id)) continue;

            // Count each loot unit once for stats.
            RecordPickupSingle(unit);

            var value = LootHandbookPricing.EstimateLootUnit(unit);

            if(!LootMarkerFilter.ShouldMark(unit, value)) continue;

            qualifyingIds.Add(id);
            qualifyingTpls.Add(unit.TemplateId.ToString());
            qualifyingValue += value;
            qualifyingCount++;
        }

        if(qualifyingCount <= 0) return;

        RaidEventMarkerBuffer.Add(
                                  new RaidMovementValue{
                                                           Point           = MarkerMapPosition.FromPlayer(player),
                                                           Type            = RaidMarkerTypes.Loot,
                                                           UtcOffsetSec    = RaidEventClock.ElapsedSeconds(),
                                                           ValueRub        = qualifyingValue,
                                                           ItemIds         = qualifyingIds,
                                                           ItemTemplateIds = qualifyingTpls
                                                       }
                                 );
    }

    private static void RecordPickupSingle(Item item){
        if(item == null) return;

        TrackOwned(item.Id);
        TryRegisterNewLoot(item, out _);
    }

    private static List<Item> CollectRecursiveLootUnits(SearchableItemItemClass root){
        var result = new List<Item>();
        var seen   = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var queue  = new Queue<SearchableItemItemClass>();
        queue.Enqueue(root);

        while(queue.Count > 0){
            var container = queue.Dequeue();

            if(container == null) continue;

            // Enumerate DIRECT children of this container.
            foreach(var sub in container.GetAllItems()){
                if(sub == null || ReferenceEquals(sub, container)) continue;

                if(!IsDirectChild(container, sub)) continue;

                var id = sub.Id;

                if(!seen.Add(id)) continue;

                if(sub is SearchableItemItemClass nested){
                    // Nested containers get their own count entry via RecordPickupSingle when processed as a loot unit
                    // (called by RecordContainerPickup's loop).
                    queue.Enqueue(nested);
                    result.Add(nested);

                    continue;
                }

                // Non-container loot unit; DO NOT descend into its attachments (weapons/mods, armor/plates).
                // Pricing handles compound value where appropriate.
                result.Add(sub);
            }
        }

        return result;
    }

    private static bool IsDirectChild(Item container, Item sub){
        try{
            var address    = sub.CurrentAddress;
            var parentItem = address?.Container?.ParentItem;

            return parentItem != null && ReferenceEquals(parentItem, container);
        }
        catch{
            return false;
        }
    }

    public static (int items, long valueRub) TakeAggregates(){
        var items = 0;
        var value = 0L;

        // OwnedItemIds is reconciled at extract; dropped/used/consumed loot is already excluded.
        foreach(var id in OwnedItemIds){
            if(string.IsNullOrEmpty(id)) continue;
            if(!FactsByItemId.TryGetValue(id, out var fact) || fact == null) continue;

            items++;
            value += fact.ValueRub;
        }

        return (items, value);
    }

    public static List<RaidLootItemEntry> TakeTopLooted(int max){
        if(max <= 0 || OwnedItemIds.Count == 0) return [];

        var byTpl = new Dictionary<string, RaidLootItemEntry>(StringComparer.OrdinalIgnoreCase);

        foreach(var id in OwnedItemIds){
            if(string.IsNullOrEmpty(id)) continue;
            if(!FactsByItemId.TryGetValue(id, out var fact) || fact == null) continue;
            if(string.IsNullOrEmpty(fact.TemplateId)) continue;

            if(!byTpl.TryGetValue(fact.TemplateId, out var entry)){
                entry = new RaidLootItemEntry{
                                                 TemplateId = fact.TemplateId,
                                                 Count      = 0,
                                                 ValueRub   = fact.ValueRub
                                             };
                byTpl[fact.TemplateId] = entry;
            }

            entry.Count++;
            if(fact.ValueRub > entry.ValueRub) entry.ValueRub = fact.ValueRub;
        }

        var list = new List<RaidLootItemEntry>(byTpl.Values);
        list.Sort((a, b) => b.ValueRub.CompareTo(a.ValueRub));

        if(list.Count > max) list.RemoveRange(max, list.Count - max);

        return list;
    }

    private static void TrackFact(Item item, long valueRub){
        var id  = item?.Id;
        var tpl = item?.TemplateId.ToString();

        if(string.IsNullOrEmpty(id) || string.IsNullOrEmpty(tpl)) return;

        FactsByItemId[id] = new LootFact{
                                            TemplateId = tpl,
                                            ValueRub   = valueRub
                                        };
    }

    public static void RecordDropped(Item item){
        RecordRemoved(item);
    }

    /// <summary>
    /// At extract, keep only loot-ledger items still on the player (covers dead-body transfers and other
    /// paths that never fire inventory remove events). Bring-in gear is not in the ledger and is unaffected.
    /// </summary>
    public static void SyncOwnedFromInventory(HashSet<string> inventoryItemIds){
        OwnedItemIds.Clear();

        foreach(var id in CountedItems){
            if(string.IsNullOrEmpty(id)) continue;

            if(inventoryItemIds != null && inventoryItemIds.Contains(id)) OwnedItemIds.Add(id);
        }
    }

    public static void PruneLootMarkers(List<RaidMovementValue> markers){
        if(markers == null || markers.Count == 0) return;

        for(var i = markers.Count - 1; i >= 0; i--){
            var m = markers[i];

            if(m      == null) continue;
            if(m.Type != RaidMarkerTypes.Loot) continue;

            if(m.ItemIds is{
                               Count: > 0
                           }){
                PruneLootMarkerItems(m);

                if(m.ItemIds == null || m.ItemIds.Count == 0) markers.RemoveAt(i);

                continue;
            }

            if(!string.IsNullOrEmpty(m.ItemTemplateId) && !HasOwnedTemplate(m.ItemTemplateId)) markers.RemoveAt(i);
        }
    }

    private static void PruneLootMarkerItems(RaidMovementValue marker){
        var keptIds  = new List<string>();
        var keptTpls = new List<string>();
        var sumValue = 0L;

        foreach(var id in marker.ItemIds){
            if(string.IsNullOrEmpty(id)) continue;
            if(!OwnedItemIds.Contains(id)) continue;

            keptIds.Add(id);

            if(!FactsByItemId.TryGetValue(id, out var fact) || fact == null) continue;

            sumValue += fact.ValueRub;

            if(!string.IsNullOrEmpty(fact.TemplateId)) keptTpls.Add(fact.TemplateId);
        }

        marker.ItemIds         = keptIds.Count  > 0 ? keptIds : null;
        marker.ItemTemplateIds = keptTpls.Count > 0 ? keptTpls : null;
        marker.ItemTemplateId  = keptTpls.Count == 1 ? keptTpls[0] : null;
        marker.ValueRub        = sumValue       > 0L ? sumValue : null;
    }

    private static bool HasOwnedTemplate(string templateId){
        foreach(var id in OwnedItemIds){
            if(!FactsByItemId.TryGetValue(id, out var fact) || fact == null) continue;

            if(string.Equals(fact.TemplateId, templateId, StringComparison.OrdinalIgnoreCase)) return true;
        }

        return false;
    }
}
