using System;
using EFT;
using Softwyx.CareerLog.Collectors;
using Softwyx.CareerLog.Collectors.Health;
using Softwyx.CareerLog.Collectors.Loot;
using Softwyx.CareerLog.Collectors.Movement;
using Softwyx.CareerLog.Persistence;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Session;

internal static class CareerLogSession{
    private static bool Active{
        get;
        set;
    }

    internal static bool CollectorsActive => Active;

    public static RaidRecord LastSavedRaid{
        get;
        private set;
    }

    private static string   _raidId;
    private static string   _storageProfileId;
    private static string   _locationId;
    private static DateTime _startedUtc;
    private static bool     _ended;
    private static Player   _localPlayer;

    public static void OnRaidStarted(GameWorld world){
        if(!Config.Settings.Enabled.Value) return;

        Reset();

        if(!world?.MainPlayer || !world.MainPlayer.IsYourPlayer){
            CareerLogPlugin.Log?.LogDebug(PluginInfo.Format("Raid start ignored -- local player not ready."));

            return;
        }

        var profile = world.MainPlayer.Profile;

        if(profile == null){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format("Raid start ignored -- profile unavailable."));

            return;
        }

        if(!RaidProfileAttribution.TryResolveStorageProfileId(profile, out var storageProfileId)){
            CareerLogPlugin.Log?.LogWarning(
                                            PluginInfo.Format(
                                                              "Raid session not started -- storage profile id unavailable."
                                                             )
                                           );

            return;
        }

        Active = true;
        _raidId = Guid.NewGuid().
                       ToString("N");
        _storageProfileId = storageProfileId;
        _locationId       = world.LocationId ?? string.Empty;
        _startedUtc       = DateTime.UtcNow;
        _ended            = false;

        var sideLabel = RaidPlayedSideValues.ToStorage(RaidProfileAttribution.GetPlayedSide(profile));
        _localPlayer = world.MainPlayer;
        LocalRaidPlayer.Set(_localPlayer);
        RaidEventClock.Start();
        RaidEventMarkerBuffer.Clear();
        LootMarkerCollector.Clear();
        StartingLootBaseline.Clear();
        LoadoutValueCollector.Clear();
        RaidHealthBaseline.Capture(_localPlayer);
        StartingLootBaseline.Capture(_localPlayer);
        LoadoutValueCollector.CaptureStart(profile);
        MovementSampler.Start(_localPlayer);

        CareerLogPlugin.Log?.LogInfo(
                                     PluginInfo.Format(
                                                       $"Raid session started ({_locationId}, {sideLabel}, profile {_storageProfileId})."
                                                      )
                                    );
    }

    public static void OnRaidEnded(
        Profile profile, Player player, ExitStatus exitStatus, float pastTime, string locationId
    ){
        if(!Config.Settings.Enabled.Value || _ended) return;

        if(!Active){
            CareerLogPlugin.Log?.LogDebug(PluginInfo.Format("Raid end ignored -- no active session."));

            return;
        }

        if(profile == null){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format("Raid end ignored -- profile unavailable."));
            Reset();

            return;
        }

        _ended = true;

        var endedUtc = DateTime.UtcNow;

        if(!RaidProfileAttribution.TryResolveStorageProfileId(profile, out var storageProfileId)
        && string.IsNullOrEmpty(_storageProfileId)){
            CareerLogPlugin.Log?.LogError(PluginInfo.Format("Raid not saved -- storage profile id unavailable."));
            Reset();

            return;
        }

        if(string.IsNullOrEmpty(storageProfileId)) storageProfileId = _storageProfileId;

        var playedSide = RaidProfileAttribution.GetPlayedSide(profile);

        var record = new RaidRecord{
                                       RaidId          = _raidId,
                                       ProfileId       = storageProfileId,
                                       PlayedSide      = RaidPlayedSideValues.ToStorage(playedSide),
                                       LocationId      = string.IsNullOrEmpty(locationId) ? _locationId : locationId,
                                       StartedUtc      = _startedUtc.ToString("o"),
                                       EndedUtc        = endedUtc.ToString("o"),
                                       ExitStatus      = exitStatus.ToString(),
                                       DurationSeconds = pastTime
                                   };

        VanillaSessionCounterSnapshot.Capture(profile.EftStats?.SessionCounters, record);
        LoadoutValueCollector.CaptureEnd(profile);
        LootExtractInventoryReconcile.Apply(player ?? _localPlayer);
        record.Loot = RaidLootSummaryBuilder.Build(record.ExitStatus, player ?? _localPlayer);
        MovementTrailCapture.Apply(record, player ?? _localPlayer, pastTime);

        try{
            var profileRecord = ProfileRecordStore.LoadProfileRecord(storageProfileId);
            ProfileRecordStore.SaveRaid(record, profileRecord);
            LastSavedRaid = record;
            LogRaidSummary(record, profile.ProfileId);
        }
        catch(Exception ex){
            CareerLogPlugin.Log?.LogError(PluginInfo.Format($"Failed to save raid record: {ex}"));
        }
        finally{
            Reset();
        }
    }

    public static void OnWorldDisposed(){
        if(!Active || _ended) return;

        CareerLogPlugin.Log?.LogWarning(
                                        PluginInfo.Format(
                                                          "Raid ended via world dispose without session-end hook -- discarding buffer."
                                                         )
                                       );
        Reset();
    }

    private static void LogRaidSummary(RaidRecord record, string activeProfileId){
        record.VanillaSessionCountersLong.TryGetValue(VanillaSessionCounterKeys.Kills, out var kills);

        var trailPoints = CountTrailValues(record.Movement);

        CareerLogPlugin.Log?.LogInfo(
                                     PluginInfo.Format(
                                                       $"Raid saved: {record.PlayedSide} | "
                                                     + $"folder {record.ProfileId} | "
                                                     + $"active {activeProfileId} | "
                                                     + $"{record.LocationId} | {record.ExitStatus} | "
                                                     + $"{record.DurationSeconds:0}s | "
                                                     + $"kills {kills} | trail {trailPoints} pts | "
                                                     + $"id {record.RaidId}"
                                                      )
                                    );
    }

    private static int CountTrailValues(RaidMovement movement){
        if(movement?.Values == null) return 0;

        var count = 0;

        foreach(var value in movement.Values)
            if(value is{
                           IsMarker: false
                       })
                count++;

        return count;
    }

    private static void Reset(){
        MovementSampler.Stop();
        MovementSampleBuffer.Clear();
        RaidEventClock.Stop();
        RaidEventMarkerBuffer.Clear();
        LootMarkerCollector.Clear();
        StartingLootBaseline.Clear();
        LoadoutValueCollector.Clear();
        RaidHealthBaseline.Clear();
        LocalRaidPlayer.Clear();

        Active            = false;
        _raidId           = null;
        _storageProfileId = null;
        _locationId       = null;
        _startedUtc       = default;
        _ended            = false;
        _localPlayer      = null;
    }
}
