using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal static class RaidIndexAggregator{
    public const  string RaidsIndexFileName = "raids-index.json";
    private const string MapStatsFileName   = "map-stats.json";

    public static void ApplyRaidSave(string profileDir, RaidRecord raid){
        if(string.IsNullOrEmpty(profileDir) || raid == null || string.IsNullOrEmpty(raid.RaidId)) return;

        UpsertRaidsIndex(profileDir, raid);
        UpsertMapStats(profileDir, raid);
    }

    public static IReadOnlyList<RaidIndexEntry> ListRaids(string profileDir){
        if(string.IsNullOrEmpty(profileDir)) return [];

        var indexPath = Path.Combine(profileDir, RaidsIndexFileName);

        if(!File.Exists(indexPath)) return RebuildIndexFromRaidFiles(profileDir);

        var index = TryLoadIndex(indexPath);

        if(index?.Raids != null)
            return SortEntries(index.Raids).
                ToList();

        return RebuildIndexFromRaidFiles(profileDir);
    }

    private static RaidIndexEntry CreateEntry(RaidRecord raid){
        if(raid == null) return null;

        raid.VanillaSessionCountersLong.TryGetValue(VanillaSessionCounterKeys.Kills,     out var kills);
        raid.VanillaSessionCountersLong.TryGetValue(VanillaSessionCounterKeys.HeadShots, out var headShots);

        return new RaidIndexEntry{
                                     RaidId          = raid.RaidId,
                                     EndedUtc        = raid.EndedUtc,
                                     StartedUtc      = raid.StartedUtc,
                                     LocationId      = raid.LocationId,
                                     ExitStatus      = raid.ExitStatus,
                                     PlayedSide      = raid.PlayedSide,
                                     DurationSeconds = raid.DurationSeconds,
                                     Kills           = kills,
                                     HeadShots       = headShots
                                 };
    }

    private static void UpsertRaidsIndex(string profileDir, RaidRecord raid){
        var indexPath = Path.Combine(profileDir, RaidsIndexFileName);
        var index     = File.Exists(indexPath) ? TryLoadIndex(indexPath) : new RaidsIndexFile();

        index.Raids ??= [];

        index.Raids.RemoveAll(entry => string.Equals(entry.RaidId, raid.RaidId, StringComparison.Ordinal));
        index.Raids.Add(CreateEntry(raid));

        index.Raids = SortEntries(index.Raids).
            ToList();
        File.WriteAllText(indexPath, PersistenceJson.Serialize(index));
    }

    private static void UpsertMapStats(string profileDir, RaidRecord raid){
        if(string.IsNullOrEmpty(raid.LocationId)) return;

        var statsPath = Path.Combine(profileDir, MapStatsFileName);
        var stats     = File.Exists(statsPath) ? TryLoadMapStats(statsPath) : new MapStatsFile();

        stats.RaidsByLocation ??= new Dictionary<string, int>(StringComparer.Ordinal);

        var count = stats.RaidsByLocation.GetValueOrDefault(raid.LocationId, 0);
        stats.RaidsByLocation[raid.LocationId] = count + 1;

        stats.FavoriteLocationId = stats.RaidsByLocation.
                                         OrderByDescending(pair => pair.Value).
                                         ThenBy(pair => pair.Key, StringComparer.Ordinal).
                                         First().
                                         Key;

        File.WriteAllText(statsPath, PersistenceJson.Serialize(stats));
    }

    private static IReadOnlyList<RaidIndexEntry> RebuildIndexFromRaidFiles(string profileDir){
        var raidsDir = Path.Combine(profileDir, "raids");

        if(!Directory.Exists(raidsDir)) return [];

        var entries = new List<RaidIndexEntry>();

        foreach(var path in Directory.GetFiles(raidsDir, "*.json"))
            try{
                var raid  = PersistenceJson.Deserialize<RaidRecord>(File.ReadAllText(path));
                var entry = CreateEntry(raid);

                if(entry != null) entries.Add(entry);
            }
            catch(Exception ex){
                CareerLogPlugin.Log?.LogWarning(
                                                PluginInfo.Format(
                                                                  $"Skipped raid file while rebuilding index: {ex.Message}"
                                                                 )
                                               );
            }

        var sorted = SortEntries(entries).
            ToList();
        var index = new RaidsIndexFile{
                                          Raids = sorted
                                      };
        var indexPath = Path.Combine(profileDir, RaidsIndexFileName);
        File.WriteAllText(indexPath, PersistenceJson.Serialize(index));

        RebuildMapStatsFromIndex(profileDir, sorted);

        return sorted;
    }

    private static void RebuildMapStatsFromIndex(string profileDir, IReadOnlyList<RaidIndexEntry> entries){
        var stats = new MapStatsFile{
                                        RaidsByLocation = new Dictionary<string, int>(StringComparer.Ordinal)
                                    };

        foreach(var entry in entries){
            if(entry == null || string.IsNullOrEmpty(entry.LocationId)) continue;

            var count = stats.RaidsByLocation.GetValueOrDefault(entry.LocationId, 0);
            stats.RaidsByLocation[entry.LocationId] = count + 1;
        }

        if(stats.RaidsByLocation.Count == 0) return;

        stats.FavoriteLocationId = stats.RaidsByLocation.
                                         OrderByDescending(pair => pair.Value).
                                         ThenBy(pair => pair.Key, StringComparer.Ordinal).
                                         First().
                                         Key;

        var statsPath = Path.Combine(profileDir, MapStatsFileName);
        File.WriteAllText(statsPath, PersistenceJson.Serialize(stats));
    }

    private static IEnumerable<RaidIndexEntry> SortEntries(IEnumerable<RaidIndexEntry> entries){
        return entries.Where(entry => entry != null && !string.IsNullOrEmpty(entry.RaidId)).
                       OrderByDescending(entry => entry.EndedUtc, StringComparer.Ordinal).
                       ThenByDescending(entry => entry.StartedUtc, StringComparer.Ordinal);
    }

    private static RaidsIndexFile TryLoadIndex(string path){
        try{
            return PersistenceJson.Deserialize<RaidsIndexFile>(File.ReadAllText(path));
        }
        catch(Exception ex){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Could not load raids index: {ex.Message}"));

            return null;
        }
    }

    private static MapStatsFile TryLoadMapStats(string path){
        try{
            return PersistenceJson.Deserialize<MapStatsFile>(File.ReadAllText(path));
        }
        catch(Exception ex){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Could not load map stats: {ex.Message}"));

            return null;
        }
    }
}
