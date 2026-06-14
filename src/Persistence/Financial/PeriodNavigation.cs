using System;
using System.Globalization;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence.Financial;

internal static class PeriodNavigation{
    public static bool CanShift(StashSnapshotIndex index, PeriodKind kind, int offset, int delta){
        if(index?.Days == null || index.Days.Count == 0 || kind == PeriodKind.Lifetime) return false;

        return HasSnapshotInRange(index, kind, offset + delta);
    }

    public static void ClampOffset(StashSnapshotIndex index, PeriodKind kind, ref int offset){
        if(index?.Days == null || index.Days.Count == 0 || kind == PeriodKind.Lifetime){
            offset = 0;

            return;
        }

        if(HasSnapshotInRange(index, kind, offset)) return;

        for(var probe = 0; probe >= -120; probe--)
            if(HasSnapshotInRange(index, kind, probe)){
                offset = probe;

                return;
            }

        for(var probe = 1; probe <= 120; probe++)
            if(HasSnapshotInRange(index, kind, probe)){
                offset = probe;

                return;
            }

        offset = 0;
    }

    private static bool HasSnapshotInRange(StashSnapshotIndex index, PeriodKind kind, int offset){
        var (start, end, _) = PeriodRange.Resolve(kind, offset, DateTime.UtcNow);

        if(kind == PeriodKind.Lifetime) return true;

        foreach(var day in index.Days){
            if(day == null || string.IsNullOrEmpty(day.Day)) continue;

            if(!DateTime.TryParseExact(
                                       day.Day,
                                       "yyyy-MM-dd",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out var localDay
                                      ))
                continue;

            var dayStart = localDay.ToUniversalTime();
            var dayEnd = localDay.AddDays(1).
                                  ToUniversalTime();

            if(dayStart < end && dayEnd > start) return true;
        }

        return false;
    }
}
