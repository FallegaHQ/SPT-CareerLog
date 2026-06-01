using UnityEngine;

namespace Softwyx.CareerLog.Map.Markers;

internal static class FilterPrefs{
    private const string KeySpawn        = "rad_map_filter_spawn";
    private const string KeyKills        = "rad_map_filter_kills";
    private const string KeyLoot         = "rad_map_filter_loot";
    private const string KeyInjury       = "rad_map_filter_injury";
    private const string KeyHealing      = "rad_map_filter_healing";
    private const string KeyAchievements = "rad_map_filter_achievements";
    private const string KeyDoorUnlocks  = "rad_map_filter_door_unlock";
    private const string KeyExtract      = "rad_map_filter_extract";
    private const string KeyDeath        = "rad_map_filter_death";
    private const string KeyRemember     = "rad_map_filter_remember";

    public static Visibility Load(){
        if(PlayerPrefs.GetInt(KeyRemember, 0) != 1) return new Visibility();

        return new Visibility{
                                 Spawn         = PlayerPrefs.GetInt(KeySpawn,        1) == 1,
                                 Kills         = PlayerPrefs.GetInt(KeyKills,        1) == 1,
                                 Loot          = PlayerPrefs.GetInt(KeyLoot,         1) == 1,
                                 Injury        = PlayerPrefs.GetInt(KeyInjury,       1) == 1,
                                 Healing       = PlayerPrefs.GetInt(KeyHealing,      1) == 1,
                                 Achievements  = PlayerPrefs.GetInt(KeyAchievements, 1) == 1,
                                 DoorUnlocks   = PlayerPrefs.GetInt(KeyDoorUnlocks,  1) == 1,
                                 Extract       = PlayerPrefs.GetInt(KeyExtract,      1) == 1,
                                 Death         = PlayerPrefs.GetInt(KeyDeath,        1) == 1,
                                 RememberPrefs = true
                             };
    }

    public static void Save(Visibility visibility){
        if(visibility is not{
                                RememberPrefs: true
                            })
            return;

        PlayerPrefs.SetInt(KeySpawn,        visibility.Spawn ? 1 : 0);
        PlayerPrefs.SetInt(KeyKills,        visibility.Kills ? 1 : 0);
        PlayerPrefs.SetInt(KeyLoot,         visibility.Loot ? 1 : 0);
        PlayerPrefs.SetInt(KeyInjury,       visibility.Injury ? 1 : 0);
        PlayerPrefs.SetInt(KeyHealing,      visibility.Healing ? 1 : 0);
        PlayerPrefs.SetInt(KeyAchievements, visibility.Achievements ? 1 : 0);
        PlayerPrefs.SetInt(KeyDoorUnlocks,  visibility.DoorUnlocks ? 1 : 0);
        PlayerPrefs.SetInt(KeyExtract,      visibility.Extract ? 1 : 0);
        PlayerPrefs.SetInt(KeyDeath,        visibility.Death ? 1 : 0);
        PlayerPrefs.SetInt(KeyRemember,     1);
        PlayerPrefs.Save();
    }

    public static void ClearSaved(){
        PlayerPrefs.DeleteKey(KeyRemember);
        PlayerPrefs.DeleteKey(KeySpawn);
        PlayerPrefs.DeleteKey(KeyKills);
        PlayerPrefs.DeleteKey(KeyLoot);
        PlayerPrefs.DeleteKey(KeyInjury);
        PlayerPrefs.DeleteKey(KeyHealing);
        PlayerPrefs.DeleteKey(KeyAchievements);
        PlayerPrefs.DeleteKey(KeyDoorUnlocks);
        PlayerPrefs.DeleteKey(KeyExtract);
        PlayerPrefs.DeleteKey(KeyDeath);
        PlayerPrefs.Save();
    }
}
