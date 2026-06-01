using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence.Financial;

internal readonly struct ChartPoint(long totalWorth, StashSnapshot snapshot){
    public long TotalWorth{
        get;
    } = totalWorth;

    public StashSnapshot Snapshot{
        get;
    } = snapshot;
}
