using System;
using System.Collections.Generic;
using System.IO;
using Softwyx.CareerLog.Persistence.Models;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Markers;

/// <summary>Loads <c>pre-colored</c> map marker PNGs bundled under <c>assets/maps/markers/</c>.</summary>
internal static class IconLoader{
    private const string ExtractFile          = "exit.png";
    private const string SpawnFile            = "whistle.png";
    private const string DeathFile            = "death.png";
    private const string KillFile             = "kill.png";
    private const string KillstreakFile       = "killstreak.png";
    private const string BossKillFile         = "boss_kill.png";
    private const string LootFile             = "loot.png";
    private const string InjuryFile           = "injury.png";
    private const string HealingFile          = "healing.png";
    private const string AchievementFile      = "achievement.png";
    private const string DoorUnlockFile       = "key-unlock.png";
    private const string LongestKillBadgeFile = "longest_kill_badge.png";
    private const string VictimFile           = "victim.png";

    private static readonly Dictionary<string, Sprite> Cache = new(StringComparer.OrdinalIgnoreCase);

    public static bool TryGetForValue(RaidMovementValue value, out Sprite sprite){
        sprite = null;

        if(value is not{
                           IsMarker: true
                       })
            return false;

        switch(value.Type){
            case RaidMarkerTypes.Spawn:
                sprite = Load(SpawnFile);

                return sprite;
            case RaidMarkerTypes.Extract:
                sprite = Load(ExtractFile);

                return sprite;
            case RaidMarkerTypes.Death:
                sprite = Load(DeathFile);

                return sprite;
            case RaidMarkerTypes.Kill:
                sprite = Load(KillFile);

                return sprite;
            case RaidMarkerTypes.Killstreak:
                sprite = Load(KillstreakFile);

                return sprite;
            case RaidMarkerTypes.BossKill:
                sprite = Load(BossKillFile);

                return sprite;
            case RaidMarkerTypes.Loot:
                sprite = Load(LootFile);

                return sprite;
            case RaidMarkerTypes.Injury:
                sprite = Load(InjuryFile);

                return sprite;
            case RaidMarkerTypes.Healing:
                sprite = Load(HealingFile);

                return sprite;
            case RaidMarkerTypes.Achievement:
                sprite = Load(AchievementFile);

                return sprite;
            case RaidMarkerTypes.DoorUnlock:
                sprite = Load(DoorUnlockFile);

                return sprite;
            default:
                return false;
        }
    }

    public static bool TryGetLongestKillBadge(out Sprite sprite){
        sprite = Load(LongestKillBadgeFile);

        return sprite;
    }

    public static bool TryGetVictimIcon(out Sprite sprite){
        sprite = Load(VictimFile);

        return sprite != null;
    }

    private static Sprite Load(string fileName){
        if(string.IsNullOrEmpty(fileName)) return null;

        if(Cache.TryGetValue(fileName, out var cached)) return cached;

        var path = PluginPaths.MapMarkerFile(fileName);

        if(!File.Exists(path)){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Map marker icon missing: {path}"));

            return null;
        }

        try{
            var raw = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            raw.LoadImage(File.ReadAllBytes(path));

            var sprite = Sprite.Create(raw, new Rect(0f, 0f, raw.width, raw.height), new Vector2(0.5f, 0.5f), 256f);

            Cache[fileName] = sprite;

            return sprite;
        }
        catch(Exception ex){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Map marker icon load failed ({path}): {ex.Message}"));

            return null;
        }
    }
}
