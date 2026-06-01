using EFT.Interactive;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Collectors.Doors;

internal static class DoorUnlockMarkerCollector{
    public static void Record(WorldInteractiveObject door){
        var player = LocalRaidPlayer.Instance;

        if(player == null || door == null) return;

        RaidEventMarkerBuffer.Add(
                                  new RaidMovementValue{
                                                           Point        = MarkerMapPosition.FromPlayer(player),
                                                           Type         = RaidMarkerTypes.DoorUnlock,
                                                           UtcOffsetSec = RaidEventClock.ElapsedSeconds(),
                                                           Label        = door.Id,
                                                           ItemTemplateId = string.IsNullOrEmpty(door.KeyId)
                                                                                ? null
                                                                                : door.KeyId
                                                       }
                                 );
    }
}
