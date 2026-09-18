using System.Reflection;
using EFT;
using HarmonyLib;
using JsonType;

namespace Softwyx.CareerLog.Interop;

/// <summary>Readable accessors for post-raid session-end flow members.</summary>
internal static class PostRaidHealthScreenAccess{
    private static readonly MethodInfo ContinueFromStatisticsMethod = AccessTools.Method(
         typeof(SessionResultShowOperation),
         GameAssemblyNames.PostRaidHealthScreenMethods.ContinueFromStatistics
        );

    internal static readonly MethodInfo ContinueToKillListMethod = AccessTools.Method(
         typeof(SessionResultShowOperation),
         GameAssemblyNames.PostRaidHealthScreenMethods.ContinueToKillList
        );

    internal static readonly MethodInfo ContinueToStatisticsMethod = AccessTools.Method(
         typeof(SessionResultShowOperation),
         GameAssemblyNames.PostRaidHealthScreenMethods.ContinueToStatistics
        );

    internal static Profile GetProfile(SessionResultShowOperation screen){
        if(screen == null) return null;

        return AccessTools.Property(
                                    typeof(SessionResultShowOperation),
                                    GameAssemblyNames.PostRaidHealthScreenProperties.Profile
                                   )?.
                           GetValue(screen, null) as Profile;
    }

    internal static LocationSettings.Location GetLocation(SessionResultShowOperation screen){
        if(screen == null) return null;

        return AccessTools.Field(
                                 typeof(SessionResultShowOperation),
                                 GameAssemblyNames.PostRaidHealthScreenFields.Location
                                )?.
                           GetValue(screen) as LocationSettings.Location;
    }

    internal static void ContinueFromStatistics(SessionResultShowOperation screen){
        ContinueFromStatisticsMethod?.Invoke(screen, null);
    }

    internal static void ShowKillList(SessionResultShowOperation screen){
        ContinueToKillListMethod?.Invoke(screen, null);
    }
}
