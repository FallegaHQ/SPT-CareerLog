using EFT.UI.Screens;

namespace Softwyx.CareerLog.Infrastructure;

internal static class ScreenTypes{
    private const int DebriefScreenValue = 9001;
    private const int RecordsScreenValue = 9002;
    private const int RaidMapScreenValue = 9003;

    public static EEftScreenType Debrief => (EEftScreenType) DebriefScreenValue;

    public static EEftScreenType Records => (EEftScreenType) RecordsScreenValue;

    public static EEftScreenType RaidMap => (EEftScreenType) RaidMapScreenValue;
}
