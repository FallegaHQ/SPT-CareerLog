using EFT;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Collectors.Health;

internal static class HealingMarkerCollector{
    // Needs more work...
    private const string KindLargeRestore = "largeRestore";
    public const  string KindSurgery      = "surgery";

    public static void RecordLargeRestore(Player player, float amount){
        if(player == null || !player.IsYourPlayer || amount <= 0f) return;

        Add(player, KindLargeRestore, amount);
    }

    public static void RecordSurgery(Player player){
        if(player == null || !player.IsYourPlayer) return;

        Add(player, KindSurgery, 0f);
    }

    private static void Add(Player player, string kind, float amount){
        RaidEventMarkerBuffer.Add(
                                  new RaidMovementValue{
                                                           Point        = MarkerMapPosition.FromPlayer(player),
                                                           Type         = RaidMarkerTypes.Healing,
                                                           UtcOffsetSec = RaidEventClock.ElapsedSeconds(),
                                                           HealKind     = kind,
                                                           HealAmount   = amount
                                                       }
                                 );
    }
}
