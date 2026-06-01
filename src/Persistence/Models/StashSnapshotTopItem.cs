namespace Softwyx.CareerLog.Persistence.Models;

internal sealed class StashSnapshotTopItem{
    public string TemplateId{
        get;
        set;
    }

    public string Name{
        get;
        set;
    }

    public long ValueRub{
        get;
        set;
    }

    public int Count{
        get;
        set;
    } = 1;
}
