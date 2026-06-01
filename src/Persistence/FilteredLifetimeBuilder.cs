using System;
using System.Collections.Generic;
using System.Linq;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

/// <summary>Derives lifetime stats from the raids index (no full raid JSON load).</summary>
internal static class FilteredLifetimeBuilder{
    public static LifetimeStats FromRaidIndex(IReadOnlyList<RaidIndexEntry> entries){
        var lifetime = new LifetimeStats{
                                            RaidsByLocation = new Dictionary<string, int>(StringComparer.Ordinal)
                                        };

        if(entries == null || entries.Count == 0) return lifetime;

        foreach(var entry in entries){
            if(entry == null) continue;

            lifetime.RaidsRecorded++;

            switch(RaidExitStatus.Classify(entry.ExitStatus)){
                case RaidExitStatus.OutcomeCategory.Survived:
                    lifetime.RaidsSurvived++;

                    break;
                case RaidExitStatus.OutcomeCategory.RunThrough:
                    lifetime.RaidsRunThrough++;

                    break;
                case RaidExitStatus.OutcomeCategory.Killed:
                    lifetime.RaidsKilled++;

                    break;
                case RaidExitStatus.OutcomeCategory.MissingInAction:
                    lifetime.RaidsMissingInAction++;

                    break;
                case RaidExitStatus.OutcomeCategory.Left:
                    lifetime.RaidsLeft++;

                    break;
                case RaidExitStatus.OutcomeCategory.Transit:
                    lifetime.RaidsTransit++;

                    break;
                case RaidExitStatus.OutcomeCategory.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            lifetime.TotalKills     += entry.Kills;
            lifetime.TotalHeadShots += entry.HeadShots;

            if(string.IsNullOrEmpty(entry.LocationId)) continue;

            var count = lifetime.RaidsByLocation.GetValueOrDefault(entry.LocationId, 0);
            lifetime.RaidsByLocation[entry.LocationId] = count + 1;
        }

        if(lifetime.RaidsByLocation.Count > 0)
            lifetime.FavoriteLocationId = lifetime.RaidsByLocation.
                                                   OrderByDescending(pair => pair.Value).
                                                   ThenBy(pair => pair.Key, StringComparer.Ordinal).
                                                   First().
                                                   Key;

        LifetimeAggregator.RecalculateRates(lifetime);

        return lifetime;
    }
}
