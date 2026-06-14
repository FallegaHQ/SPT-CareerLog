using System;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence;
using Softwyx.CareerLog.Persistence.Financial;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Shared;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal static class TabContent{
    public static void Build(
        Transform contentRoot, string profileId, ScrollContentBuilder.ScrollTextStyle style, Action onFinancialChanged,
        Action<int> onTablePageChanged, Action<StashSnapshot> onSnapshotClicked
    ){
        if(!contentRoot) return;

        var index = SnapshotsReadApi.LoadIndex(profileId);

        var offset = NavigationState.FinancialPeriodOffset;
        PeriodNavigation.ClampOffset(index, NavigationState.FinancialPeriod, ref offset);

        if(offset != NavigationState.FinancialPeriodOffset) NavigationState.SetFinancialPeriodOffset(offset);

        var snapshots = SnapshotsReadApi.LoadForFinancialPeriod(
                                                                profileId,
                                                                NavigationState.FinancialPeriod,
                                                                NavigationState.FinancialPeriodOffset,
                                                                DateTime.UtcNow
                                                               );

        ControlsView.Add(contentRoot, index, style, onFinancialChanged);

        if(snapshots.Count == 0){
            var empty = ScrollContentBuilder.AddItemsPanel(contentRoot);
            ScrollContentBuilder.AddLocalizedBodyLine(empty, LocaleKeys.FinancialNoSnapshots, style);

            return;
        }

        if(NavigationState.FinancialView == ViewMode.Table)
            TableView.Add(contentRoot, snapshots, style, onTablePageChanged, onSnapshotClicked);
        else
            ChartView.Add(
                          contentRoot,
                          ChartBuckets.Build(
                                             snapshots,
                                             NavigationState.FinancialPeriod,
                                             NavigationState.FinancialPeriodOffset,
                                             DateTime.UtcNow
                                            ),
                          style,
                          onSnapshotClicked
                         );
    }
}
