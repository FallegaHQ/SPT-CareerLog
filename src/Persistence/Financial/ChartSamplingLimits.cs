using Softwyx.CareerLog.Config;

namespace Softwyx.CareerLog.Persistence.Financial;

internal static class ChartSamplingLimits{
    public static int PeriodMax(PeriodKind kind){
        return kind switch{
                   PeriodKind.Day      => Settings.FinancialChartMaxPointsDay.Value,
                   PeriodKind.Week     => Settings.FinancialChartMaxPointsWeek.Value,
                   PeriodKind.Month    => Settings.FinancialChartMaxPointsMonth.Value,
                   PeriodKind.Lifetime => Settings.FinancialChartMaxPointsLifetime.Value,
                   _                   => Settings.FinancialChartMaxPointsLifetime.Value
               };
    }

    public static int PerDayMaxWhenReducing(){
        return Settings.FinancialChartMaxPointsPerDay.Value;
    }
}
