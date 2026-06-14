using UnityEngine;

namespace Softwyx.CareerLog.Collectors.Movement;

internal sealed class MovementSamplerHost : MonoBehaviour{
    private static MovementSamplerHost _instance;

    private void OnDestroy(){
        if(_instance == this) _instance = null;
    }

    public static MovementSamplerHost Ensure(){
        if(_instance) return _instance;

        var go = new GameObject(nameof(MovementSamplerHost));
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<MovementSamplerHost>();

        return _instance;
    }
}
