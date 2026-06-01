using System;
using System.Globalization;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Ui.Formatting;

internal static class RaidSummaryFormatter{
    public static string FormatRaidHeader(RaidRecord raid){
        if(raid == null) return LocaleLoader.Format(LocaleKeys.NoRaidData);

        var sidePrefix = FormatSidePrefix(raid.PlayedSide);
        var map        = LocationNameResolver.Resolve(raid.LocationId);
        var outcome    = ExitOutcomeLabels.DisplayLabel(raid.ExitStatus);
        var duration   = FormatDuration(raid.DurationSeconds);

        return string.IsNullOrEmpty(sidePrefix)
                   ? LocaleLoader.Format(LocaleKeys.RaidHeaderLine,         map,        outcome, duration)
                   : LocaleLoader.Format(LocaleKeys.RaidHeaderLineWithSide, sidePrefix, map,     outcome, duration);
    }

    public static string FormatRaidOutcomeNote(RaidRecord raid){
        return raid == null ? string.Empty : ExitOutcomeLabels.OutcomeNote(raid.ExitStatus);
    }

    public static string FormatRaidListLine(RaidIndexEntry entry){
        if(entry == null) return string.Empty;

        var ended = TryParseUtc(entry.EndedUtc);
        var date = ended?.ToLocalTime().
                          ToString("g", CultureInfo.CurrentCulture)
                ?? entry.EndedUtc;

        var sidePrefix = FormatSidePrefix(entry.PlayedSide);
        var map        = LocationNameResolver.Resolve(entry.LocationId);
        var outcome    = ExitOutcomeLabels.DisplayLabel(entry.ExitStatus);
        var duration   = FormatDuration(entry.DurationSeconds);
        var kills      = LocaleLoader.Format(LocaleKeys.ListKillsCount, entry.Kills);

        return string.IsNullOrEmpty(sidePrefix)
                   ? LocaleLoader.Format(LocaleKeys.RaidListLine, date, map, outcome, kills, duration)
                   : LocaleLoader.Format(
                                         LocaleKeys.RaidListLineWithSide,
                                         sidePrefix,
                                         date,
                                         map,
                                         outcome,
                                         kills,
                                         duration
                                        );
    }

    private static string FormatSidePrefix(string playedSide){
        return !RaidPlayedSideValues.IsScav(playedSide) ? string.Empty : LocaleLoader.Format(LocaleKeys.SideScavTag);
    }

    private static string FormatDuration(float seconds){
        if(seconds < 0) seconds = 0;

        var span = TimeSpan.FromSeconds(seconds);

        return span.TotalHours >= 1 ? $"{(int) span.TotalHours}h {span.Minutes}m" : $"{span.Minutes}m {span.Seconds}s";
    }

    private static DateTime? TryParseUtc(string value){
        if(string.IsNullOrEmpty(value)) return null;

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
                   ? parsed
                   : null;
    }
}
