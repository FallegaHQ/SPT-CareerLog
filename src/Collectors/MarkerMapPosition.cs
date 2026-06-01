using EFT;
using Softwyx.CareerLog.Map.Data;

namespace Softwyx.CareerLog.Collectors;

internal static class MarkerMapPosition{
    public static float[] FromPlayer(Player player){
        if(player == null)
            return[
                      0f, 0f
                  ];

        var map = CoordinateConverter.RoundForPersist(CoordinateConverter.GameWorldToMap(player.Position));

        return[
                  map.x, map.y
              ];
    }
}
