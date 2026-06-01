using System;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Persistence;

internal static class SideFilterValues{
    public const  string Combined = "Combined";
    private const string Pmc      = "Pmc";
    private const string Scav     = "Scav";

    public static SideFilter Parse(string value){
        if(string.Equals(value, Pmc, StringComparison.OrdinalIgnoreCase)) return SideFilter.Pmc;

        return string.Equals(value, Scav, StringComparison.OrdinalIgnoreCase) ? SideFilter.Scav : SideFilter.Combined;
    }

    public static bool Matches(RaidIndexEntry entry, SideFilter filter){
        if(entry == null || filter == SideFilter.Combined) return entry != null;

        var isScav = RaidPlayedSideValues.IsScav(entry.PlayedSide);

        return filter == SideFilter.Scav ? isScav : !isScav;
    }
}
