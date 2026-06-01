using UnityEngine;

namespace Softwyx.CareerLog.Collectors.Movement;

internal readonly struct MovementSample(Vector2 position, float elapsedSec){
    public Vector2 Position   => position;
    public float   ElapsedSec => elapsedSec;
}
