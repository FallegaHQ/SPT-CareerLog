using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence.Financial;

/// <summary>
///     Picks real snapshots for charts. View max is checked first; per-day caps apply only when over the view limit.
///     Anchor flags mark indices that must survive downsampling — one snapshot can satisfy several anchors.
/// </summary>
internal static class ChartPointSampler{
    public static List<ChartPoint> Sample(IReadOnlyList<StashSnapshot> inRange, PeriodKind kind){
        if(inRange == null || inRange.Count == 0) return [];

        var sorted = CopySorted(inRange);

        if(sorted.Count == 0) return [];

        var periodMax = ChartSamplingLimits.PeriodMax(kind);

        if(sorted.Count <= periodMax) return ToChartPoints(sorted);

        return kind switch{
                   PeriodKind.Lifetime => ToChartPoints(
                                                        SampleAnchored(
                                                                       sorted,
                                                                       periodMax,
                                                                       ChartAnchorFlags.LifetimeAnchors
                                                                      )
                                                       ),
                   PeriodKind.Day => ToChartPoints(SampleAnchored(sorted, periodMax, ChartAnchorFlags.DayAnchors)),
                   PeriodKind.Week or PeriodKind.Month => ToChartPoints(SampleMultiDay(sorted)),
                   _ => ToChartPoints(sorted)
               };
    }

    private static List<StashSnapshot> SampleMultiDay(List<StashSnapshot> sorted){
        var perDayMax = ChartSamplingLimits.PerDayMaxWhenReducing();
        var byDay     = GroupByLocalDay(sorted);
        var days      = new List<DateTime>(byDay.Keys);
        days.Sort();

        var picked = CreateSnapshotMap();

        foreach(var day in days){
            var daySnaps = byDay[day];

            if(daySnaps.Count == 0) continue;

            var limit = Math.Min(perDayMax, daySnaps.Count);

            foreach(var snap in SampleAnchored(daySnaps, limit, ChartAnchorFlags.DayInPeriod))
                IncludeSnapshot(picked, snap);
        }

        // Period markers — same snapshot may already be included from its day.
        IncludeSnapshot(picked, sorted[0]);
        IncludeSnapshot(picked, sorted[^1]);

        return SortPicked(picked);
    }

    private static List<StashSnapshot> SampleAnchored(
        List<StashSnapshot> sorted, int maxCount, ChartAnchorFlags anchors
    ){
        if(sorted.Count <= maxCount) return [..sorted];

        var required = CollectRequiredIndices(sorted, anchors);
        var selected = new SortedSet<int>(required);

        if(selected.Count >= maxCount) return IndicesToSnapshots(sorted, selected);

        for(var slot = 0; selected.Count < maxCount; slot++){
            var idx = maxCount <= 1 ? 0 : (int) Math.Round(slot * (sorted.Count - 1) / (double) (maxCount - 1));

            selected.Add(Math.Clamp(idx, 0, sorted.Count - 1));

            if(slot > sorted.Count * 2) break;
        }

        for(var i = 0; selected.Count < maxCount && i < sorted.Count; i++) selected.Add(i);

        return IndicesToSnapshots(sorted, selected);
    }

    private static HashSet<int> CollectRequiredIndices(List<StashSnapshot> sorted, ChartAnchorFlags anchors){
        var indices = new HashSet<int>();

        if(anchors.HasFlag(ChartAnchorFlags.First)) indices.Add(0);

        if(anchors.HasFlag(ChartAnchorFlags.Last)) indices.Add(sorted.Count - 1);

        if(anchors.HasFlag(ChartAnchorFlags.MinWorth)) indices.Add(IndexOfMinWorth(sorted));

        if(anchors.HasFlag(ChartAnchorFlags.MaxWorth)) indices.Add(IndexOfMaxWorth(sorted));

        return indices;
    }

    private static List<StashSnapshot> IndicesToSnapshots(List<StashSnapshot> sorted, SortedSet<int> indices){
        var result = new List<StashSnapshot>(indices.Count);

        foreach(var index in indices) result.Add(sorted[index]);

        return result;
    }

    private static Dictionary<string, StashSnapshot> CreateSnapshotMap(){
        return new Dictionary<string, StashSnapshot>(StringComparer.Ordinal);
    }

    private static void IncludeSnapshot(Dictionary<string, StashSnapshot> picked, StashSnapshot snapshot){
        if(snapshot == null || string.IsNullOrEmpty(snapshot.Utc)) return;

        picked[snapshot.Utc] = snapshot;
    }

    private static List<StashSnapshot> SortPicked(Dictionary<string, StashSnapshot> picked){
        var list = picked.Values.ToList();
        list.Sort((a, b) => string.Compare(a.Utc, b.Utc, StringComparison.Ordinal));

        return list;
    }

    private static Dictionary<DateTime, List<StashSnapshot>> GroupByLocalDay(List<StashSnapshot> sorted){
        var byDay = new Dictionary<DateTime, List<StashSnapshot>>();

        foreach(var snapshot in sorted){
            if(!TryParseUtc(snapshot.Utc, out var utc)) continue;

            var day = utc.ToLocalTime().
                          Date;

            if(!byDay.TryGetValue(day, out var list)){
                list       = [];
                byDay[day] = list;
            }

            list.Add(snapshot);
        }

        foreach(var list in byDay.Values) list.Sort((a, b) => string.Compare(a.Utc, b.Utc, StringComparison.Ordinal));

        return byDay;
    }

    private static int IndexOfMinWorth(List<StashSnapshot> sorted){
        var bestIndex = 0;
        var bestWorth = sorted[0].TotalWorth;

        for(var i = 1; i < sorted.Count; i++){
            if(sorted[i].TotalWorth >= bestWorth) continue;

            bestWorth = sorted[i].TotalWorth;
            bestIndex = i;
        }

        return bestIndex;
    }

    private static int IndexOfMaxWorth(List<StashSnapshot> sorted){
        var bestIndex = 0;
        var bestWorth = sorted[0].TotalWorth;

        for(var i = 1; i < sorted.Count; i++){
            if(sorted[i].TotalWorth <= bestWorth) continue;

            bestWorth = sorted[i].TotalWorth;
            bestIndex = i;
        }

        return bestIndex;
    }

    private static List<ChartPoint> ToChartPoints(List<StashSnapshot> snapshots){
        var   points        = new List<ChartPoint>(snapshots.Count);
        long? previousWorth = null;

        foreach(var snapshot in snapshots){
            points.Add(ChartPoint.FromSnapshot(snapshot, previousWorth));
            previousWorth = snapshot.TotalWorth;
        }

        return points;
    }

    private static List<StashSnapshot> CopySorted(IReadOnlyList<StashSnapshot> snapshots){
        var byUtc  = new Dictionary<string, StashSnapshot>(StringComparer.Ordinal);
        var sorted = new List<StashSnapshot>(snapshots.Count);

        foreach(var snapshot in snapshots){
            if(snapshot == null || string.IsNullOrEmpty(snapshot.Utc) || !TryParseUtc(snapshot.Utc, out _)) continue;

            byUtc[snapshot.Utc] = snapshot;
        }

        sorted.AddRange(byUtc.Values);
        sorted.Sort((a, b) => string.Compare(a.Utc, b.Utc, StringComparison.Ordinal));

        return sorted;
    }

    private static bool TryParseUtc(string value, out DateTime utc){
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out utc);
    }
}
