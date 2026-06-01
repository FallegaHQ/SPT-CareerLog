using System;

namespace Softwyx.CareerLog.Persistence.Models;

internal static class RaidPlayedSideValues{
    public const string Pmc  = "Pmc";
    public const string Scav = "Scav";

    public static string ToStorage(RaidPlayedSide side){
        return side == RaidPlayedSide.Scav ? Scav : Pmc;
    }

    public static bool IsScav(string playedSide){
        return string.Equals(playedSide, Scav, StringComparison.OrdinalIgnoreCase);
    }
}
