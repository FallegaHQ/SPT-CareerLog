using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Collectors.Meta;

internal static class AchievementMarkerCollector{
    public static void Record(string achievementId, string title){
        var player = LocalRaidPlayer.Instance;

        if(player == null) return;

        RaidEventMarkerBuffer.Add(
                                  new RaidMovementValue{
                                                           Point          = MarkerMapPosition.FromPlayer(player),
                                                           Type           = RaidMarkerTypes.Achievement,
                                                           UtcOffsetSec   = RaidEventClock.ElapsedSeconds(),
                                                           Label          = title,
                                                           ItemTemplateId = achievementId
                                                       }
                                 );
    }
}
