using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Financial;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal static class ChartView{
    public static void Add(
        Transform             parent, IReadOnlyList<ChartPoint> points, ScrollContentBuilder.ScrollTextStyle style,
        Action<StashSnapshot> onSnapshotClicked
    ){
        if(!parent) return;

        if(points == null || points.Count == 0){
            var panel = ScrollContentBuilder.AddItemsPanel(parent);
            ScrollContentBuilder.AddLocalizedBodyLine(panel, LocaleKeys.FinancialChartEmpty, style);

            return;
        }

        ChartPanel.Install(parent, points, style, onSnapshotClicked);
    }
}
