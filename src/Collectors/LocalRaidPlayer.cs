using EFT;

namespace Softwyx.CareerLog.Collectors;

internal static class LocalRaidPlayer{
    public static Player Instance{
        get;
        private set;
    }

    public static void Set(Player player){
        Instance = player != null && player.IsYourPlayer ? player : null;
    }

    public static void Clear(){
        Instance = null;
    }
}
