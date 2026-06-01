namespace Softwyx.CareerLog.Persistence.Models;

/// <summary>Lightweight raid row for list UI (see <c>raids-index.json</c>).</summary>
internal sealed class RaidIndexEntry{
    public int SchemaVersion{
        get;
        set;
    } = 1;

    public string RaidId{
        get;
        set;
    }

    public string EndedUtc{
        get;
        set;
    }

    public string StartedUtc{
        get;
        set;
    }

    public string LocationId{
        get;
        set;
    }

    public string ExitStatus{
        get;
        set;
    }

    public string PlayedSide{
        get;
        set;
    }

    public float DurationSeconds{
        get;
        set;
    }

    public long Kills{
        get;
        set;
    }

    public long HeadShots{
        get;
        set;
    }
}
