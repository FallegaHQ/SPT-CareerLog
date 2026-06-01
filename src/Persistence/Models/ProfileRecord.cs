namespace Softwyx.CareerLog.Persistence.Models;

internal sealed class ProfileRecord{
    public int SchemaVersion{
        get;
        set;
    } = 1;

    public string ProfileId{
        get;
        set;
    }

    public LifetimeStats Lifetime{
        get;
        set;
    } = new();
}
