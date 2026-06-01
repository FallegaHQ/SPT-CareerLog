using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Ui.Design;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Layout;

/// <summary>RECORDS two-column shell -- delegates to focused layout types under <c>Layout/</c>.</summary>
internal static class LeftPanelLayout{
    private const float SplitAt = Spacing.RecordsLeftColumnSplit;

    public static void PrepareShell(Transform recordsRoot){
        if(!recordsRoot) return;

        ShellLayout.HideLegacyHandbookChrome(recordsRoot);

        var panelsBackground = ShellLayout.EnsurePanelsBackgroundShell(recordsRoot);

        if(!panelsBackground) return;

        var leftPanel = SplitPanelsConfigurer.ConfigureSplitPanel(
                                                                  recordsRoot,
                                                                  panelsBackground,
                                                                  UiHierarchy.RecordsScreen.LeftPanel,
                                                                  0f,
                                                                  SplitAt
                                                                 );
        LeftColumnLayout.EnsurePlayerModelSlot(leftPanel);
        SplitPanelsConfigurer.ConfigureSplitPanel(
                                                  recordsRoot,
                                                  panelsBackground,
                                                  UiHierarchy.RecordsScreen.RightPanel,
                                                  SplitAt,
                                                  1f
                                                 );
    }

    public static PanelRegions Resolve(Transform recordsRoot){
        if(!recordsRoot) return default;

        var panelsBackground = PanelDiscovery.FindPanelsBackground(recordsRoot);
        var searchRoot       = panelsBackground ? panelsBackground : recordsRoot;

        var leftPanel  = PanelDiscovery.FindPanel(searchRoot, recordsRoot, UiHierarchy.RecordsScreen.LeftPanel);
        var rightPanel = PanelDiscovery.FindPanel(searchRoot, recordsRoot, UiHierarchy.RecordsScreen.RightPanel);

        if(leftPanel) LeftColumnLayout.EnsureColumnLayout(leftPanel);

        var leftStats  = LeftColumnLayout.EnsureStatsArea(leftPanel);
        var tabBar     = RightColumnLayout.EnsureTabBar(rightPanel);
        var tabContent = RightColumnLayout.EnsureTabScroll(rightPanel, out var tabHost);

        return new PanelRegions(leftStats, tabHost, tabContent, tabBar);
    }
}
