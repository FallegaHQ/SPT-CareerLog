using System.Collections.Generic;
using Softwyx.CareerLog.Map.Markers;
using Softwyx.CareerLog.Persistence.Models;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Data;

/// <summary>Single-pass view of <see cref="RaidMovement.Values" /> for map trail + markers.</summary>
internal sealed class RaidMovementIndex{
    private readonly List<RaidMovementValue> _markers     = [];
    private readonly List<Vector2>           _trailPoints = [];
    private readonly List<float>             _trailTimes  = [];
    private          float                   _recordedDurationSec;

    public  IReadOnlyList<Vector2> TrailPoints      => _trailPoints;
    private float                  LastTrailTimeSec => _trailTimes.Count > 0 ? _trailTimes[^1] : 0f;

    public float DurationSec{
        get{
            var max = 0f;

            foreach(var t in _trailTimes)
                if(t > max)
                    max = t;

            foreach(var marker in _markers)
                if(marker != null && marker.UtcOffsetSec > max)
                    max = marker.UtcOffsetSec;

            if(_recordedDurationSec > max) max = _recordedDurationSec;

            return max;
        }
    }

    public static RaidMovementIndex Build(RaidMovement movement, float recordedDurationSec = 0f){
        var index = new RaidMovementIndex{
                                             _recordedDurationSec = recordedDurationSec
                                         };
        index.Fill(movement);

        return index;
    }

    /// <summary>Committed prefix + tip interpolated on the active segment at raid time.</summary>
    public RaidTrailPlaybackState TrailStateAtTime(float seconds){
        if(_trailPoints.Count == 0 || _trailTimes.Count != _trailPoints.Count || seconds <= 0f)
            return RaidTrailPlaybackState.Empty;

        var duration = DurationSec;

        if(duration <= 0f) return RaidTrailPlaybackState.Empty;

        // Map wall-clock playback to sample timestamps when raid duration extends past last movement sample.
        var trailSeconds = seconds;

        if(LastTrailTimeSec > 0f && duration > LastTrailTimeSec + 0.01f)
            trailSeconds = seconds * (LastTrailTimeSec / duration);

        if(trailSeconds >= LastTrailTimeSec)
            return new RaidTrailPlaybackState(_trailPoints.Count - 1, _trailPoints[^1]);

        if(trailSeconds < _trailTimes[0]) return RaidTrailPlaybackState.Empty;

        var headIdx = BinarySearchFloor(trailSeconds);

        if(headIdx + 1 >= _trailPoints.Count) return new RaidTrailPlaybackState(headIdx, _trailPoints[headIdx]);

        var t   = Mathf.InverseLerp(_trailTimes[headIdx], _trailTimes[headIdx + 1], trailSeconds);
        var tip = Vector2.Lerp(_trailPoints[headIdx], _trailPoints[headIdx    + 1], t);

        return new RaidTrailPlaybackState(headIdx, tip);
    }

    public List<RaidMovementValue> MarkersVisibleAt(float seconds, Visibility visibility){
        var result = new List<RaidMovementValue>();

        foreach(var marker in _markers){
            if(marker              == null) continue;
            if(marker.UtcOffsetSec > seconds) continue;
            if(visibility != null && !visibility.IsVisible(marker)) continue;

            if(!IconLoader.TryGetForValue(marker, out _)) continue;

            result.Add(marker);
        }

        return result;
    }

    private int BinarySearchFloor(float seconds){
        if(_trailTimes.Count <= 1) return 0;

        var lo = 0;
        var hi = _trailTimes.Count - 2;

        while(lo < hi){
            var mid = (lo + hi + 1) >> 1;

            if(_trailTimes[mid] <= seconds)
                lo = mid;
            else
                hi = mid - 1;
        }

        return lo;
    }

    private void Fill(RaidMovement movement){
        _trailPoints.Clear();
        _trailTimes.Clear();
        _markers.Clear();

        if(movement?.Values == null) return;

        foreach(var value in movement.Values){
            if(value == null) continue;

            if(value.IsMarker){
                _markers.Add(value);

                continue;
            }

            _trailPoints.Add(CoordinateConverter.MovementToMapPoint(value));
            _trailTimes.Add(value.UtcOffsetSec);
        }

        EnsureSpawnMarker();
    }

    private void EnsureSpawnMarker(){
        foreach(var marker in _markers)
            if(marker is{
                            Type: RaidMarkerTypes.Spawn
                        })
                return;

        if(_trailPoints.Count == 0) return;

        var spawn = _trailPoints[0];
        _markers.Insert(
                        0,
                        new RaidMovementValue{
                                                 Point =[
                                                            spawn.x, spawn.y
                                                        ],
                                                 Type         = RaidMarkerTypes.Spawn,
                                                 UtcOffsetSec = 0f
                                             }
                       );
    }
}
