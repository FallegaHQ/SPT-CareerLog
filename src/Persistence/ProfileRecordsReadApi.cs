using System;
using System.Collections.Generic;
using System.IO;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

/// <summary>Menu RECORDS read surface: one disk read per profile until data files change.</summary>
internal static class ProfileRecordsReadApi{
    private static CacheEntry _cache;

    public static ProfileRecordsView Get(string profileId){
        if(string.IsNullOrEmpty(profileId)) return ProfileRecordsView.Empty;

        var profileDir  = ProfileRecordStore.GetProfileDataDirectory(profileId);
        var indexPath   = Path.Combine(profileDir, RaidIndexAggregator.RaidsIndexFileName);
        var profilePath = Path.Combine(profileDir, "profile-record.json");
        var indexTicks = File.Exists(indexPath)
                             ? File.GetLastWriteTimeUtc(indexPath).
                                    Ticks
                             : 0L;
        var profileTicks = File.Exists(profilePath)
                               ? File.GetLastWriteTimeUtc(profilePath).
                                      Ticks
                               : 0L;
        var stashIndexTicks = SnapshotStore.GetIndexTicks(profileId);

        if(_cache != null
        && string.Equals(_cache.ProfileId, profileId, StringComparison.Ordinal)
        && _cache.IndexTicks      == indexTicks
        && _cache.ProfileTicks    == profileTicks
        && _cache.StashIndexTicks == stashIndexTicks)
            return new ProfileRecordsView(_cache.RaidIndex, _cache.ProfileRecord);

        var raidIndex     = ProfileRecordStore.ListRaidIndex(profileId);
        var profileRecord = ProfileRecordStore.LoadProfileRecord(profileId);

        _cache = new CacheEntry{
                                   ProfileId       = profileId,
                                   IndexTicks      = indexTicks,
                                   ProfileTicks    = profileTicks,
                                   StashIndexTicks = stashIndexTicks,
                                   RaidIndex       = raidIndex,
                                   ProfileRecord   = profileRecord
                               };

        return new ProfileRecordsView(raidIndex, profileRecord);
    }

    public static void Invalidate(string profileId){
        if(_cache != null && string.Equals(_cache.ProfileId, profileId, StringComparison.Ordinal)) _cache = null;
    }

    public static void Clear(){
        _cache = null;
    }

    public static RaidRecord LoadRaid(string profileId, string raidId){
        return ProfileRecordStore.LoadRaid(profileId, raidId);
    }

    private sealed class CacheEntry{
        public long                          IndexTicks;
        public string                        ProfileId;
        public ProfileRecord                 ProfileRecord;
        public long                          ProfileTicks;
        public IReadOnlyList<RaidIndexEntry> RaidIndex;
        public long                          StashIndexTicks;
    }
}
