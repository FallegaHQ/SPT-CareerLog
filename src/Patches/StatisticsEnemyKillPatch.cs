using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT;
using EFT.HealthSystem;
using Softwyx.CareerLog.Collectors;
using Softwyx.CareerLog.Collectors.Combat;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class StatisticsEnemyKillPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(LocationStatisticsCollectorAbstractClass).GetMethod(
                                                                          nameof(
                                                                              LocationStatisticsCollectorAbstractClass.
                                                                                  OnEnemyKill)
                                                                         );
    }

    [PatchPostfix]
    private static void Postfix(
        LocationStatisticsCollectorAbstractClass __instance, DamageInfoStruct damage, EDamageType lethalDamageType,
        EBodyPart bodyPart, EPlayerSide playerSide, WildSpawnType role, string playerAccountId, string playerProfileId,
        string playerName, string groupId, int level, int killExp, float distance, int hour,
        List<string> targetEquipment, HealthEffects enemyEffects, List<string> zoneIds, bool isFriendly, bool isAI
    ){
        if(!Settings.Enabled.Value || !CareerLogSession.CollectorsActive) return;

        if(!CollectorProfileGuard.IsLocalStatisticsProfile(__instance.Profile_0)) return;

        var killer = LocalRaidPlayer.Instance;

        if(killer == null) return;

        var weaponTemplateId = damage.Weapon?.TemplateId.ToString();

        KillMarkerCollector.Record(
                                   killer,
                                   playerProfileId,
                                   playerName,
                                   playerSide,
                                   bodyPart,
                                   weaponTemplateId,
                                   distance,
                                   role
                                  );
    }
}
