using System.Collections.Generic;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Formatting;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Shared;

/// <summary>Shared vanilla counter row rendering for debrief and RECORDS detail.</summary>
internal static class CounterRows{
    public static void AddCategorizedRaidCounters(
        Transform                                items, RaidRecord raid, ScrollContentBuilder.ScrollTextStyle style,
        params SessionCounterCategory.Category[] skipCategories
    ){
        if(!items || raid == null) return;

        var longCounters  = raid.VanillaSessionCountersLong;
        var floatCounters = raid.VanillaSessionCountersFloat;

        if((longCounters == null || longCounters.Count == 0) && (floatCounters == null || floatCounters.Count == 0))
            return;

        AddCategory(items, SessionCounterCategory.Category.Combat, style, longCounters, floatCounters, skipCategories);
        AddCategory(items, SessionCounterCategory.Category.Health, style, longCounters, floatCounters, skipCategories);
        AddCategory(items, SessionCounterCategory.Category.Loot,   style, longCounters, floatCounters, skipCategories);
        AddCategory(
                    items,
                    SessionCounterCategory.Category.Experience,
                    style,
                    longCounters,
                    floatCounters,
                    skipCategories
                   );
        AddCategory(items, SessionCounterCategory.Category.Daily, style, longCounters, floatCounters, skipCategories);
        AddCategory(items, SessionCounterCategory.Category.Other, style, longCounters, floatCounters, skipCategories);
    }

    private static void AddCategory(
        Transform items, SessionCounterCategory.Category category, ScrollContentBuilder.ScrollTextStyle style,
        Dictionary<string, long> longCounters, Dictionary<string, float> floatCounters,
        SessionCounterCategory.Category[] skipCategories
    ){
        if(ShouldSkip(category, skipCategories)) return;

        if(!HasNonZero(category, longCounters, floatCounters)) return;

        if(items.childCount > 0) ScrollContentBuilder.AddBodyLine(items, string.Empty, style);

        ScrollContentBuilder.AddSubHeader(items, LocaleLoader.Format(CategoryLocaleKey(category)), style);

        if(longCounters is{
                              Count: > 0
                          })
            foreach(var pair in ScrollContentBuilder.SortOrdinal(longCounters)){
                if(SessionCounterCategory.For(pair.Key) != category) continue;
                if(pair.Value                           == 0) continue;

                ScrollContentBuilder.AddStatRow(
                                                items,
                                                SessionCounterLabelResolver.Resolve(pair.Key),
                                                pair.Value.ToString(),
                                                style
                                               );
            }

        if(floatCounters is not{
                                   Count: > 0
                               })
            return;

        foreach(var pair in ScrollContentBuilder.SortOrdinal(floatCounters)){
            if(SessionCounterCategory.For(pair.Key) != category) continue;
            if(pair.Value                           == 0f) continue;

            ScrollContentBuilder.AddStatRow(
                                            items,
                                            SessionCounterLabelResolver.Resolve(pair.Key),
                                            pair.Value.ToString("0.#"),
                                            style
                                           );
        }
    }

    private static bool ShouldSkip(
        SessionCounterCategory.Category category, SessionCounterCategory.Category[] skipCategories
    ){
        if(skipCategories == null || skipCategories.Length == 0) return false;

        foreach(var skip in skipCategories)
            if(skip == category)
                return true;

        return false;
    }

    private static bool HasNonZero(
        SessionCounterCategory.Category category, Dictionary<string, long> longCounters,
        Dictionary<string, float>       floatCounters
    ){
        if(longCounters != null)
            foreach(var pair in longCounters){
                if(SessionCounterCategory.For(pair.Key) != category) continue;

                if(pair.Value != 0) return true;
            }

        if(floatCounters == null) return false;

        foreach(var pair in floatCounters){
            if(SessionCounterCategory.For(pair.Key) != category) continue;

            if(pair.Value != 0f) return true;
        }

        return false;
    }

    private static string CategoryLocaleKey(SessionCounterCategory.Category category){
        return category switch{
                   SessionCounterCategory.Category.Combat     => LocaleKeys.CounterCategoryCombat,
                   SessionCounterCategory.Category.Health     => LocaleKeys.CounterCategoryHealth,
                   SessionCounterCategory.Category.Loot       => LocaleKeys.CounterCategoryLoot,
                   SessionCounterCategory.Category.Experience => LocaleKeys.CounterCategoryExperience,
                   SessionCounterCategory.Category.Daily      => LocaleKeys.CounterCategoryDaily,
                   _                                          => LocaleKeys.CounterCategoryOther
               };
    }
}
