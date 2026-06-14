using System.Collections.Generic;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Shared;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal static class TopItemRows{
    public static void AddAll(
        Transform items, IReadOnlyList<StashSnapshotTopItem> topItems, ScrollContentBuilder.ScrollTextStyle style,
        int       maxCount = 10
    ){
        if(!items || topItems == null || topItems.Count == 0) return;

        var limit = topItems.Count > maxCount ? maxCount : topItems.Count;

        for(var i = 0; i < limit; i++){
            var item = topItems[i];

            if(item == null) continue;

            var name = ResolveItemCaption(item);
            ScrollContentBuilder.AddStatRow(items, name, ValueFormatter.Rubles(item.ValueRub), style);
        }
    }

    private static string ResolveItemCaption(StashSnapshotTopItem item){
        if(item == null) return string.Empty;

        var name = ItemLocale.Name(item.TemplateId) ?? item.Name ?? item.TemplateId;

        return item.Count > 1 ? $"{name} ×{item.Count}" : name;
    }
}
