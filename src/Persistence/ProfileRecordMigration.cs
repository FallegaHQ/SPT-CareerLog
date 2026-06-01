using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal static class ProfileRecordMigration{
    public static ProfileRecord ApplyOnLoad(ProfileRecord record){
        if(record == null) return null;

        if(record.SchemaVersion <= 0) record.SchemaVersion = 1;

        record.Lifetime ??= new LifetimeStats();

        return record;
    }
}
