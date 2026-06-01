using System.Collections.Generic;

namespace Softwyx.CareerLog.Persistence.Models;

internal sealed class StashSnapshotDayFile{
    public int SchemaVersion{
        get;
        set;
    } = 1;

    public string Day{
        get;
        set;
    }

    public List<StashSnapshot> Snapshots{
        get;
        set;
    } = [];
}
