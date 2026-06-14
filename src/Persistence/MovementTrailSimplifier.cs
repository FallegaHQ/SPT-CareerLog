using System.Collections.Generic;
using Softwyx.CareerLog.Persistence.Models;
using UnityEngine;

namespace Softwyx.CareerLog.Persistence;

/// <summary>Douglas–Peucker simplification on movement values; typed markers are always kept.</summary>
internal static class MovementTrailSimplifier{
    public static List<RaidMovementValue> Simplify(List<RaidMovementValue> values, float epsilonMeters){
        if(values == null || values.Count == 0 || epsilonMeters <= 0f || values.Count < 3) return values;

        var points = new List<Vector2>(values.Count);
        var keep   = new bool[values.Count];

        for(var i = 0; i < values.Count; i++){
            points.Add(ToPoint(values[i]));
            keep[i] = values[i].IsMarker;
        }

        keep[0]                = true;
        keep[values.Count - 1] = true;

        SimplifySegment(points, 0, values.Count - 1, epsilonMeters, keep);

        var result = new List<RaidMovementValue>();

        for(var i = 0; i < values.Count; i++)
            if(keep[i])
                result.Add(values[i]);

        return result;
    }

    private static void SimplifySegment(List<Vector2> points, int start, int end, float epsilon, bool[] keep){
        while(true){
            if(end <= start + 1) return;

            var maxDistance = 0f;
            var index       = 0;

            for(var i = start + 1; i < end; i++){
                if(keep[i]) continue;

                var distance = PerpendicularDistance(points[i], points[start], points[end]);

                if(distance <= maxDistance) continue;

                maxDistance = distance;
                index       = i;
            }

            if(maxDistance < epsilon) return;

            keep[index] = true;
            SimplifySegment(points, start, index, epsilon, keep);
            start = index;
        }
    }

    private static float PerpendicularDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd){
        var dx  = lineEnd.x - lineStart.x;
        var dz  = lineEnd.y - lineStart.y;
        var mag = Mathf.Sqrt(dx * dx + dz * dz);

        if(mag < Mathf.Epsilon) return Vector2.Distance(point, lineStart);

        return Mathf.Abs(dz * point.x - dx * point.y + lineEnd.x * lineStart.y - lineEnd.y * lineStart.x) / mag;
    }

    private static Vector2 ToPoint(RaidMovementValue value){
        if(value?.Point == null || value.Point.Length < 2) return Vector2.zero;

        return new Vector2(value.Point[0], value.Point[1]);
    }
}
