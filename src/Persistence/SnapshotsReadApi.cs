using Softwyx.CareerLog.Persistence.Financial;
using Softwyx.CareerLog.Persistence.Models;
using System;
using System.Collections.Generic;

namespace Softwyx.CareerLog.Persistence;

internal static class SnapshotsReadApi{
    public static SnapshotSummaryView GetSummary(string profileId){
        if(string.IsNullOrEmpty(profileId)) return SnapshotSummaryView.Empty;

        var index = SnapshotStore.LoadIndex(profileId);

        return index?.Latest == null
                   ? SnapshotSummaryView.Empty
                   : new SnapshotSummaryView(index.Latest, index.FirstTotalWorth, HasAny(index));
    }

    public static IReadOnlyList<StashSnapshot> LoadForFinancialPeriod(
        string profileId, PeriodKind kind, int offset, DateTime nowUtc
    ){
        return string.IsNullOrEmpty(profileId) ? [] : SnapshotStore.LoadForPeriod(profileId, kind, offset, nowUtc);
    }

    public static StashSnapshotIndex LoadIndex(string profileId){
        return string.IsNullOrEmpty(profileId) ? null : SnapshotStore.LoadIndex(profileId);
    }

    private static bool HasAny(StashSnapshotIndex index){
        return index?.Latest != null
            || index?.Days is{
                                 Count: > 0
                             };
    }
}
