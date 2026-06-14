using System.Collections.Generic;
using System.Linq;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Persistence.Models;
using UnityEngine;

namespace Softwyx.CareerLog.Persistence.Markers;

internal static class InjuryMarkerSynthesis{
    public static List<RaidMovementValue> Apply(IReadOnlyList<RaidMovementValue> markers){
        if(markers == null || markers.Count == 0) return [];

        var window = Settings.InjuryMergeWindowSec.Value;
        var injuries = markers.Where(
                                     m => m is{
                                                  Type: RaidMarkerTypes.Injury
                                              }
                                    ).
                               OrderBy(m => m.UtcOffsetSec).
                               ToList();

        if(injuries.Count == 0) return markers.ToList();

        var consumed = new HashSet<RaidMovementValue>();
        var merged   = new List<RaidMovementValue>();
        var index    = 0;

        while(index < injuries.Count){
            var run = new List<RaidMovementValue>{
                                                     injuries[index]
                                                 };
            var cursor = index + 1;

            while(cursor < injuries.Count){
                var gap = injuries[cursor].UtcOffsetSec - run[^1].UtcOffsetSec;

                if(gap > window) break;

                run.Add(injuries[cursor]);
                cursor++;
            }

            if(run.Count > 1){
                foreach(var injury in run) consumed.Add(injury);

                merged.Add(BuildMerged(run));
            }

            index = cursor;
        }

        var output = new List<RaidMovementValue>();

        foreach(var marker in markers){
            if(marker is{
                            Type: RaidMarkerTypes.Injury
                        }
            && consumed.Contains(marker))
                continue;

            output.Add(marker);
        }

        output.AddRange(merged);

        return output;
    }

    private static RaidMovementValue BuildMerged(List<RaidMovementValue> run){
        var parts = new HashSet<string>();
        var sumX  = 0f;
        var sumZ  = 0f;

        foreach(var injury in run){
            sumX += injury.Point[0];
            sumZ += injury.Point[1];

            if(injury.BodyParts == null) continue;

            foreach(var part in injury.BodyParts)
                if(!string.IsNullOrEmpty(part))
                    parts.Add(part);
        }

        var count = run.Count;

        return new RaidMovementValue{
                                        Point =[
                                                   Round(sumX / count), Round(sumZ / count)
                                               ],
                                        Type         = RaidMarkerTypes.Injury,
                                        UtcOffsetSec = run[0].UtcOffsetSec,
                                        BodyParts    = parts.ToList()
                                    };
    }

    private static float Round(float value){
        return Mathf.Round(value * 100f) / 100f;
    }
}
