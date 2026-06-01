using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Design;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Shared;

internal static class LootSummarySection{
    public static void Add(Transform contentRoot, RaidRecord raidRecord, ScrollContentBuilder.ScrollTextStyle style){
        var loot = raidRecord?.Loot;

        if(loot == null
        || (loot.ItemsTaken           <= 0
         && loot.ValueTakenRub        <= 0
         && loot.LoadoutValueStartRub <= 0
         && loot.BringInValueLostRub  <= 0
         && loot.ValueLostRub         <= 0))
            return;

        var section = ScrollContentBuilder.AddSection(contentRoot, LocaleKeys.SectionLootSummary, style);
        var items   = ScrollContentBuilder.GetItemsContainer(section);

        if(loot.ItemsTaken > 0)
            ScrollContentBuilder.AddStatRow(
                                            items,
                                            LocaleLoader.Format(LocaleKeys.LootCaptionItemsTaken),
                                            loot.ItemsTaken.ToString(),
                                            style
                                           );

        if(loot.ValueTakenRub > 0)
            ScrollContentBuilder.AddStatRow(
                                            items,
                                            LocaleLoader.Format(LocaleKeys.LootCaptionValueTaken),
                                            Colors.Text.ColoredValue(loot.ValueTakenRub, Colors.Text.ValuePositive),
                                            style
                                           );

        if(loot.LoadoutValueStartRub > 0)
            ScrollContentBuilder.AddStatRow(
                                            items,
                                            LocaleLoader.Format(LocaleKeys.LootCaptionLoadoutStart),
                                            $"{loot.LoadoutValueStartRub} ₽",
                                            style
                                           );

        if(loot.LoadoutValueEndRub > 0)
            ScrollContentBuilder.AddStatRow(
                                            items,
                                            LocaleLoader.Format(LocaleKeys.LootCaptionLoadoutEnd),
                                            $"{loot.LoadoutValueEndRub} ₽",
                                            style
                                           );

        if(loot.BringInValueLostRub > 0)
            ScrollContentBuilder.AddStatRow(
                                            items,
                                            LocaleLoader.Format(LocaleKeys.LootCaptionBringInValueLost),
                                            Colors.Text.ColoredValue(
                                                                     loot.BringInValueLostRub,
                                                                     Colors.Text.ValueNegative
                                                                    ),
                                            style
                                           );

        if(loot.ValueLostRub > 0)
            ScrollContentBuilder.AddStatRow(
                                            items,
                                            LocaleLoader.Format(LocaleKeys.LootCaptionValueLost),
                                            Colors.Text.ColoredValue(loot.ValueLostRub, Colors.Text.ValueNegative),
                                            style
                                           );

        if(loot.TopLooted is not{
                                    Count: > 0
                                })
            return;

        if(items.childCount > 0) ScrollContentBuilder.AddBodyLine(items, string.Empty, style);

        ScrollContentBuilder.AddSubHeader(items, LocaleLoader.Format(LocaleKeys.LootMostExpensiveHeader), style);

        foreach(var entry in loot.TopLooted){
            if(entry == null || string.IsNullOrEmpty(entry.TemplateId)) continue;

            var name    = ItemLocale.Name(entry.TemplateId) ?? entry.TemplateId;
            var caption = entry.Count > 1 ? $"{name} ×{entry.Count}" : name;
            ScrollContentBuilder.AddStatRow(items, caption, $"{entry.ValueRub} ₽", style);
        }
    }
}
