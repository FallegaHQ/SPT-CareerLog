using System;
using System.Globalization;

namespace Softwyx.CareerLog.Persistence.Financial;

internal static class PeriodRange{
    public static (DateTime startUtc, DateTime endUtcExclusive, string label) Resolve(
        PeriodKind kind, int offset, DateTime nowUtc
    ){
        var localNow = nowUtc.ToLocalTime();

        switch(kind){
            case PeriodKind.Day:{
                var day = localNow.Date.AddDays(offset);

                return (day.ToUniversalTime(), day.AddDays(1).
                                                   ToUniversalTime(),
                        day.ToString("d MMM yyyy", CultureInfo.CurrentCulture));
            }
            case PeriodKind.Week:{
                var dayOfWeek = (int) localNow.DayOfWeek;
                var weekStart = localNow.Date.
                                         AddDays(-dayOfWeek).
                                         AddDays(offset * 7);

                return (weekStart.ToUniversalTime(), weekStart.AddDays(7).
                                                               ToUniversalTime(),
                        $"{weekStart:d MMM} – {weekStart.AddDays(6):d MMM yyyy}");
            }
            case PeriodKind.Month:{
                var monthStart = new DateTime(localNow.Year, localNow.Month, 1).AddMonths(offset);

                return (monthStart.ToUniversalTime(), monthStart.AddMonths(1).
                                                                 ToUniversalTime(),
                        monthStart.ToString("MMMM yyyy", CultureInfo.CurrentCulture));
            }
            case PeriodKind.Lifetime:
            default:{
                return (DateTime.MinValue, DateTime.MaxValue, "Lifetime");
            }
        }
    }
}
