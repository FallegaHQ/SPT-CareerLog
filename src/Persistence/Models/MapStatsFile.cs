using System.Collections.Generic;

namespace Softwyx.CareerLog.Persistence.Models;

internal sealed class MapStatsFile{
    public int SchemaVersion{
        get;
        set;
    } = 1;

    public Dictionary<string, int> RaidsByLocation{
        get;
        set;
    } = new();

    public string FavoriteLocationId{
        get;
        set;
    }
}
