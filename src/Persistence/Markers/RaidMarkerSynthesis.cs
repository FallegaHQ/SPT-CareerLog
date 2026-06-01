using Softwyx.CareerLog.Persistence.Models;
using System.Collections.Generic;
using System.Linq;

namespace Softwyx.CareerLog.Persistence.Markers;

internal static class RaidMarkerSynthesis{
    public static List<RaidMovementValue> Apply(IReadOnlyList<RaidMovementValue> raw, string locationId){
        if(raw == null || raw.Count == 0) return [];

        var markers = KillStreakSynthesis.Apply(raw);
        markers = LootMarkerSynthesis.Apply(markers, locationId);
        markers = InjuryMarkerSynthesis.Apply(markers);
        LongestKillFlag.Apply(markers);

        return markers.OrderBy(m => m.UtcOffsetSec).
                       ToList();
    }
}
