using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence;
using Softwyx.CareerLog.Ui.Shared;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal static class SummaryTabContent{
    public static void Build(
        Transform contentRoot, SnapshotSummaryView summary, ScrollContentBuilder.ScrollTextStyle style
    ){
        if(!contentRoot) return;

        if(!summary.HasSnapshots || summary.Latest == null){
            var emptyItems = ScrollContentBuilder.AddItemsPanel(contentRoot);
            ScrollContentBuilder.AddLocalizedBodyLine(emptyItems, LocaleKeys.FinancialNoSnapshots, style);

            return;
        }

        var latest = summary.Latest;

        var section = ScrollContentBuilder.AddSection(contentRoot, LocaleKeys.SectionSummary, style);
        var items   = ScrollContentBuilder.GetItemsContainer(section);

        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialTotalWorth,
                                                 ValueFormatter.Rubles(latest.TotalWorth),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialLiquidRubles,
                                                 ValueFormatter.Rubles(latest.Rubles),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialItemsValue,
                                                 ValueFormatter.Rubles(latest.ItemsValue),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialEquippedValue,
                                                 ValueFormatter.Rubles(latest.EquippedValue),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialInventoryValue,
                                                 ValueFormatter.Rubles(latest.InventoryValue),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialItemCount,
                                                 latest.ItemCount.ToString(),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.FinancialNetChange,
                                                 ValueFormatter.SignedRubles(
                                                                             latest.TotalWorth - summary.FirstTotalWorth
                                                                            ),
                                                 style
                                                );

        ScrollContentBuilder.AddLocalizedBodyLine(items, LocaleKeys.FinancialEstimatedDisclaimer, style);

        if(latest.TopItems is not{
                                     Count: > 0
                                 })
            return;

        var topSection = ScrollContentBuilder.AddSection(contentRoot, LocaleKeys.FinancialTopItemsHeader, style);
        var topItems   = ScrollContentBuilder.GetItemsContainer(topSection);

        TopItemRows.AddAll(topItems, latest.TopItems, style);
    }
}
