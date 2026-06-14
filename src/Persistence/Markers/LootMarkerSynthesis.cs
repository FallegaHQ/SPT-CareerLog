using System;
using System.Collections.Generic;
using System.Linq;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Persistence.Models;
using UnityEngine;

namespace Softwyx.CareerLog.Persistence.Markers;

internal static class LootMarkerSynthesis{
    public static List<RaidMovementValue> Apply(IReadOnlyList<RaidMovementValue> markers, string locationId){
        if(markers == null || markers.Count == 0) return [];

        var loot = markers.Where(
                                 m => m is{
                                              Type: RaidMarkerTypes.Loot
                                          }
                                ).
                           OrderBy(m => m.UtcOffsetSec).
                           ToList();

        if(loot.Count < 2 || !LocationRegistry.TryGet(locationId, out var def) || def == null) return markers.ToList();

        var diag = MapDiagonal(def);

        if(diag <= 0.01f) return markers.ToList();

        var maxGap   = Settings.LootMarkerMergeMaxGapSec.Value;
        var fraction = Settings.LootMarkerMergeDistanceFraction.Value;
        var maxDist  = diag * Mathf.Clamp01(fraction);

        var consumed = new HashSet<RaidMovementValue>();
        var merged   = new List<RaidMovementValue>();

        var i = 0;

        while(i < loot.Count){
            var run = new List<RaidMovementValue>{
                                                     loot[i]
                                                 };
            var j = i + 1;

            while(j < loot.Count){
                var prev = run[^1];
                var next = loot[j];

                if(next.UtcOffsetSec - prev.UtcOffsetSec > maxGap) break;
                if(Distance(prev, next)                  > maxDist) break;

                run.Add(next);
                j++;
            }

            if(run.Count >= 2){
                foreach(var r in run) consumed.Add(r);
                merged.Add(MergeRun(run));
            }

            i = j;
        }

        if(consumed.Count == 0) return markers.ToList();

        var output = new List<RaidMovementValue>();

        foreach(var m in markers){
            if(m is{
                       Type: RaidMarkerTypes.Loot
                   }
            && consumed.Contains(m))
                continue;

            output.Add(m);
        }

        output.AddRange(merged);

        return output;
    }

    private static RaidMovementValue MergeRun(List<RaidMovementValue> run){
        var sumX     = 0f;
        var sumZ     = 0f;
        var sumValue = 0L;
        var itemIds  = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var tplIds   = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach(var m in run){
            if(m?.Point == null || m.Point.Length < 2) continue;

            sumX     += m.Point[0];
            sumZ     += m.Point[1];
            sumValue += m.ValueRub ?? 0L;

            if(!string.IsNullOrEmpty(m.ItemTemplateId)) tplIds.Add(m.ItemTemplateId);
            if(m.ItemTemplateIds is{
                                       Count: > 0
                                   })
                foreach(var tpl in m.ItemTemplateIds)
                    if(!string.IsNullOrEmpty(tpl))
                        tplIds.Add(tpl);

            if(m.ItemIds is not{
                                   Count: > 0
                               })
                continue;

            foreach(var id in m.ItemIds)
                if(!string.IsNullOrEmpty(id))
                    itemIds.Add(id);
        }

        var count = Math.Max(1, run.Count);

        return new RaidMovementValue{
                                        Point =[
                                                   Mathf.Round(sumX / count * 100f) / 100f,
                                                   Mathf.Round(sumZ / count * 100f) / 100f
                                               ],
                                        Type            = RaidMarkerTypes.Loot,
                                        UtcOffsetSec    = run[0].UtcOffsetSec,
                                        ValueRub        = sumValue,
                                        ItemIds         = itemIds.Count > 0 ? itemIds.ToList() : null,
                                        ItemTemplateIds = tplIds.Count  > 0 ? tplIds.ToList() : null
                                    };
    }

    private static float Distance(RaidMovementValue a, RaidMovementValue b){
        if(a?.Point == null || b?.Point == null || a.Point.Length < 2 || b.Point.Length < 2) return float.MaxValue;

        return Vector2.Distance(new Vector2(a.Point[0], a.Point[1]), new Vector2(b.Point[0], b.Point[1]));
    }

    private static float MapDiagonal(LocationDefinition def){
        var w = Mathf.Abs(def.BoundsMaxX - def.BoundsMinX);
        var h = Mathf.Abs(def.BoundsMaxY - def.BoundsMinY);

        return Mathf.Sqrt(w * w + h * h);
    }
}
