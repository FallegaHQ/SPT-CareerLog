using System.Collections.Generic;
using Newtonsoft.Json;

namespace Softwyx.CareerLog.Persistence.Models;

/// <summary>
/// One map-space sample on the raid timeline -- a trail point and/or a typed marker
/// (<see cref="RaidMarkerTypes"/>).
/// </summary>
internal sealed class RaidMovementValue{
    /// <summary>Map-space position as <c>[x, z]</c>.</summary>
    public float[] Point{
        get;
        set;
    }

    /// <summary>Marker kind when set.</summary>
    public string Type{
        get;
        set;
    }

    /// <summary>Seconds since raid start.</summary>
    public float UtcOffsetSec{
        get;
        set;
    }

    public bool? LongestKill{
        get;
        set;
    }

    public float? KillDistance{
        get;
        set;
    }

    public int? KillCount{
        get;
        set;
    }

    public string WeaponTemplateId{
        get;
        set;
    }

    public string Label{
        get;
        set;
    }

    public string ItemTemplateId{
        get;
        set;
    }

    /// <summary>Template ids when this marker represents multiple items (e.g., merged loot marker).</summary>
    public List<string> ItemTemplateIds{
        get;
        set;
    }

    /// <summary>Item instance ids when this marker represents multiple items (e.g., merged loot marker).</summary>
    public List<string> ItemIds{
        get;
        set;
    }

    public long? ValueRub{
        get;
        set;
    }

    public string HealKind{
        get;
        set;
    }

    public float? HealAmount{
        get;
        set;
    }

    public List<string> BodyParts{
        get;
        set;
    }

    public List<RaidMarkerVictimEntry> Victims{
        get;
        set;
    }

    [JsonIgnore] public bool IsMarker => !string.IsNullOrEmpty(Type);
}
