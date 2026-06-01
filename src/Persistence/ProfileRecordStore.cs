using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal static class ProfileRecordStore{
    private static string DataRoot =>
        Path.Combine(
                     Path.GetDirectoryName(
                                           Assembly.GetExecutingAssembly().
                                                    Location
                                          )
                  ?? string.Empty,
                     "data"
                    );

    public static void SaveRaid(RaidRecord raid, ProfileRecord profileRecord){
        if(raid == null || string.IsNullOrEmpty(raid.ProfileId))
            throw new ArgumentException("Raid record must include a profile id.");

        var profileDir = GetProfileDirectory(raid.ProfileId);
        var raidsDir   = Path.Combine(profileDir, "raids");

        Directory.CreateDirectory(raidsDir);

        var raidJson = PersistenceJson.Serialize(raid);
        var raidPath = Path.Combine(raidsDir, $"{raid.RaidId}.json");
        File.WriteAllText(raidPath, raidJson);

        profileRecord ??= LoadProfileRecord(raid.ProfileId)
                       ?? new ProfileRecord{
                                               ProfileId = raid.ProfileId
                                           };

        profileRecord.ProfileId =   raid.ProfileId;
        profileRecord.Lifetime  ??= new LifetimeStats();

        LifetimeAggregator.ApplyRaid(profileRecord, raid);

        var profilePath = Path.Combine(profileDir, "profile-record.json");
        File.WriteAllText(profilePath, PersistenceJson.Serialize(profileRecord));

        RaidIndexAggregator.ApplyRaidSave(profileDir, raid);
        ProfileRecordsReadApi.Invalidate(raid.ProfileId);
    }

    internal static string GetProfileDataDirectory(string profileId){
        return GetProfileDirectory(profileId);
    }

    public static RaidRecord LoadRaid(string profileId, string raidId){
        if(string.IsNullOrEmpty(profileId) || string.IsNullOrEmpty(raidId)) return null;

        var raidPath = Path.Combine(GetProfileDirectory(profileId), "raids", $"{raidId}.json");

        return !File.Exists(raidPath) ? null : TryDeserializeRaid(File.ReadAllText(raidPath));
    }

    public static IReadOnlyList<RaidIndexEntry> ListRaidIndex(string profileId){
        return string.IsNullOrEmpty(profileId) ? [] : RaidIndexAggregator.ListRaids(GetProfileDirectory(profileId));
    }

    public static ProfileRecord LoadProfileRecord(string profileId){
        if(string.IsNullOrEmpty(profileId)) return null;

        var profilePath = Path.Combine(GetProfileDirectory(profileId), "profile-record.json");

        if(!File.Exists(profilePath)) return null;

        try{
            var record = PersistenceJson.Deserialize<ProfileRecord>(File.ReadAllText(profilePath));

            return ProfileRecordMigration.ApplyOnLoad(record);
        }
        catch(Exception ex){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Could not load profile record: {ex.Message}"));

            return null;
        }
    }

    private static RaidRecord TryDeserializeRaid(string json){
        try{
            var raid = PersistenceJson.Deserialize<RaidRecord>(json);

            return RaidRecordMigration.ApplyOnLoad(raid);
        }
        catch(Exception ex){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Skipped invalid raid JSON: {ex.Message}"));

            return null;
        }
    }

    private static string GetProfileDirectory(string profileId){
        foreach(var c in Path.GetInvalidFileNameChars()) profileId = profileId.Replace(c, '_');

        return Path.Combine(DataRoot, profileId);
    }
}
