using System.Collections.Generic;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Collectors.Stash;

internal sealed class WorthResult{
    public long                       EquippedValue;
    public long                       InventoryValue;
    public int                        ItemCount;
    public long                       ItemsValue;
    public int                        OccupiedSlotCount;
    public long                       Rubles;
    public List<StashSnapshotTopItem> TopItems = [];
    public long                       TotalWorth;
}
