using System.Reflection;
using EFT;
using HarmonyLib;

namespace Softwyx.CareerLog.Interop;

/// <summary>Readable accessors for post-raid session-end flow members.</summary>
internal static class PostRaidHealthScreenAccess{
    private static readonly MethodInfo ContinueFromStatisticsMethod = AccessTools.Method(
         typeof(PostRaidHealthScreenClass),
         GameAssemblyNames.PostRaidHealthScreenMethods.ContinueFromStatistics
        );

    internal static readonly MethodInfo ContinueToKillListMethod = AccessTools.Method(
         typeof(PostRaidHealthScreenClass),
         GameAssemblyNames.PostRaidHealthScreenMethods.ContinueToKillList
        );

    internal static readonly MethodInfo ContinueToStatisticsMethod = AccessTools.Method(
         typeof(PostRaidHealthScreenClass),
         GameAssemblyNames.PostRaidHealthScreenMethods.ContinueToStatistics
        );

    internal static Profile GetProfile(PostRaidHealthScreenClass screen){
        if(screen == null) return null;

        return AccessTools.Property(
                                    typeof(PostRaidHealthScreenClass),
                                    GameAssemblyNames.PostRaidHealthScreenProperties.Profile
                                   )?.
                           GetValue(screen, null) as Profile;
    }

    internal static LocationSettingsClass.Location GetLocation(PostRaidHealthScreenClass screen){
        if(screen == null) return null;

        return AccessTools.Field(
                                 typeof(PostRaidHealthScreenClass),
                                 GameAssemblyNames.PostRaidHealthScreenFields.Location
                                )?.
                           GetValue(screen) as LocationSettingsClass.Location;
    }

    internal static void ContinueFromStatistics(PostRaidHealthScreenClass screen){
        ContinueFromStatisticsMethod?.Invoke(screen, null);
    }

    internal static void ShowKillList(PostRaidHealthScreenClass screen){
        ContinueToKillListMethod?.Invoke(screen, null);
    }
}
