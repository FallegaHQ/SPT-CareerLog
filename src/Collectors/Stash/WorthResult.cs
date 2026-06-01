using Softwyx.CareerLog.Persistence.Models;
using System.Collections.Generic;

namespace Softwyx.CareerLog.Collectors.Stash;

internal sealed class WorthResult{
    public long                       Rubles;
    public long                       ItemsValue;
    public long                       EquippedValue;
    public long                       InventoryValue;
    public long                       TotalWorth;
    public int                        ItemCount;
    public int                        OccupiedSlotCount;
    public List<StashSnapshotTopItem> TopItems = [];
}
