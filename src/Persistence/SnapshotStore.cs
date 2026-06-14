using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Softwyx.CareerLog.Persistence.Financial;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal static class SnapshotStore{
    private const string FolderName = "stash-snapshots";
    private const string IndexFile  = "index.json";

    public static StashSnapshotIndex LoadIndex(string profileId){
        if(string.IsNullOrEmpty(profileId)) return null;

        var path = GetIndexPath(profileId);

        if(!File.Exists(path)) return null;

        try{
            var index = PersistenceJson.Deserialize<StashSnapshotIndex>(File.ReadAllText(path));

            return NormalizeIndex(index, profileId);
        }
        catch(Exception ex){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Could not load stash snapshot index: {ex.Message}"));

            return null;
        }
    }

    public static StashSnapshot TryGetLatest(string profileId){
        var index = LoadIndex(profileId);

        return index?.Latest;
    }

    public static bool Append(string profileId, StashSnapshot snapshot){
        if(string.IsNullOrEmpty(profileId) || snapshot == null || string.IsNullOrEmpty(snapshot.Utc)) return false;

        if(!TryParseUtc(snapshot.Utc, out var utc)) return false;

        var dayKey = ToLocalDayKey(utc);
        var dayFile = LoadDayFile(profileId, dayKey)
                   ?? new StashSnapshotDayFile{
                                                  Day = dayKey
                                              };

        dayFile.Snapshots ??= [];
        dayFile.Snapshots.Add(NormalizeSnapshot(snapshot));

        SaveDayFile(profileId, dayFile);

        var index = LoadIndex(profileId)
                 ?? new StashSnapshotIndex{
                                              ProfileId = profileId
                                          };

        index.ProfileId = profileId;

        if(string.IsNullOrEmpty(index.FirstUtc)){
            index.FirstUtc        = snapshot.Utc;
            index.FirstTotalWorth = snapshot.TotalWorth;
        }

        index.LatestUtc = snapshot.Utc;
        index.Latest    = snapshot;

        UpsertDayEntry(index, dayKey, snapshot.Utc);

        SaveIndex(profileId, index);
        ProfileRecordsReadApi.Invalidate(profileId);

        return true;
    }

    public static List<StashSnapshot> LoadForPeriod(string profileId, PeriodKind kind, int offset, DateTime nowUtc){
        var index = LoadIndex(profileId);

        if(index?.Days == null || index.Days.Count == 0) return [];

        var (startUtc, endUtc, _) = PeriodRange.Resolve(kind, offset, nowUtc);

        var result = new List<StashSnapshot>();

        if(kind == PeriodKind.Lifetime){
            foreach(var dayEntry in index.Days){
                if(dayEntry == null || string.IsNullOrEmpty(dayEntry.Day)) continue;

                var dayFile = LoadDayFile(profileId, dayEntry.Day);

                if(dayFile?.Snapshots == null) continue;

                foreach(var snapshot in dayFile.Snapshots){
                    if(snapshot == null || !TryParseUtc(snapshot.Utc, out _)) continue;

                    result.Add(snapshot);
                }
            }

            result.Sort((a, b) => string.Compare(a.Utc, b.Utc, StringComparison.Ordinal));

            return result;
        }

        foreach(var dayEntry in index.Days){
            if(dayEntry == null || string.IsNullOrEmpty(dayEntry.Day)) continue;

            if(!DayOverlapsPeriod(dayEntry.Day, startUtc, endUtc)) continue;

            var dayFile = LoadDayFile(profileId, dayEntry.Day);

            if(dayFile?.Snapshots == null) continue;

            foreach(var snapshot in dayFile.Snapshots){
                if(snapshot == null || !TryParseUtc(snapshot.Utc, out var utc)) continue;

                if(utc < startUtc || utc >= endUtc) continue;

                result.Add(snapshot);
            }
        }

        result.Sort((a, b) => string.Compare(a.Utc, b.Utc, StringComparison.Ordinal));

        return result;
    }

    private static string GetSnapshotsDirectory(string profileId){
        return Path.Combine(ProfileRecordStore.GetProfileDataDirectory(profileId), FolderName);
    }

    internal static long GetIndexTicks(string profileId){
        var path = GetIndexPath(profileId);

        return File.Exists(path)
                   ? File.GetLastWriteTimeUtc(path).
                          Ticks
                   : 0L;
    }

    private static void UpsertDayEntry(StashSnapshotIndex index, string dayKey, string utc){
        index.Days ??= [];

        foreach(var entry in index.Days){
            if(!string.Equals(entry.Day, dayKey, StringComparison.Ordinal)) continue;

            entry.Count++;
            entry.LastUtc = utc;

            return;
        }

        index.Days.Add(
                       new StashSnapshotIndexDay{
                                                    Day      = dayKey,
                                                    Count    = 1,
                                                    FirstUtc = utc,
                                                    LastUtc  = utc
                                                }
                      );

        index.Days.Sort((a, b) => string.Compare(a.Day, b.Day, StringComparison.Ordinal));
    }

    private static bool DayOverlapsPeriod(string dayKey, DateTime startUtc, DateTime endUtcExclusive){
        if(!DateTime.TryParseExact(
                                   dayKey,
                                   "yyyy-MM-dd",
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.None,
                                   out var day
                                  ))
            return false;

        var dayStart = day.ToUniversalTime();
        var dayEnd = day.AddDays(1).
                         ToUniversalTime();

        if(endUtcExclusive == DateTime.MaxValue) return dayEnd > startUtc;

        return dayStart < endUtcExclusive && dayEnd > startUtc;
    }

    private static StashSnapshotDayFile LoadDayFile(string profileId, string dayKey){
        var path = GetDayFilePath(profileId, dayKey);

        if(!File.Exists(path)) return null;

        try{
            var dayFile = PersistenceJson.Deserialize<StashSnapshotDayFile>(File.ReadAllText(path));

            if(dayFile == null) return null;

            dayFile.Day       ??= dayKey;
            dayFile.Snapshots ??= [];

            foreach(var snapshot in dayFile.Snapshots) NormalizeSnapshot(snapshot);

            return dayFile;
        }
        catch(Exception ex){
            CareerLogPlugin.Log?.LogWarning(
                                            PluginInfo.Format($"Skipped invalid stash day file {dayKey}: {ex.Message}")
                                           );

            return null;
        }
    }

    private static void SaveDayFile(string profileId, StashSnapshotDayFile dayFile){
        var dir = GetSnapshotsDirectory(profileId);
        Directory.CreateDirectory(dir);

        var path = GetDayFilePath(profileId, dayFile.Day);
        File.WriteAllText(path, PersistenceJson.Serialize(dayFile));
    }

    private static void SaveIndex(string profileId, StashSnapshotIndex index){
        var dir = GetSnapshotsDirectory(profileId);
        Directory.CreateDirectory(dir);

        var path = GetIndexPath(profileId);
        File.WriteAllText(path, PersistenceJson.Serialize(NormalizeIndex(index, profileId)));
    }

    private static StashSnapshotIndex NormalizeIndex(StashSnapshotIndex index, string profileId){
        if(index == null) return null;

        if(index.SchemaVersion <= 0) index.SchemaVersion = 1;

        index.ProfileId =   profileId;
        index.Days      ??= [];

        if(index.Latest != null) NormalizeSnapshot(index.Latest);

        return index;
    }

    private static StashSnapshot NormalizeSnapshot(StashSnapshot snapshot){
        if(snapshot == null) return null;

        snapshot.TopItems ??= [];
        snapshot.Notes    ??= [];

        if(snapshot.TotalWorth <= 0L && snapshot.Rubles > 0L && snapshot.ItemsValue > 0L)
            snapshot.TotalWorth = snapshot.Rubles + snapshot.ItemsValue;

        return snapshot;
    }

    private static string GetIndexPath(string profileId){
        return Path.Combine(GetSnapshotsDirectory(profileId), IndexFile);
    }

    private static string GetDayFilePath(string profileId, string dayKey){
        return Path.Combine(GetSnapshotsDirectory(profileId), $"{dayKey}.json");
    }

    private static string ToLocalDayKey(DateTime utc){
        return utc.ToLocalTime().
                   Date.
                   ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static bool TryParseUtc(string value, out DateTime utc){
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out utc);
    }
}
