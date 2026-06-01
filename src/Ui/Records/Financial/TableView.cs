using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Financial;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal static class TableView{
    public static void Add(
        Transform   parent,        IReadOnlyList<StashSnapshot> snapshots, ScrollContentBuilder.ScrollTextStyle style,
        Action<int> onPageChanged, Action<StashSnapshot>        onSnapshotClicked
    ){
        if(!parent) return;

        var       panel     = ScrollContentBuilder.AddItemsPanel(parent);
        var       now       = DateTime.UtcNow;
        var       period    = NavigationState.FinancialPeriod;
        var       offset    = NavigationState.FinancialPeriodOffset;
        const int pageSize  = NavigationState.FinancialPageSize;
        var       pageCount = ChartBuckets.TablePageCount(snapshots, period, offset, now, pageSize);
        var       pageIndex = NavigationState.FinancialTablePage;

        if(pageCount > 0 && pageIndex >= pageCount) NavigationState.SetFinancialTablePage(pageCount - 1);

        pageIndex = NavigationState.FinancialTablePage;
        var rows = ChartBuckets.FilterTablePage(snapshots, period, offset, now, pageIndex, pageSize);

        if(rows.Count == 0){
            ScrollContentBuilder.AddLocalizedBodyLine(panel, LocaleKeys.FinancialChartEmpty, style);

            return;
        }

        ScrollContentBuilder.AddLocalizedBodyLine(panel, LocaleKeys.FinancialTableHint, style);

        foreach(var snapshot in rows)
            SnapshotTableBlock.Add(
                                   panel,
                                   snapshot,
                                   style,
                                   onSnapshotClicked == null ? null : () => onSnapshotClicked(snapshot)
                                  );

        if(pageCount > 1 && onPageChanged != null)
            TablePaginationView.Add(panel, pageIndex, pageCount, style, onPageChanged);
    }
}
