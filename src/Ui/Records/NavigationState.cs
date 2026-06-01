using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Persistence;
using Softwyx.CareerLog.Persistence.Financial;

namespace Softwyx.CareerLog.Ui.Records;

/// <summary>Tab + raid selection; reset when leaving Records back to main menu.</summary>
internal static class NavigationState{
    public const int RaidsPageSize     = 20;
    public const int FinancialPageSize = 20;

    public static Tab ActiveTab{
        get;
        private set;
    }

    public static string SelectedRaidId{
        get;
        private set;
    }

    public static int RaidsListPage{
        get;
        private set;
    }

    public static SideFilter ActiveSideFilter{
        get;
        private set;
    }

    public static PeriodKind FinancialPeriod{
        get;
        private set;
    } = PeriodKind.Day;

    public static int FinancialPeriodOffset{
        get;
        private set;
    }

    public static ChartStyle ChartStyle{
        get;
        private set;
    }

    public static ViewMode FinancialView{
        get;
        private set;
    } = ViewMode.Chart;

    public static int FinancialTablePage{
        get;
        private set;
    }

    public static void Reset(){
        ActiveTab        = Tab.Summary;
        SelectedRaidId   = null;
        RaidsListPage    = 0;
        ActiveSideFilter = Settings.ParseSideFilter();
        ResetFinancialNavigation();
    }

    private static void ResetFinancialNavigation(){
        FinancialPeriod       = PeriodKind.Day;
        FinancialPeriodOffset = 0;
        ChartStyle            = Settings.ParseChartStyle();
        FinancialView         = ViewMode.Chart;
        FinancialTablePage    = 0;
    }

    public static void SetFinancialPeriod(PeriodKind period){
        FinancialPeriod       = period;
        FinancialPeriodOffset = 0;
        FinancialTablePage    = 0;
    }

    public static void SetFinancialPeriodOffset(int offset){
        FinancialPeriodOffset = offset;
    }

    public static void ShiftFinancialPeriod(int delta){
        FinancialPeriodOffset += delta;
        FinancialTablePage    =  0;
    }

    public static void SetChartStyle(ChartStyle style){
        ChartStyle = style;
    }

    public static void SetFinancialView(ViewMode view){
        FinancialView      = view;
        FinancialTablePage = 0;
    }

    public static void SetFinancialTablePage(int pageIndex){
        FinancialTablePage = pageIndex < 0 ? 0 : pageIndex;
    }

    public static void SetSideFilter(SideFilter filter){
        ActiveSideFilter = filter;
        SelectedRaidId   = null;
        RaidsListPage    = 0;
    }

    public static void SetTab(Tab recordsTab){
        ActiveTab      = recordsTab;
        SelectedRaidId = null;
        RaidsListPage  = 0;

        if(recordsTab == Tab.Financial) ResetFinancialNavigation();
    }

    public static void SelectRaid(string raidId){
        SelectedRaidId = string.IsNullOrEmpty(raidId) ? null : raidId;
    }

    public static void ClearRaidSelection(){
        SelectedRaidId = null;
    }

    public static void SetRaidsPage(int pageIndex){
        RaidsListPage = pageIndex < 0 ? 0 : pageIndex;
    }
}
