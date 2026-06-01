using System;

namespace Softwyx.CareerLog.Collectors;

/// <summary>Seconds elapsed since the current raid session started.</summary>
internal static class RaidEventClock{
    private static DateTime _startedUtc;
    private static bool     _active;

    public static void Start(){
        _startedUtc = DateTime.UtcNow;
        _active     = true;
    }

    public static void Stop(){
        _active = false;
    }

    public static float ElapsedSeconds(){
        if(!_active) return 0f;

        return (float) (DateTime.UtcNow - _startedUtc).TotalSeconds;
    }
}
