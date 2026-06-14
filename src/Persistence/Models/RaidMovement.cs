using System.Collections.Generic;

namespace Softwyx.CareerLog.Persistence.Models;

/// <summary>Player movement timeline for one raid (trail samples and markers in order).</summary>
internal sealed class RaidMovement{
    /// <summary>
    ///     Seconds between in-raid position samples (see movement sampler config).
    ///     Kept here in case the player changes the main config.
    /// </summary>
    public float SampleIntervalSec{
        get;
        set;
    }

    /// <summary>Ordered trail samples and marker events (see <see cref="RaidMovementValue" />).</summary>
    public List<RaidMovementValue> Values{
        get;
        set;
    } = [];
}
