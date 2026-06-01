using System.Collections.Generic;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal readonly struct ProfileRecordsView(IReadOnlyList<RaidIndexEntry> raidIndex, ProfileRecord profileRecord){
    public static ProfileRecordsView Empty => default;

    private IReadOnlyList<RaidIndexEntry> RaidIndex{
        get;
    } = raidIndex ?? [];

    private ProfileRecord ProfileRecord{
        get;
    } = profileRecord;

    public IReadOnlyList<RaidIndexEntry> FilterRaids(SideFilter filter){
        var source = RaidIndex;

        if(source.Count == 0 || filter == SideFilter.Combined) return source;

        var matchCount = 0;

        foreach(var match in source)
            if(SideFilterValues.Matches(match, filter))
                matchCount++;

        if(matchCount == 0) return [];

        if(matchCount == source.Count) return source;

        var filtered = new List<RaidIndexEntry>(matchCount);

        foreach(var entry in source)
            if(SideFilterValues.Matches(entry, filter))
                filtered.Add(entry);

        return filtered;
    }

    public LifetimeStats GetLifetimeForEntries(IReadOnlyList<RaidIndexEntry> entries, SideFilter filter){
        if(filter != SideFilter.Combined || ProfileRecord?.Lifetime == null)
            return FilteredLifetimeBuilder.FromRaidIndex(entries);

        LifetimeAggregator.RecalculateRates(ProfileRecord.Lifetime);

        return ProfileRecord.Lifetime;
    }
}
