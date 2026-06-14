using System;
using System.Collections.Generic;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Ui.Shared;

/// <summary>Groups session counter keys for UI (aligned with vanilla post-raid stat sections).</summary>
internal static class SessionCounterCategory{
    private static readonly Dictionary<string, Category> ByKey = BuildMap();

    public static Category For(string counterKey){
        return string.IsNullOrEmpty(counterKey) ? Category.Other : ByKey.GetValueOrDefault(counterKey, Category.Other);
    }

    private static Dictionary<string, Category> BuildMap(){
        var map = new Dictionary<string, Category>(StringComparer.Ordinal);

        Add(
            Category.Combat,
            VanillaSessionCounterKeys.Kills,
            VanillaSessionCounterKeys.Deaths,
            VanillaSessionCounterKeys.HeadShots,
            VanillaSessionCounterKeys.HitCount,
            VanillaSessionCounterKeys.LongShots,
            VanillaSessionCounterKeys.LongestShot,
            VanillaSessionCounterKeys.LongestKillShot,
            VanillaSessionCounterKeys.LongestKillShotOnBot,
            VanillaSessionCounterKeys.LongestKillStreak,
            VanillaSessionCounterKeys.AmmoUsed,
            VanillaSessionCounterKeys.AmmoReached,
            VanillaSessionCounterKeys.CauseBodyDamage,
            VanillaSessionCounterKeys.CauseArmorDamage,
            VanillaSessionCounterKeys.CombatDamage,
            VanillaSessionCounterKeys.BodyPartHeadDamage,
            VanillaSessionCounterKeys.BodyPartChestDamage,
            VanillaSessionCounterKeys.BodyPartStomachDamage,
            VanillaSessionCounterKeys.BodyPartLeftArmDamage,
            VanillaSessionCounterKeys.BodyPartRightArmDamage,
            VanillaSessionCounterKeys.BodyPartLeftLegDamage,
            VanillaSessionCounterKeys.BodyPartRightLegDamage,
            VanillaSessionCounterKeys.KilledLevel0010,
            VanillaSessionCounterKeys.KilledLevel1030,
            VanillaSessionCounterKeys.KilledLevel3050,
            VanillaSessionCounterKeys.KilledLevel5070,
            VanillaSessionCounterKeys.KilledLevel7099,
            VanillaSessionCounterKeys.KilledLevel100,
            VanillaSessionCounterKeys.KilledBear,
            VanillaSessionCounterKeys.KilledUsec,
            VanillaSessionCounterKeys.KilledSavage,
            VanillaSessionCounterKeys.KilledPmc,
            VanillaSessionCounterKeys.KilledBoss,
            VanillaSessionCounterKeys.KilledWithKnife,
            VanillaSessionCounterKeys.KilledWithPistol,
            VanillaSessionCounterKeys.KilledWithSmg,
            VanillaSessionCounterKeys.KilledWithShotgun,
            VanillaSessionCounterKeys.KilledWithAssaultRifle,
            VanillaSessionCounterKeys.KilledWithAssaultCarbine,
            VanillaSessionCounterKeys.KilledWithGrenadeLauncher,
            VanillaSessionCounterKeys.KilledWithMachineGun,
            VanillaSessionCounterKeys.KilledWithMarksmanRifle,
            VanillaSessionCounterKeys.KilledWithSniperRifle,
            VanillaSessionCounterKeys.KilledWithSpecialWeapon,
            VanillaSessionCounterKeys.KilledWithThrowWeapon,
            VanillaSessionCounterKeys.KilledWithTripwires
           );

        Add(
            Category.Health,
            VanillaSessionCounterKeys.Pedometer,
            VanillaSessionCounterKeys.BloodLoss,
            VanillaSessionCounterKeys.BodyPartsDestroyed,
            VanillaSessionCounterKeys.Heal,
            VanillaSessionCounterKeys.Fractures,
            VanillaSessionCounterKeys.Contusions,
            VanillaSessionCounterKeys.Dehydrations,
            VanillaSessionCounterKeys.Exhaustions,
            VanillaSessionCounterKeys.Medicines,
            VanillaSessionCounterKeys.UsedFoods,
            VanillaSessionCounterKeys.UsedDrinks
           );

        Add(
            Category.Loot,
            VanillaSessionCounterKeys.MoneyRub,
            VanillaSessionCounterKeys.MoneyEur,
            VanillaSessionCounterKeys.MoneyUsd,
            VanillaSessionCounterKeys.BodiesLooted,
            VanillaSessionCounterKeys.SafeLooted,
            VanillaSessionCounterKeys.Triggers,
            VanillaSessionCounterKeys.TriggersVisited,
            VanillaSessionCounterKeys.LockableContainers,
            VanillaSessionCounterKeys.Weapons,
            VanillaSessionCounterKeys.Ammunitions,
            VanillaSessionCounterKeys.Mods,
            VanillaSessionCounterKeys.ThrowWeapons,
            VanillaSessionCounterKeys.SpecialItems,
            VanillaSessionCounterKeys.FoodDrinks,
            VanillaSessionCounterKeys.Keys,
            VanillaSessionCounterKeys.BartItems,
            VanillaSessionCounterKeys.MobContainers,
            VanillaSessionCounterKeys.Equipments,
            VanillaSessionCounterKeys.ExpDoorUnlocked,
            VanillaSessionCounterKeys.ExpDoorBreached
           );

        Add(
            Category.Experience,
            VanillaSessionCounterKeys.ExpKillBase,
            VanillaSessionCounterKeys.ExpKillBodyPartBonus,
            VanillaSessionCounterKeys.ExpKillStreakBonus,
            VanillaSessionCounterKeys.ExpItemLooting,
            VanillaSessionCounterKeys.ExpDamage,
            VanillaSessionCounterKeys.ExpHeal,
            VanillaSessionCounterKeys.ExpEnergy,
            VanillaSessionCounterKeys.ExpHydration,
            VanillaSessionCounterKeys.ExpExitStatus,
            VanillaSessionCounterKeys.ExpTrigger,
            VanillaSessionCounterKeys.ExpStationaryContainer,
            VanillaSessionCounterKeys.ExpExamine,
            VanillaSessionCounterKeys.ExpKill,
            VanillaSessionCounterKeys.ExpLooting
           );

        Add(
            Category.Daily,
            VanillaSessionCounterKeys.DailyTotalCompleteCount,
            VanillaSessionCounterKeys.DailyVeryEasyCount,
            VanillaSessionCounterKeys.DailyEasyCount,
            VanillaSessionCounterKeys.DailyMediumCount,
            VanillaSessionCounterKeys.DailyHardCount,
            VanillaSessionCounterKeys.DailyVeryHardCount,
            VanillaSessionCounterKeys.DailyAvgCompletionTimeDay,
            VanillaSessionCounterKeys.DailyAvgCompletionTimeWeek,
            VanillaSessionCounterKeys.DailyMaxCompleteStreak,
            VanillaSessionCounterKeys.DailyCurrentCompleteStreak,
            VanillaSessionCounterKeys.DailyFavoriteQuestType,
            VanillaSessionCounterKeys.DailyTotalFailCount,
            VanillaSessionCounterKeys.DailyMaxFailStreak,
            VanillaSessionCounterKeys.DailyMoneyEarned,
            VanillaSessionCounterKeys.DailyExpEarned
           );

        return map;

        void Add(Category category, params string[] keys){
            foreach(var key in keys) map[key] = category;
        }
    }

    internal enum Category{
        Combat,
        Health,
        Loot,
        Experience,
        Daily,
        Other
    }
}
