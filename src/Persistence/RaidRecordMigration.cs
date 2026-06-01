using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

/// <summary>Default missing fields after JSON load.</summary>
internal static class RaidRecordMigration{
    public const int CurrentSchemaVersion = 1;

    public static RaidRecord ApplyOnLoad(RaidRecord raid){
        if(raid == null) return null;

        if(raid.SchemaVersion <= 0) raid.SchemaVersion = CurrentSchemaVersion;

        raid.VanillaSessionCountersLong  ??= new System.Collections.Generic.Dictionary<string, long>();
        raid.VanillaSessionCountersFloat ??= new System.Collections.Generic.Dictionary<string, float>();

        if(raid.Movement != null) raid.Movement.Values ??= [];

        raid.Loot ??= new RaidLootSummary();

        return raid;
    }
}
