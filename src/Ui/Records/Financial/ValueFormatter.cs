using System;
using System.Globalization;
using Softwyx.CareerLog.Ui.Design;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal static class ValueFormatter{
    public static string Rubles(long value){
        return $"{value:N0} ₽";
    }

    public static string SignedRubles(long value){
        return value switch{
                   > 0L => $"+{value:N0} ₽",
                   < 0L => $"{value:N0} ₽",
                   _    => "0 ₽"
               };
    }

    public static string TrendRichText(long deltaRubles){
        return deltaRubles switch{
                   > 0L => Colors.Text.WrapRichText($"▲ {SignedRubles(deltaRubles)}", Colors.Text.TrendUp),
                   < 0L => Colors.Text.WrapRichText($"▼ {SignedRubles(deltaRubles)}", Colors.Text.TrendDown),
                   _    => Colors.Text.WrapRichText("-- 0 ₽",                         Colors.Text.TrendFlat)
               };
    }

    public static string LocalTime(string utc){
        if(!DateTime.TryParse(utc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
            return utc;

        return parsed.ToLocalTime().
                      ToString("g");
    }
}
