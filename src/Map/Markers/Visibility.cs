using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Map.Markers;

/// <summary>Which marker families are drawn during map playback.</summary>
internal sealed class Visibility{
    public bool Achievements = true;
    public bool Death        = true;
    public bool DoorUnlocks  = true;
    public bool Extract      = true;
    public bool Healing      = true;
    public bool Injury       = true;
    public bool Kills        = true;
    public bool Loot         = true;
    public bool RememberPrefs;
    public bool Spawn = true;

    public bool IsVisible(RaidMovementValue marker){
        if(marker == null) return false;

        return marker.Type switch{
                   RaidMarkerTypes.Spawn                                                          => Spawn,
                   RaidMarkerTypes.Extract                                                        => Extract,
                   RaidMarkerTypes.Death                                                          => Death,
                   RaidMarkerTypes.Kill or RaidMarkerTypes.BossKill or RaidMarkerTypes.Killstreak => Kills,
                   RaidMarkerTypes.Loot                                                           => Loot,
                   RaidMarkerTypes.Injury                                                         => Injury,
                   RaidMarkerTypes.Healing                                                        => Healing,
                   RaidMarkerTypes.Achievement                                                    => Achievements,
                   RaidMarkerTypes.DoorUnlock                                                     => DoorUnlocks,
                   _                                                                              => true
               };
    }
}
