using Comfort.Common;
using EFT;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Collectors.Combat;

internal static class KillMarkerCollector{
    public static void Record(
        Player killer,           string victimProfileId, string victimName, EPlayerSide victimSide, EBodyPart bodyPart,
        string weaponTemplateId, float  distance,        WildSpawnType role
    ){
        if(killer == null || !killer.IsYourPlayer) return;

        var victimPoint = ResolveVictimPoint(victimProfileId, killer);
        var type        = IsBoss(role) ? RaidMarkerTypes.BossKill : RaidMarkerTypes.Kill;

        var marker = new RaidMovementValue{
                                              Point            = MarkerMapPosition.FromPlayer(killer),
                                              Type             = type,
                                              UtcOffsetSec     = RaidEventClock.ElapsedSeconds(),
                                              KillDistance     = distance,
                                              WeaponTemplateId = weaponTemplateId,
                                              Label            = victimName,
                                              Victims =[
                                                           BuildVictim(
                                                                       victimPoint,
                                                                       victimName,
                                                                       victimSide,
                                                                       bodyPart,
                                                                       weaponTemplateId,
                                                                       distance,
                                                                       role
                                                                      )
                                                       ]
                                          };

        RaidEventMarkerBuffer.Add(marker);
    }

    private static RaidMarkerVictimEntry BuildVictim(
        float[]       point, string name, EPlayerSide side, EBodyPart bodyPart, string weaponTemplateId, float distance,
        WildSpawnType role
    ){
        return new RaidMarkerVictimEntry{
                                            Point            = point,
                                            Name             = name,
                                            Side             = side.ToString(),
                                            WeaponTemplateId = weaponTemplateId,
                                            BodyPart         = bodyPart.ToString(),
                                            Distance         = distance,
                                            Boss             = IsBoss(role)
                                        };
    }

    private static float[] ResolveVictimPoint(string victimProfileId, Player killer){
        var world = Singleton<GameWorld>.Instance;

        if(world == null || string.IsNullOrEmpty(victimProfileId)) return MarkerMapPosition.FromPlayer(killer);

        var victim = world.GetAlivePlayerByProfileID(victimProfileId);

        return MarkerMapPosition.FromPlayer(victim ? victim : killer);
    }

    private static bool IsBoss(WildSpawnType role){
        return role switch{
                   WildSpawnType.sectantPriest       // Cultist Priest
                    or WildSpawnType.bossGluhar      // Glukhar
                    or WildSpawnType.bossBoar        // Kaban
                    or WildSpawnType.bossKilla       // Killa
                    or WildSpawnType.bossKnight      // Knight
                    or WildSpawnType.bossKolontay    // Kollontay
                    or WildSpawnType.bossPartisan    // Partisan
                    or WildSpawnType.bossBully       // Reshala
                    or WildSpawnType.bossSanitar     // Sanitar
                    or WildSpawnType.infectedTagilla // Shadow of Tagilla
                    or WildSpawnType.bossKojaniy     // Shturman
                    or WildSpawnType.bossTagilla     // Tagilla
                    or WildSpawnType.bossZryachiy => // Zryachiy
                       true,
                   _ => false
               };
    }
}
