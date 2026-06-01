using Softwyx.CareerLog.Persistence.Models;
using System;
using System.Globalization;

namespace Softwyx.CareerLog.Collectors.Stash;

internal static class SnapshotAppendRules{
    public static bool ShouldAppend(StashSnapshot previous, WorthResult worth, DateTime utc, bool force){
        if(force || previous == null) return true;

        if(previous.TotalWorth != worth.TotalWorth) return true;

        return !IsSameLocalDay(previous.Utc, utc);
    }

    private static bool IsSameLocalDay(string previousUtc, DateTime utc){
        if(!DateTime.TryParse(
                              previousUtc,
                              CultureInfo.InvariantCulture,
                              DateTimeStyles.RoundtripKind,
                              out var previous
                             ))
            return false;

        return previous.ToLocalTime().
                        Date
            == utc.ToLocalTime().
                   Date;
    }
}
