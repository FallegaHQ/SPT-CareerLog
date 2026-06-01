using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.SessionEnd.Debrief;

internal static class DebriefScrollContentBuilder{
    public static void Rebuild(Transform contentRoot, RaidRecord raidRecord, TextMeshProUGUI styleSource){
        if(!contentRoot) return;

        ScrollContentBuilder.Clear(contentRoot);
        ScrollContentBuilder.EnsureContentLayout(contentRoot);

        var style = ScrollContentBuilder.ResolveStyle(styleSource);

        LootSummarySection.Add(contentRoot, raidRecord, style);
        BuildCountersSection(contentRoot, raidRecord, style);

        ScrollContentBuilder.RebuildLayout(contentRoot);
    }

    public static void Clear(Transform contentRoot){
        ScrollContentBuilder.Clear(contentRoot);
    }

    private static void BuildCountersSection(
        Transform contentRoot, RaidRecord raidRecord, ScrollContentBuilder.ScrollTextStyle style
    ){
        if(raidRecord == null){
            var section = ScrollContentBuilder.AddSection(contentRoot, LocaleKeys.SectionSessionCounters, style);
            var items   = ScrollContentBuilder.GetItemsContainer(section);
            ScrollContentBuilder.AddBodyLine(items, LocaleLoader.Format(LocaleKeys.NoRaidData), style);

            return;
        }

        var hasLong  = raidRecord.VanillaSessionCountersLong.Count  > 0;
        var hasFloat = raidRecord.VanillaSessionCountersFloat.Count > 0;

        if(!hasLong && !hasFloat){
            var section = ScrollContentBuilder.AddSection(contentRoot, LocaleKeys.SectionSessionCounters, style);
            var items   = ScrollContentBuilder.GetItemsContainer(section);
            ScrollContentBuilder.AddBodyLine(items, LocaleLoader.Format(LocaleKeys.DebriefNoAdditionalData), style);

            return;
        }

        var counterSection = ScrollContentBuilder.AddSection(contentRoot, LocaleKeys.SectionSessionCounters, style);
        var counterItems   = ScrollContentBuilder.GetItemsContainer(counterSection);

        CounterRows.AddCategorizedRaidCounters(
                                               counterItems,
                                               raidRecord,
                                               style,
                                               SessionCounterCategory.Category.Experience
                                              );
    }
}
