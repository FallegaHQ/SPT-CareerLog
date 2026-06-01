using System.Collections.Generic;
using EFT;

namespace Softwyx.CareerLog.Persistence.Models;

internal sealed class LifetimeStats{
    public int RaidsRecorded{
        get;
        set;
    }

    /// <summary>Full extract with Found in Raid (vanilla <see cref="ExitStatus.Survived"/>).</summary>
    public int RaidsSurvived{
        get;
        set;
    }

    /// <summary>Extracted too quickly / low activity (vanilla <see cref="EFT.ExitStatus.Runner"/>).</summary>
    public int RaidsRunThrough{
        get;
        set;
    }

    public int RaidsKilled{
        get;
        set;
    }

    public int RaidsMissingInAction{
        get;
        set;
    }

    public int RaidsLeft{
        get;
        set;
    }

    public int RaidsTransit{
        get;
        set;
    }

    /// <summary>KIA + MIA + left early (computed on load for older saves).</summary>
    public int RaidsFailed{
        get;
        set;
    }

    public long TotalKills{
        get;
        set;
    }

    public long TotalHeadShots{
        get;
        set;
    }

    /// <summary>Survived / raids recorded (Excluding Run Throughs).</summary>
    public double SurvivalRate{
        get;
        set;
    }

    /// <summary>(Survived + Run Through) / raids recorded.</summary>
    public double ExtractRate{
        get;
        set;
    }

    public string FavoriteLocationId{
        get;
        set;
    }

    public Dictionary<string, int> RaidsByLocation{
        get;
        set;
    } = new();
}
