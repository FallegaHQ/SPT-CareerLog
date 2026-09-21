using System.Collections.Generic;

namespace Softwyx.CareerLog.Persistence.Models;

internal sealed class RaidRecord{
    /// <summary>See <see cref="RaidRecordMigration.CurrentSchemaVersion" />.</summary>
    public int SchemaVersion{
        get;
        set;
    } = RaidRecordMigration.CurrentSchemaVersion;

    public string RaidId{
        get;
        set;
    }

    public string ProfileId{
        get;
        set;
    }

    /// <summary><see cref="RaidPlayedSideValues.Pmc" /> or <see cref="RaidPlayedSideValues.Scav" />.</summary>
    public string PlayedSide{
        get;
        set;
    }

    public string LocationId{
        get;
        set;
    }

    public string StartedUtc{
        get;
        set;
    }

    public string EndedUtc{
        get;
        set;
    }

    public string ExitStatus{
        get;
        set;
    }

    public float DurationSeconds{
        get;
        set;
    }

    /// <summary>
    ///     Full vanilla session counter snapshot at raid end (see <see cref="VanillaSessionCounterSnapshot" />).
    ///     Keys are <see cref="EFT.Counters.PredefinedCounters" /> field names -- <see cref="VanillaSessionCounterKeys" />.
    ///     Non-float counters (Long / Undefined) are stored here.
    /// </summary>
    public Dictionary<string, long> VanillaSessionCountersLong{
        get;
        set;
    } = new();

    /// <summary>
    ///     Float-typed vanilla session counters (<see cref="EFT.Counters.CounterValueType.Float" />).
    ///     Keys match <see cref="VanillaSessionCounterKeys" />.
    /// </summary>
    public Dictionary<string, float> VanillaSessionCountersFloat{
        get;
        set;
    } = new();

    /// <summary>Movement trail and map markers (see <see cref="RaidMovement" />).</summary>
    public RaidMovement Movement{
        get;
        set;
    }

    /// <summary>Loot and loadout aggregates for this raid (Phase C).</summary>
    public RaidLootSummary Loot{
        get;
        set;
    }
}
