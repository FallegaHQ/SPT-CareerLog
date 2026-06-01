using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal readonly struct SnapshotSummaryView(StashSnapshot latest, long firstTotalWorth, bool hasSnapshots){
    public static SnapshotSummaryView Empty => default;

    public StashSnapshot Latest{
        get;
    } = latest;

    public long FirstTotalWorth{
        get;
    } = firstTotalWorth;

    public bool HasSnapshots{
        get;
    } = hasSnapshots;
}
