using Softwyx.CareerLog.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Softwyx.CareerLog.Persistence.Financial;

internal static class ChartBuckets{
    public static List<ChartPoint> Build(
        IReadOnlyList<StashSnapshot> snapshots, PeriodKind kind, int offset, DateTime nowUtc
    ){
        if(snapshots == null || snapshots.Count == 0) return [];

        var (start, end, _) = PeriodRange.Resolve(kind, offset, nowUtc);
        var inRange = FilterRange(snapshots, start, end);

        if(inRange.Count == 0) return [];

        return kind switch{
                   PeriodKind.Day      => BucketByDay(inRange, 40),
                   PeriodKind.Week     => BucketByDay(inRange, 6),
                   PeriodKind.Month    => BucketByDay(inRange, 2),
                   PeriodKind.Lifetime => BucketByDay(inRange, 1),
                   _                   => MapAll(inRange)
               };
    }

    public static List<StashSnapshot> FilterTablePage(
        IReadOnlyList<StashSnapshot> snapshots, PeriodKind kind, int offset, DateTime nowUtc, int pageIndex,
        int                          pageSize
    ){
        var points = Build(snapshots, kind, offset, nowUtc);

        if(points.Count == 0) return [];

        var descending = new List<StashSnapshot>(points.Count);

        for(var i = points.Count - 1; i >= 0; i--)
            if(points[i].Snapshot != null)
                descending.Add(points[i].Snapshot);

        var start = Math.Max(0, pageIndex * pageSize);

        if(start >= descending.Count) return [];

        var count = Math.Min(pageSize, descending.Count - start);
        var page  = descending.GetRange(start, count);

        return page;
    }

    public static int TablePageCount(
        IReadOnlyList<StashSnapshot> snapshots, PeriodKind kind, int offset, DateTime nowUtc, int pageSize
    ){
        var points = Build(snapshots, kind, offset, nowUtc);

        if(points.Count == 0) return 0;

        return (points.Count + pageSize - 1) / pageSize;
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

    private static List<ChartPoint> MapAll(List<StashSnapshot> snapshots){
        var points = new List<ChartPoint>(snapshots.Count);

        foreach(var snapshot in snapshots)
            if(TryParseUtc(snapshot.Utc, out _))
                points.Add(new ChartPoint(snapshot.TotalWorth, snapshot));

        return points;
    }

    private static List<ChartPoint> BucketByDay(List<StashSnapshot> snapshots, int maxPerDay){
        var byDay = new Dictionary<DateTime, List<StashSnapshot>>();

        foreach(var snapshot in snapshots){
            if(!TryParseUtc(snapshot.Utc, out var utc)) continue;

            var day = utc.ToLocalTime().
                          Date;

            if(!byDay.TryGetValue(day, out var list)){
                list       = [];
                byDay[day] = list;
            }

            list.Add(snapshot);
        }

        var days = new List<DateTime>(byDay.Keys);
        days.Sort();

        var points = new List<ChartPoint>();

        foreach(var day in days){
            var daySnapshots = byDay[day];
            daySnapshots.Sort((a, b) => string.Compare(a.Utc, b.Utc, StringComparison.Ordinal));
            AppendSampled(points, daySnapshots, maxPerDay);
        }

        return points;
    }

    private static void AppendSampled(List<ChartPoint> points, List<StashSnapshot> daySnapshots, int maxPerDay){
        if(daySnapshots.Count == 0) return;

        var limit = Math.Max(1, maxPerDay);

        if(daySnapshots.Count <= limit){
            foreach(var snapshot in daySnapshots)
                if(TryParseUtc(snapshot.Utc, out _))
                    points.Add(new ChartPoint(snapshot.TotalWorth, snapshot));

            return;
        }

        for(var i = 0; i < limit; i++){
            var index = (int) Math.Round(i * (daySnapshots.Count - 1) / (double) (limit - 1));
            var snap  = daySnapshots[index];

            if(TryParseUtc(snap.Utc, out _)) points.Add(new ChartPoint(snap.TotalWorth, snap));
        }
    }

    private static bool TryParseUtc(string value, out DateTime utc){
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out utc);
    }
}
