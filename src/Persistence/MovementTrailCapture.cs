using System.Collections.Generic;
using System.Linq;
using EFT;
using Softwyx.CareerLog.Collectors;
using Softwyx.CareerLog.Collectors.Loot;
using Softwyx.CareerLog.Collectors.Movement;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Persistence.Markers;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal static class MovementTrailCapture{
    public static void Apply(RaidRecord record, Player player, float durationSeconds){
        if(record == null) return;

        var samples = MovementSampler.StopAndTakeSamples();

        if(samples.Count == 0){
            CareerLogPlugin.Log?.LogDebug(PluginInfo.Format("Movement trail empty -- not saved."));

            return;
        }

        var interval  = Settings.MovementSampleIntervalSec.Value;
        var startRub  = record.Loot?.LoadoutValueStartRub ?? 0L;
        var endRub    = record.Loot?.LoadoutValueEndRub   ?? 0L;
        var values    = BuildTimeline(samples, player, durationSeconds, record.ExitStatus, startRub, endRub);
        var rawEvents = RaidEventMarkerBuffer.TakeSnapshot();
        LootExtractInventoryReconcile.PruneLootMarkers(rawEvents);
        var events = RaidMarkerSynthesis.Apply(rawEvents, record.LocationId);
        LootExtractInventoryReconcile.PruneLootMarkers(events);

        values = MovementTimelineMerge(values, events);

        if(Settings.OptimizeMovementTrail.Value)
            values = MovementTrailSimplifier.Simplify(values, Settings.MovementDouglasPeuckerEpsilonMeters.Value);

        ExtendTrailToDuration(values, durationSeconds, player);

        record.Movement = new RaidMovement{
                                              SampleIntervalSec = interval,
                                              Values            = values
                                          };

        if(!LocationRegistry.HasMap(record.LocationId))
            CareerLogPlugin.Log?.LogDebug(
                                          PluginInfo.Format(
                                                            $"Movement saved without bundled map for "
                                                          + $"'{record.LocationId}' ({CountTrail(values)} trail pts)."
                                                           )
                                         );
    }

    private static List<RaidMovementValue> BuildTimeline(
        List<MovementSample> samples, Player player, float durationSeconds, string exitStatus, long startRub,
        long                 endRub
    ){
        var values = new List<RaidMovementValue>(samples.Count + 1);

        foreach(var sample in samples){
            var rounded = CoordinateConverter.RoundForPersist(sample.Position);

            values.Add(
                       new RaidMovementValue{
                                                Point =[
                                                           rounded.x, rounded.y
                                                       ],
                                                UtcOffsetSec = sample.ElapsedSec
                                            }
                      );
        }

        if(values.Count > 0){
            values[0].Type     = RaidMarkerTypes.Spawn;
            values[0].ValueRub = startRub;
        }

        if(player == null) return values;

        var endMap = CoordinateConverter.RoundForPersist(CoordinateConverter.GameWorldToMap(player.Position));

        // Only a true death gets the death marker; other non-extract outcomes are still shown as an end marker.
        var endType = string.Equals(exitStatus, "Killed") ? RaidMarkerTypes.Death : RaidMarkerTypes.Extract;

        values.Add(
                   new RaidMovementValue{
                                            Point =[
                                                       endMap.x, endMap.y
                                                   ],
                                            Type         = endType,
                                            ValueRub     = endRub,
                                            UtcOffsetSec = durationSeconds
                                        }
                  );

        return values;
    }

    private static void ExtendTrailToDuration(List<RaidMovementValue> values, float durationSeconds, Player player){
        if(values == null || values.Count == 0 || durationSeconds <= 0f || player == null) return;

        var lastTrailIndex = -1;

        for(var i = 0; i < values.Count; i++)
            if(values[i] != null && !values[i].IsMarker)
                lastTrailIndex = i;

        if(lastTrailIndex < 0) return;

        if(values[lastTrailIndex].UtcOffsetSec >= durationSeconds - 0.05f) return;

        var endMap = CoordinateConverter.RoundForPersist(CoordinateConverter.GameWorldToMap(player.Position));
        var extension = new RaidMovementValue{
                                                 Point =[
                                                            endMap.x, endMap.y
                                                        ],
                                                 UtcOffsetSec = durationSeconds
                                             };

        var insertAt = values.Count;

        for(var i = 0; i < values.Count; i++){
            if(!values[i].IsMarker || values[i].UtcOffsetSec < durationSeconds - 0.01f) continue;

            insertAt = i;

            break;
        }

        values.Insert(insertAt, extension);
    }

    private static int CountTrail(List<RaidMovementValue> values){
        var count = 0;

        if(values == null) return count;

        foreach(var value in values)
            if(value is{
                           IsMarker: false
                       })
                count++;

        return count;
    }

    private static List<RaidMovementValue> MovementTimelineMerge(
        List<RaidMovementValue> trail, List<RaidMovementValue> eventMarkers
    ){
        if(trail == null || trail.Count == 0) return eventMarkers?.ToList() ?? [];

        if(eventMarkers == null || eventMarkers.Count == 0) return trail;

        var merged = new List<RaidMovementValue>(trail.Count + eventMarkers.Count);
        merged.AddRange(trail);
        merged.AddRange(eventMarkers);

        return merged.OrderBy(v => v.UtcOffsetSec).
                      ToList();
    }
}
