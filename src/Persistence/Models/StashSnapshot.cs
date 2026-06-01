using System.Collections.Generic;

namespace Softwyx.CareerLog.Persistence.Models;

internal sealed class StashSnapshot{
    public string Utc{
        get;
        set;
    }

    public long Rubles{
        get;
        set;
    }

    public long ItemsValue{
        get;
        set;
    }

    public long EquippedValue{
        get;
        set;
    }

    public long InventoryValue{
        get;
        set;
    }

    public long TotalWorth{
        get;
        set;
    }

    public int ItemCount{
        get;
        set;
    }

    public int OccupiedSlotCount{
        get;
        set;
    }

    public long DeltaRubles{
        get;
        set;
    }

    public List<StashSnapshotTopItem> TopItems{
        get;
        set;
    } = [];

    public List<string> Notes{
        get;
        set;
    } = [];
}
