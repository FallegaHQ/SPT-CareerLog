using System.Collections.Generic;

namespace Softwyx.CareerLog.Persistence.Models;

internal sealed class RaidsIndexFile{
    public int SchemaVersion{
        get;
        set;
    } = 1;

    public List<RaidIndexEntry> Raids{
        get;
        set;
    } = [];
}
