using System.Collections.Generic;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Collectors;

/// <summary>In-raid event markers before save-time synthesis and trail merge.</summary>
internal static class RaidEventMarkerBuffer{
    private static readonly List<RaidMovementValue> Markers = [];

    public static void Clear(){
        Markers.Clear();
    }

    public static void Add(RaidMovementValue marker){
        if(marker == null) return;

        Markers.Add(marker);
    }

    public static List<RaidMovementValue> TakeSnapshot(){
        return[
                  ..Markers
              ];
    }
}
