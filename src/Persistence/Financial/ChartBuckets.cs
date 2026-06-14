using System;
using System.Collections.Generic;
using System.Globalization;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence.Financial;

internal static class ChartBuckets{
    public static List<ChartPoint> Build(
        IReadOnlyList<StashSnapshot> snapshots, PeriodKind kind, int offset, DateTime nowUtc
    ){
        if(snapshots == null || snapshots.Count == 0) return [];

        var (start, end, _) = PeriodRange.Resolve(kind, offset, nowUtc);
        var inRange = FilterRange(snapshots, start, end);

        if(inRange.Count == 0) return [];

        return ChartPointSampler.Sample(inRange, kind);
    }

    public static List<StashSnapshot> FilterTablePage(
        IReadOnlyList<StashSnapshot> snapshots, PeriodKind kind, int offset, DateTime nowUtc, int pageIndex,
        int                          pageSize
    ){
        var (start, end, _) = PeriodRange.Resolve(kind, offset, nowUtc);
        var inRange = FilterRange(snapshots, start, end);

        if(inRange.Count == 0) return [];

        var descending = new List<StashSnapshot>(inRange.Count);

        for(var i = inRange.Count - 1; i >= 0; i--) descending.Add(inRange[i]);

        var startIndex = Math.Max(0, pageIndex * pageSize);

        if(startIndex >= descending.Count) return [];

        var count = Math.Min(pageSize, descending.Count - startIndex);

        return descending.GetRange(startIndex, count);
    }

    public static int TablePageCount(
        IReadOnlyList<StashSnapshot> snapshots, PeriodKind kind, int offset, DateTime nowUtc, int pageSize
    ){
        var (start, end, _) = PeriodRange.Resolve(kind, offset, nowUtc);
        var inRange = FilterRange(snapshots, start, end);

        if(inRange.Count == 0) return 0;

        return (inRange.Count + pageSize - 1) / pageSize;
    }

    private static List<StashSnapshot> FilterRange(
        IReadOnlyList<StashSnapshot> snapshots, DateTime startUtc, DateTime endUtcExclusive
    ){
        var list = new List<StashSnapshot>();

        foreach(var snapshot in snapshots){
            if(snapshot == null || !TryParseUtc(snapshot.Utc, out var utc)) continue;

            if(KindIncludes(startUtc, endUtcExclusive, utc)) list.Add(snapshot);
        }

        list.Sort((a, b) => string.Compare(a.Utc, b.Utc, StringComparison.Ordinal));

        return list;
    }

    private static bool KindIncludes(DateTime startUtc, DateTime endUtcExclusive, DateTime utc){
        if(endUtcExclusive == DateTime.MaxValue) return utc >= startUtc || startUtc == DateTime.MinValue;

        return utc >= startUtc && utc < endUtcExclusive;
    }

    private static bool TryParseUtc(string value, out DateTime utc){
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out utc);
    }
}
