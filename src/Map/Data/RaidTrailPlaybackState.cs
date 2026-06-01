using UnityEngine;

namespace Softwyx.CareerLog.Map.Data;

/// <summary>Committed trail prefix plus interpolated tip at raid time. Grants higher fidelity trail.</summary>
internal readonly struct RaidTrailPlaybackState(int headIndex, Vector2 tip){
    public static readonly RaidTrailPlaybackState Empty = new(-1, Vector2.zero);

    public int HeadIndex{
        get;
    } = headIndex;
    public Vector2 Tip{
        get;
    } = tip;

    public bool IsEmpty => HeadIndex < 0;

    public int CommittedSegmentEnd => HeadIndex < 0 ? 0 : HeadIndex + 1;
}
