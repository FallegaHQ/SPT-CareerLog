using System.Collections.Generic;
using System.Linq;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence.Markers;

internal static class KillStreakSynthesis{
    public static List<RaidMovementValue> Apply(IReadOnlyList<RaidMovementValue> markers){
        if(markers == null || markers.Count == 0) return [];

        var window   = Settings.KillstreakMaxGapSec.Value;
        var minKills = Settings.KillstreakMinKills.Value;
        var kills = markers.Where(
                                  m => m is{
                                               Type: RaidMarkerTypes.Kill
                                           }
                                 ).
                            OrderBy(m => m.UtcOffsetSec).
                            ToList();

        var consumed = new HashSet<RaidMovementValue>();
        var output   = new List<RaidMovementValue>();

        var index = 0;

        while(index < kills.Count){
            var run = new List<RaidMovementValue>{
                                                     kills[index]
                                                 };
            var cursor = index + 1;

            while(cursor < kills.Count){
                var gap = kills[cursor].UtcOffsetSec - run[^1].UtcOffsetSec;

                if(gap > window) break;

                run.Add(kills[cursor]);
                cursor++;
            }

            if(run.Count >= minKills){
                foreach(var kill in run) consumed.Add(kill);

                output.Add(BuildStreak(run));
            }

            index = cursor;
        }

        foreach(var marker in markers){
            if(marker is{
                            Type: RaidMarkerTypes.Kill
                        }
            && consumed.Contains(marker))
                continue;

            output.Add(marker);
        }

        return output;
    }

    private static RaidMovementValue BuildStreak(List<RaidMovementValue> run){
        var victims = new List<RaidMarkerVictimEntry>();

        foreach(var kill in run){
            if(kill.Victims == null) continue;

            victims.AddRange(kill.Victims);
        }

        var last  = run[^1];
        var count = run.Count;

        return new RaidMovementValue{
                                        Point = last.Point is{
                                                                 Length: >= 2
                                                             }
                                                    ?[
                                                         last.Point[0], last.Point[1]
                                                     ]
                                                    : null,
                                        Type         = RaidMarkerTypes.Killstreak,
                                        UtcOffsetSec = last.UtcOffsetSec,
                                        KillCount    = count,
                                        Victims      = victims,
                                        KillDistance = run.Max(k => k.KillDistance ?? 0f)
                                    };
    }
}
