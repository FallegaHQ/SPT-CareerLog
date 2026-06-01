using EFT;
using Softwyx.CareerLog.Persistence;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Session;
using System;
using System.Collections.Generic;

namespace Softwyx.CareerLog.Collectors.Stash;

internal static class SnapshotAppend{
    public static void TryAppendFromMenu(Profile profile, bool force, DateTime? utcOverride = null, string note = null){
        if(profile == null) return;

        if(RaidProfileAttribution.GetPlayedSide(profile) == RaidPlayedSide.Scav) return;

        if(!RaidProfileAttribution.TryResolveStorageProfileId(profile, out var storageProfileId)
        || string.IsNullOrEmpty(storageProfileId))
            return;

        var worth = WorthCalculator.Calculate(profile);

        TryAppendWorth(storageProfileId, worth, force, utcOverride ?? DateTime.UtcNow, note);
    }

    private static void TryAppendWorth(
        string storageProfileId, WorthResult worth, bool force, DateTime utc, string note = null
    ){
        if(string.IsNullOrEmpty(storageProfileId) || worth == null) return;

        var previous = SnapshotStore.TryGetLatest(storageProfileId);
        var isFirst  = previous == null;

        if(!SnapshotAppendRules.ShouldAppend(previous, worth, utc, force)) return;

        var notes = BuildNotes(isFirst, note);
        var snap  = BuildSnapshot(worth, previous, isFirst, utc, notes);

        if(!SnapshotStore.Append(storageProfileId, snap)) return;

        CareerLogPlugin.Log?.LogInfo(
                                     PluginInfo.Format(
                                                       $"Stash snapshot saved: {worth.TotalWorth:N0} ₽ "
                                                     + $"(Δ {snap.DeltaRubles:+#,0;-#,0;0}) profile {storageProfileId}."
                                                      )
                                    );
    }

    private static StashSnapshot BuildSnapshot(
        WorthResult worth, StashSnapshot previous, bool isFirst, DateTime utc, IReadOnlyList<string> notes
    ){
        return new StashSnapshot{
                                    Utc               = utc.ToString("o"),
                                    Rubles            = worth.Rubles,
                                    ItemsValue        = worth.ItemsValue,
                                    EquippedValue     = worth.EquippedValue,
                                    InventoryValue    = worth.InventoryValue,
                                    TotalWorth        = worth.TotalWorth,
                                    ItemCount         = worth.ItemCount,
                                    OccupiedSlotCount = worth.OccupiedSlotCount,
                                    TopItems          = worth.TopItems ?? [],
                                    DeltaRubles       = isFirst ? 0L : worth.TotalWorth - previous.TotalWorth,
                                    Notes = notes == null
                                                ? []
                                                :[
                                                     ..notes
                                                 ]
                                };
    }

    private static List<string> BuildNotes(bool isFirst, string note){
        if(!isFirst) return string.IsNullOrEmpty(note) ? [] : [note];

        var notes = new List<string>{
                                        "Tracking started."
                                    };

        if(!string.IsNullOrEmpty(note)) notes.Add(note);

        return notes;
    }
}
