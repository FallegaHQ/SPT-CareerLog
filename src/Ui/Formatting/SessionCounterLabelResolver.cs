using System;
using System.Collections.Generic;
using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Ui.Formatting;

/// <summary>
///     Maps a <see cref="VanillaSessionCounterKeys" /> field name to a display label.
///     Priority: game-locale key → mod-locale fallback (for stats without translated labels) → raw field name.
/// </summary>
internal static class SessionCounterLabelResolver{
    /// <summary>
    ///     Field name → game locale key as used by the vanilla statistics screen builder.
    ///     Source: assembly dump -- AddBattleStats / AddHealthStats / AddLootingStats etc.
    /// </summary>
    private static readonly Dictionary<string, string> GameKeyByField = new(StringComparer.Ordinal){
                                                                                                       // ----- Combat -----
                                                                                                       {
                                                                                                           VanillaSessionCounterKeys.Kills,
                                                                                                           "fatalHits"
                                                                                                       },{
                                                                                                             VanillaSessionCounterKeys.HeadShots,
                                                                                                             "headshots"
                                                                                                         },{
                                                                                                               VanillaSessionCounterKeys.LongShots,
                                                                                                               "longshots"
                                                                                                           },{
                                                                                                                 VanillaSessionCounterKeys.
                                                                                                                     LongestShot,
                                                                                                                 "longshotDist"
                                                                                                             },{
                                                                                                                   VanillaSessionCounterKeys.
                                                                                                                       LongestKillStreak,
                                                                                                                   "killStreak"
                                                                                                               },{
                                                                                                                     VanillaSessionCounterKeys.AmmoUsed,
                                                                                                                     "ammoUsed"
                                                                                                                 },{
                                                                                                                       VanillaSessionCounterKeys.HitCount,
                                                                                                                       "hitCount"
                                                                                                                   },{
                                                                                                                         VanillaSessionCounterKeys.
                                                                                                                             AmmoReached,
                                                                                                                         "overallAccr"
                                                                                                                     },{
                                                                                                                           VanillaSessionCounterKeys.
                                                                                                                               CauseBodyDamage,
                                                                                                                           "damAppliedBody"
                                                                                                                       },{
                                                                                                                             VanillaSessionCounterKeys.
                                                                                                                                 CauseArmorDamage,
                                                                                                                             "damAppliedArmor"
                                                                                                                         },{
                                                                                                                               VanillaSessionCounterKeys.
                                                                                                                                   KilledLevel0010,
                                                                                                                               "010Kills"
                                                                                                                           },{
                                                                                                                                 VanillaSessionCounterKeys.
                                                                                                                                     KilledLevel1030,
                                                                                                                                 "1030Kills"
                                                                                                                             },{
                                                                                                                                   VanillaSessionCounterKeys.
                                                                                                                                       KilledLevel3050,
                                                                                                                                   "3050Kills"
                                                                                                                               },{
                                                                                                                                     VanillaSessionCounterKeys.
                                                                                                                                         KilledLevel5070,
                                                                                                                                     "5070Kills"
                                                                                                                                 },{
                                                                                                                                       VanillaSessionCounterKeys.
                                                                                                                                           KilledLevel7099,
                                                                                                                                       "7099Kills"
                                                                                                                                   },{
                                                                                                                                         VanillaSessionCounterKeys.
                                                                                                                                             KilledLevel100,
                                                                                                                                         "100Kills"
                                                                                                                                     },{
                                                                                                                                           VanillaSessionCounterKeys.
                                                                                                                                               KilledBear,
                                                                                                                                           "bearKills"
                                                                                                                                       },{
                                                                                                                                             VanillaSessionCounterKeys.
                                                                                                                                                 KilledUsec,
                                                                                                                                             "usecKills"
                                                                                                                                         },{
                                                                                                                                               VanillaSessionCounterKeys.
                                                                                                                                                   KilledSavage,
                                                                                                                                               "savageKills"
                                                                                                                                           },{
                                                                                                                                                 VanillaSessionCounterKeys.KilledPmc,
                                                                                                                                                 "pmcKills"
                                                                                                                                             },{
                                                                                                                                                   VanillaSessionCounterKeys.
                                                                                                                                                       KilledBoss,
                                                                                                                                                   "bossKills"
                                                                                                                                               },{
                                                                                                                                                     VanillaSessionCounterKeys.
                                                                                                                                                         KilledWithKnife,
                                                                                                                                                     "knifeKills"
                                                                                                                                                 },{
                                                                                                                                                       VanillaSessionCounterKeys.
                                                                                                                                                           KilledWithPistol,
                                                                                                                                                       "pistolKills"
                                                                                                                                                   },{
                                                                                                                                                         VanillaSessionCounterKeys.
                                                                                                                                                             KilledWithSmg,
                                                                                                                                                         "smgKills"
                                                                                                                                                     },{
                                                                                                                                                           VanillaSessionCounterKeys.
                                                                                                                                                               KilledWithShotgun,
                                                                                                                                                           "shotgunKills"
                                                                                                                                                       },{
                                                                                                                                                             VanillaSessionCounterKeys.
                                                                                                                                                                 KilledWithAssaultRifle,
                                                                                                                                                             "assaultKills"
                                                                                                                                                         },{
                                                                                                                                                               VanillaSessionCounterKeys.
                                                                                                                                                                   KilledWithAssaultCarbine,
                                                                                                                                                               "carbineKills"
                                                                                                                                                           },{
                                                                                                                                                                 VanillaSessionCounterKeys.
                                                                                                                                                                     KilledWithGrenadeLauncher,
                                                                                                                                                                 "glKills"
                                                                                                                                                             },{
                                                                                                                                                                   VanillaSessionCounterKeys.
                                                                                                                                                                       KilledWithMachineGun,
                                                                                                                                                                   "mgKills"
                                                                                                                                                               },{
                                                                                                                                                                     VanillaSessionCounterKeys.
                                                                                                                                                                         KilledWithMarksmanRifle,
                                                                                                                                                                     "dmrKills"
                                                                                                                                                                 },{
                                                                                                                                                                       VanillaSessionCounterKeys.
                                                                                                                                                                           KilledWithSniperRifle,
                                                                                                                                                                       "sniperKills"
                                                                                                                                                                   },{
                                                                                                                                                                         VanillaSessionCounterKeys.
                                                                                                                                                                             KilledWithSpecialWeapon,
                                                                                                                                                                         "specKills"
                                                                                                                                                                     },{
                                                                                                                                                                           VanillaSessionCounterKeys.
                                                                                                                                                                               KilledWithThrowWeapon,
                                                                                                                                                                           "grenadeKills"
                                                                                                                                                                       },{
                                                                                                                                                                             VanillaSessionCounterKeys.
                                                                                                                                                                                 KilledWithTripwires,
                                                                                                                                                                             "tripwireKills"
                                                                                                                                                                         },
                                                                                                       // ----- Health -----
                                                                                                       {
                                                                                                           VanillaSessionCounterKeys.BloodLoss,
                                                                                                           "bloodLost"
                                                                                                       },{
                                                                                                             VanillaSessionCounterKeys.
                                                                                                                 BodyPartsDestroyed,
                                                                                                             "bodypartsLost"
                                                                                                         },{
                                                                                                               VanillaSessionCounterKeys.Heal,
                                                                                                               "hpHealed"
                                                                                                           },{
                                                                                                                 VanillaSessionCounterKeys.Fractures,
                                                                                                                 "fractures"
                                                                                                             },{
                                                                                                                   VanillaSessionCounterKeys.
                                                                                                                       Contusions,
                                                                                                                   "contusions"
                                                                                                               },{
                                                                                                                     VanillaSessionCounterKeys.
                                                                                                                         Dehydrations,
                                                                                                                     "dehydrations"
                                                                                                                 },{
                                                                                                                       VanillaSessionCounterKeys.
                                                                                                                           Exhaustions,
                                                                                                                       "exhaustions"
                                                                                                                   },{
                                                                                                                         VanillaSessionCounterKeys.
                                                                                                                             UsedDrinks,
                                                                                                                         "drinksUsed"
                                                                                                                     },{
                                                                                                                           VanillaSessionCounterKeys.UsedFoods,
                                                                                                                           "foodUsed"
                                                                                                                       },{
                                                                                                                             VanillaSessionCounterKeys.Medicines,
                                                                                                                             "medicineUsed"
                                                                                                                         },
                                                                                                       // ----- Looting -----
                                                                                                       {
                                                                                                           VanillaSessionCounterKeys.Pedometer,
                                                                                                           "kmTraveled"
                                                                                                       },{
                                                                                                             VanillaSessionCounterKeys.MoneyUsd,
                                                                                                             "StatFoundMoneyUSD"
                                                                                                         },{
                                                                                                               VanillaSessionCounterKeys.MoneyEur,
                                                                                                               "StatFoundMoneyEUR"
                                                                                                           },{
                                                                                                                 VanillaSessionCounterKeys.MoneyRub,
                                                                                                                 "StatFoundMoneyRUB"
                                                                                                             },{
                                                                                                                   VanillaSessionCounterKeys.
                                                                                                                       BodiesLooted,
                                                                                                                   "bodiesLooted"
                                                                                                               },{
                                                                                                                     VanillaSessionCounterKeys.Triggers,
                                                                                                                     "placesLooted"
                                                                                                                 },{
                                                                                                                       VanillaSessionCounterKeys.
                                                                                                                           SafeLooted,
                                                                                                                       "unlockedSafes"
                                                                                                                   },{
                                                                                                                         VanillaSessionCounterKeys.Weapons,
                                                                                                                         "weapFound"
                                                                                                                     },{
                                                                                                                           VanillaSessionCounterKeys.Mods,
                                                                                                                           "modsFound"
                                                                                                                       },{
                                                                                                                             VanillaSessionCounterKeys.
                                                                                                                                 ThrowWeapons,
                                                                                                                             "throwFound"
                                                                                                                         },{
                                                                                                                               VanillaSessionCounterKeys.
                                                                                                                                   SpecialItems,
                                                                                                                               "specFound"
                                                                                                                           },{
                                                                                                                                 VanillaSessionCounterKeys.
                                                                                                                                     FoodDrinks,
                                                                                                                                 "foodDrinksFound"
                                                                                                                             },{
                                                                                                                                   VanillaSessionCounterKeys.Keys,
                                                                                                                                   "keysFound"
                                                                                                                               },{
                                                                                                                                     VanillaSessionCounterKeys.BartItems,
                                                                                                                                     "bartitemsFound"
                                                                                                                                 },{
                                                                                                                                       VanillaSessionCounterKeys.
                                                                                                                                           Equipments,
                                                                                                                                       "eqipFound"
                                                                                                                                   },
                                                                                                       // ----- Experience (post-raid XP screen + statistics) -----
                                                                                                       {
                                                                                                           VanillaSessionCounterKeys.ExpKill,
                                                                                                           "expKill"
                                                                                                       },{
                                                                                                             VanillaSessionCounterKeys.
                                                                                                                 ExpLooting,
                                                                                                             "expLoot"
                                                                                                         },{
                                                                                                               VanillaSessionCounterKeys.ExpHeal,
                                                                                                               "expHeal"
                                                                                                           },{
                                                                                                                 VanillaSessionCounterKeys.
                                                                                                                     ExpExitStatus,
                                                                                                                 "expSurvive"
                                                                                                             },{
                                                                                                                   VanillaSessionCounterKeys.
                                                                                                                       ExpKillBase,
                                                                                                                   "StatsElimination"
                                                                                                               },{
                                                                                                                     VanillaSessionCounterKeys.
                                                                                                                         ExpKillBodyPartBonus,
                                                                                                                     "StatsHeadshot"
                                                                                                                 },{
                                                                                                                       VanillaSessionCounterKeys.
                                                                                                                           ExpKillStreakBonus,
                                                                                                                       "StatsStreak"
                                                                                                                   },{
                                                                                                                         VanillaSessionCounterKeys.ExpDamage,
                                                                                                                         "StatsCausedHeavyDamage"
                                                                                                                     },{
                                                                                                                           VanillaSessionCounterKeys.
                                                                                                                               ExpTrigger,
                                                                                                                           "StatsExpTrigger"
                                                                                                                       },{
                                                                                                                             VanillaSessionCounterKeys.ExpEnergy,
                                                                                                                             "StatsEnergy"
                                                                                                                         },{
                                                                                                                               VanillaSessionCounterKeys.
                                                                                                                                   ExpHydration,
                                                                                                                               "StatsHydration"
                                                                                                                           },{
                                                                                                                                 VanillaSessionCounterKeys.
                                                                                                                                     ExpItemLooting,
                                                                                                                                 "StatsItems"
                                                                                                                             },{
                                                                                                                                   VanillaSessionCounterKeys.
                                                                                                                                       ExpDoorUnlocked,
                                                                                                                                   "StatsDoorsUnlocked"
                                                                                                                               },{
                                                                                                                                     VanillaSessionCounterKeys.
                                                                                                                                         ExpDoorBreached,
                                                                                                                                     "StatsDoorsBreached"
                                                                                                                                 },{
                                                                                                                                       VanillaSessionCounterKeys.
                                                                                                                                           ExpStationaryContainer,
                                                                                                                                       "StatsStationaryContainer"
                                                                                                                                   },
                                                                                                       // ----- Daily -----
                                                                                                       {
                                                                                                           VanillaSessionCounterKeys.
                                                                                                               DailyTotalCompleteCount,
                                                                                                           "Daily/Stat/Total"
                                                                                                       },{
                                                                                                             VanillaSessionCounterKeys.
                                                                                                                 DailyVeryEasyCount,
                                                                                                             "Daily/Stat/VeryEasy"
                                                                                                         },{
                                                                                                               VanillaSessionCounterKeys.
                                                                                                                   DailyEasyCount,
                                                                                                               "Daily/Stat/Easy"
                                                                                                           },{
                                                                                                                 VanillaSessionCounterKeys.
                                                                                                                     DailyMediumCount,
                                                                                                                 "Daily/Stat/Medium"
                                                                                                             },{
                                                                                                                   VanillaSessionCounterKeys.
                                                                                                                       DailyHardCount,
                                                                                                                   "Daily/Stat/Hard"
                                                                                                               },{
                                                                                                                     VanillaSessionCounterKeys.
                                                                                                                         DailyVeryHardCount,
                                                                                                                     "Daily/Stat/VeryHard"
                                                                                                                 },{
                                                                                                                       VanillaSessionCounterKeys.
                                                                                                                           DailyAvgCompletionTimeDay,
                                                                                                                       "Daily/Stat/TimeDay"
                                                                                                                   },{
                                                                                                                         VanillaSessionCounterKeys.
                                                                                                                             DailyAvgCompletionTimeWeek,
                                                                                                                         "Daily/Stat/TimeWeek"
                                                                                                                     },{
                                                                                                                           VanillaSessionCounterKeys.
                                                                                                                               DailyMaxCompleteStreak,
                                                                                                                           "Daily/Stat/MaxCompleteStreak"
                                                                                                                       },{
                                                                                                                             VanillaSessionCounterKeys.
                                                                                                                                 DailyCurrentCompleteStreak,
                                                                                                                             "Daily/Stat/CurrentCompleteStreak"
                                                                                                                         },{
                                                                                                                               VanillaSessionCounterKeys.
                                                                                                                                   DailyTotalFailCount,
                                                                                                                               "Daily/Stat/Fails"
                                                                                                                           },{
                                                                                                                                 VanillaSessionCounterKeys.
                                                                                                                                     DailyMaxFailStreak,
                                                                                                                                 "Daily/Stat/MaxFailStreak"
                                                                                                                             },{
                                                                                                                                   VanillaSessionCounterKeys.
                                                                                                                                       DailyMoneyEarned,
                                                                                                                                   "Daily/Stat/MoneyEarned"
                                                                                                                               },{
                                                                                                                                     VanillaSessionCounterKeys.
                                                                                                                                         DailyExpEarned,
                                                                                                                                     "Daily/Stat/ExpEarned"
                                                                                                                                 }
                                                                                                   };

    public static string Resolve(string counterFieldName){
        if(string.IsNullOrEmpty(counterFieldName)) return string.Empty;

        // 1. Use the correct game locale key derived from the vanilla statistics screen.
        if(GameKeyByField.TryGetValue(counterFieldName, out var gameKey)){
            var fromGame = GameLocaleAccess.TryLocalize(gameKey);

            if(!string.IsNullOrEmpty(fromGame)) return fromGame;
        }

        // 2. Mod-locale fallback (careerlog_counter_<fieldname_lowered>).
        var modKey      = LocaleKeys.Counter(counterFieldName);
        var fromModFile = LocaleLoader.Format(modKey);

        return !string.Equals(fromModFile, modKey, StringComparison.Ordinal)
                   ? fromModFile
                   // 3. Last resort: raw field name.
                   : counterFieldName;
    }
}
