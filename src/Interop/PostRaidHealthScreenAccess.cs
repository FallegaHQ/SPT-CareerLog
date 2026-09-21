using System.Reflection;
using EFT;
using HarmonyLib;
using JsonType;

namespace Softwyx.CareerLog.Interop;

/// <summary>Readable accessors for post-raid session-end flow members.</summary>
internal static class PostRaidHealthScreenAccess{
    internal static readonly MethodInfo ContinueToKillListMethod = AccessTools.Method(
         typeof(SessionResultShowOperation),
         GameAssemblyNames.PostRaidHealthScreenMethods.ContinueToKillList
        );

    internal static readonly MethodInfo ContinueToStatisticsMethod = AccessTools.Method(
         typeof(SessionResultShowOperation),
         GameAssemblyNames.PostRaidHealthScreenMethods.ContinueToStatistics
        );

    internal static Profile GetProfile(SessionResultShowOperation screen){
        return screen?.SelectedProfile;
    }

    internal static LocationSettings.Location GetLocation(SessionResultShowOperation screen){
        return screen?._location;
    }

    internal static void ContinueFromStatistics(SessionResultShowOperation screen){
        screen?.ProceedFromKillList();
    }

    internal static void ShowKillList(SessionResultShowOperation screen){
        screen?.CG_get_ExitStatusScreenController();
    }
}
