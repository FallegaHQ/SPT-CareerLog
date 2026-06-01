using System.Collections.Generic;
using UnityEngine;

namespace Softwyx.CareerLog.Collectors.Movement;

internal static class MovementSampleBuffer{
    private static readonly List<MovementSample> Samples = [];

    public static void Clear(){
        Samples.Clear();
    }

    public static void Add(Vector3 unityPosition, float elapsedSec){
        Samples.Add(new MovementSample(new Vector2(unityPosition.x, unityPosition.z), elapsedSec));
    }

    public static List<MovementSample> TakeAll(){
        var copy = new List<MovementSample>(Samples);
        Clear();

        return copy;
    }
}
