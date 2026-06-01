using EFT.HealthSystem;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Collectors.Health;

internal static class InjuryMarkerCollector{
    public static void Record(ActiveHealthController controller, EBodyPart bodyPart){
        if(controller?.Player == null || !controller.Player.IsYourPlayer) return;

        RaidEventMarkerBuffer.Add(
                                  new RaidMovementValue{
                                                           Point = MarkerMapPosition.FromPlayer(controller.Player),
                                                           Type = RaidMarkerTypes.Injury,
                                                           UtcOffsetSec = RaidEventClock.ElapsedSeconds(),
                                                           BodyParts =[
                                                                          bodyPart.ToString()
                                                                      ]
                                                       }
                                 );
    }
}
