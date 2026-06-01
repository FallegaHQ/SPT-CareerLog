namespace Softwyx.CareerLog.Persistence.Models;

/// <summary>One victim in a kill or killstreak marker.</summary>
internal sealed class RaidMarkerVictimEntry{
    /// <summary>Victim position in map space <c>[x, z]</c>.</summary>
    public float[] Point{
        get;
        set;
    }

    public string Name{
        get;
        set;
    }

    public string Side{
        get;
        set;
    }

    public string WeaponTemplateId{
        get;
        set;
    }

    public string BodyPart{
        get;
        set;
    }

    public float? Distance{
        get;
        set;
    }

    public bool Boss{
        get;
        set;
    }
}
