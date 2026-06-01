using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Formatting;
using Softwyx.CareerLog.Ui.Shared;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Overview;

internal static class LifetimeStatsRows{
    public static void AddBasicSummary(
        Transform items, LifetimeStats lifetime, ScrollContentBuilder.ScrollTextStyle style
    ){
        if(items == null || lifetime == null) return;

        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatRaidsRecorded,
                                                 lifetime.RaidsRecorded.ToString(),
                                                 style
                                                );

        if(lifetime.RaidsRecorded <= 0) return;

        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatSurvivalRate,
                                                 FormatPercent(lifetime.SurvivalRate),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatExtractRate,
                                                 FormatPercent(lifetime.ExtractRate),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatTotalKills,
                                                 lifetime.TotalKills.ToString(),
                                                 style
                                                );
    }

    public static void AddFullOverview(
        Transform items, LifetimeStats lifetime, ScrollContentBuilder.ScrollTextStyle style
    ){
        if(items == null || lifetime == null) return;

        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatRaidsRecorded,
                                                 lifetime.RaidsRecorded.ToString(),
                                                 style
                                                );

        if(lifetime.RaidsRecorded <= 0){
            ScrollContentBuilder.AddLocalizedBodyLine(items, LocaleKeys.NoLifetime, style);

            return;
        }

        ScrollContentBuilder.AddSubHeader(items, LocaleLoader.Format(LocaleKeys.SectionRaids), style);
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatRaidsSurvived,
                                                 lifetime.RaidsSurvived.ToString(),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatRaidsRunThrough,
                                                 lifetime.RaidsRunThrough.ToString(),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatRaidsKilled,
                                                 lifetime.RaidsKilled.ToString(),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatRaidsMissingInAction,
                                                 lifetime.RaidsMissingInAction.ToString(),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(items, LocaleKeys.StatRaidsLeft, lifetime.RaidsLeft.ToString(), style);
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatRaidsTransit,
                                                 lifetime.RaidsTransit.ToString(),
                                                 style
                                                );

        ScrollContentBuilder.AddSubHeader(items, LocaleLoader.Format(LocaleKeys.SectionOverview), style);
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatSurvivalRate,
                                                 FormatPercent(lifetime.SurvivalRate),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatExtractRate,
                                                 FormatPercent(lifetime.ExtractRate),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatTotalKills,
                                                 lifetime.TotalKills.ToString(),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatTotalHeadshots,
                                                 lifetime.TotalHeadShots.ToString(),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 items,
                                                 LocaleKeys.StatFavoriteMap,
                                                 LocationNameResolver.Resolve(lifetime.FavoriteLocationId),
                                                 style
                                                );
    }

    private static string FormatPercent(double value){
        return $"{value * 100.0:0.#}%";
    }
}
