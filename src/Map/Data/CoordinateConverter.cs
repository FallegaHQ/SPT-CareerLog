using Softwyx.CareerLog.Persistence.Models;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Data;

internal static class CoordinateConverter{
    private const int PersistDecimals = 4;

    private static Vector2 GameWorldToMap(float gameX, float gameZ){
        return new Vector2(gameX, gameZ);
    }

    public static Vector2 GameWorldToMap(Vector3 unityPosition){
        return GameWorldToMap(unityPosition.x, unityPosition.z);
    }

    public static Vector2 RoundForPersist(Vector2 mapPoint){
        return new Vector2(RoundComponent(mapPoint.x), RoundComponent(mapPoint.y));
    }

    private static float RoundComponent(float value){
        var mul = Mathf.Pow(10f, PersistDecimals);

        return Mathf.Round(value * mul) / mul;
    }

    public static Vector2 MovementToMapPoint(RaidMovementValue value){
        if(value?.Point == null || value.Point.Length < 2) return Vector2.zero;

        return new Vector2(value.Point[0], value.Point[1]);
    }
}
