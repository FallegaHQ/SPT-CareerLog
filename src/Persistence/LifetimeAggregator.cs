using System;
using System.Collections.Generic;
using System.Linq;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal static class LifetimeAggregator{
    public static void ApplyRaid(ProfileRecord profileRecord, RaidRecord raid){
        if(profileRecord == null || raid == null) return;

        profileRecord.Lifetime ??= new LifetimeStats();
        var lifetime = profileRecord.Lifetime;
        var outcome  = RaidExitStatus.Classify(raid.ExitStatus);

        lifetime.RaidsRecorded++;

        switch(outcome){
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

        if(RaidExitStatus.IsLoss(outcome)) lifetime.RaidsFailed++;

        if(raid.VanillaSessionCountersLong.TryGetValue(VanillaSessionCounterKeys.Kills, out var kills))
            lifetime.TotalKills += kills;

        if(raid.VanillaSessionCountersLong.TryGetValue(VanillaSessionCounterKeys.HeadShots, out var headShots))
            lifetime.TotalHeadShots += headShots;

        if(!string.IsNullOrEmpty(raid.LocationId)){
            var count = lifetime.RaidsByLocation.GetValueOrDefault(raid.LocationId, 0);

            lifetime.RaidsByLocation[raid.LocationId] = count + 1;
            lifetime.FavoriteLocationId = lifetime.RaidsByLocation.
                                                   OrderByDescending(pair => pair.Value).
                                                   ThenBy(pair => pair.Key, StringComparer.Ordinal).
                                                   First().
                                                   Key;
        }

        RecalculateRates(lifetime);
    }

    public static void RecalculateRates(LifetimeStats lifetime){
        if(lifetime is not{
                              RaidsRecorded: > 0
                          }){
            if(lifetime == null) return;

            lifetime.SurvivalRate = 0;
            lifetime.ExtractRate  = 0;

            return;
        }

        lifetime.RaidsFailed = lifetime.RaidsKilled + lifetime.RaidsMissingInAction + lifetime.RaidsLeft;

        lifetime.SurvivalRate = (double) lifetime.RaidsSurvived                              / lifetime.RaidsRecorded;
        lifetime.ExtractRate  = (double) (lifetime.RaidsSurvived + lifetime.RaidsRunThrough) / lifetime.RaidsRecorded;
    }
}
