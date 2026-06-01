using EFT.UI;
using EFT.UI.SessionEnd;
using Softwyx.CareerLog.Interop;
using TMPro;

namespace Softwyx.CareerLog.Infrastructure;

/// <summary>Cached Harmony field access for session-end statistics screen templates.</summary>
internal static class SessionEndScreenReflection{
    public static DefaultUIButton GetNextButton(SessionResultStatistics template){
        return EftScreenFieldBinder.GetField<DefaultUIButton>(
                                                              template,
                                                              GameAssemblyNames.SessionResultStatisticsFields.NextButton
                                                             );
    }

    public static DefaultUIButton GetBackButton(SessionResultStatistics template){
        return EftScreenFieldBinder.GetField<DefaultUIButton>(
                                                              template,
                                                              GameAssemblyNames.SessionResultStatisticsFields.BackButton
                                                             );
    }

    public static StatisticsSpawn GetStatsSpawn(SessionResultStatistics template){
        return EftScreenFieldBinder.GetField<StatisticsSpawn>(
                                                              template,
                                                              GameAssemblyNames.SessionResultStatisticsFields.StatsSpawn
                                                             );
    }

    public static TextMeshProUGUI GetLocationName(SessionResultStatistics template){
        return EftScreenFieldBinder.GetTextMeshProUGUI(
                                                       template,
                                                       GameAssemblyNames.SessionResultStatisticsFields.LocationName
                                                      );
    }
}
