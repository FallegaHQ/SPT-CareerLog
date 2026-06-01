using Comfort.Common;
using EFT;

namespace Softwyx.CareerLog.Patches;

internal static class CollectorProfileGuard{
    internal static bool IsLocalStatisticsProfile(Profile profile){
        if(profile == null) return false;

        var world = Singleton<GameWorld>.Instance;
        var main  = world?.MainPlayer;

        return main && main.IsYourPlayer && profile.Id == main.Profile.Id;
    }
}
