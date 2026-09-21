using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence.Financial;

internal readonly struct ChartPoint(long totalWorth, StashSnapshot snapshot, long? displayDeltaRubles){
    public long TotalWorth{
        get;
    } = totalWorth;

    public StashSnapshot Snapshot{
        get;
    } = snapshot;

    /// <summary>Delta vs previous chart point when set; otherwise UI falls back to <see cref="StashSnapshot.DeltaRubles" />.</summary>
    public long? DisplayDeltaRubles{
        get;
    } = displayDeltaRubles;

    public static ChartPoint FromSnapshot(StashSnapshot snapshot, long? previousWorth){
        if(snapshot == null) return default;

        long? delta = previousWorth.HasValue ? snapshot.TotalWorth - previousWorth.Value : null;

        return new ChartPoint(snapshot.TotalWorth, snapshot, delta);
    }
}
