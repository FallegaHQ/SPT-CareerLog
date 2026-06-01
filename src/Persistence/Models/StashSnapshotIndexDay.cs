namespace Softwyx.CareerLog.Persistence.Models;

internal sealed class StashSnapshotIndexDay{
    public string Day{
        get;
        set;
    }

    public int Count{
        get;
        set;
    }

    public string FirstUtc{
        get;
        set;
    }

    public string LastUtc{
        get;
        set;
    }
}
