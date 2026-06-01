using System.Collections.Generic;

namespace Softwyx.CareerLog.Persistence.Models;

internal sealed class StashSnapshotIndex{
    public int SchemaVersion{
        get;
        set;
    } = 1;

    public string ProfileId{
        get;
        set;
    }

    public string FirstUtc{
        get;
        set;
    }

    public long FirstTotalWorth{
        get;
        set;
    }

    public string LatestUtc{
        get;
        set;
    }

    public StashSnapshot Latest{
        get;
        set;
    }

    public List<StashSnapshotIndexDay> Days{
        get;
        set;
    } = [];
}
