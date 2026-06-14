using System.Collections.Generic;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence.Markers;

internal static class LongestKillFlag{
    public static void Apply(IReadOnlyList<RaidMovementValue> markers){
        if(markers == null) return;

        RaidMovementValue best     = null;
        var               bestDist = float.MinValue;

        foreach(var marker in markers){
            if(!IsKillFamily(marker)) continue;

            var dist = marker.KillDistance ?? 0f;

            if(dist <= bestDist) continue;

            bestDist = dist;
            best     = marker;
        }

        if(best == null) return;

        foreach(var marker in markers)
            if(IsKillFamily(marker))
                marker.LongestKill = marker == best;
    }

    private static bool IsKillFamily(RaidMovementValue marker){
        return marker?.Type is RaidMarkerTypes.Kill or RaidMarkerTypes.BossKill or RaidMarkerTypes.Killstreak;
    }
}
