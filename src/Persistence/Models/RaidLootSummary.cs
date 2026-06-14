using System.Collections.Generic;

namespace Softwyx.CareerLog.Persistence.Models;

/// <summary>Raid loot aggregates.</summary>
internal sealed class RaidLootSummary{
    public List<RaidLootItemEntry> TopLooted{
        get;
        set;
    } = [];

    public int ItemsTaken{
        get;
        set;
    }

    public long ValueTakenRub{
        get;
        set;
    }

    public int ItemsLost{
        get;
        set;
    }

    public long ValueLostRub{
        get;
        set;
    }

    public long LoadoutValueStartRub{
        get;
        set;
    }

    public long LoadoutValueEndRub{
        get;
        set;
    }

    /// <summary>Bring-in gear fully gone at extract (consumed, dropped, left behind, ...).</summary>
    public int BringInItemsLost{
        get;
        set;
    }

    /// <summary>Est. handbook value of bring-in gear lost during the raid (includes partial and stack use).</summary>
    public long BringInValueLostRub{
        get;
        set;
    }
}
