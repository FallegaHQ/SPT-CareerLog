using EFT.InventoryLogic;
using Softwyx.CareerLog.Config;
using System;
using System.Collections.Generic;

namespace Softwyx.CareerLog.Collectors.Loot;

internal static class LootMarkerFilter{
    private static HashSet<string> _templateAllowlist;
    private static string          _lastTemplateConfig;
    private static string          _lastCategoryConfig;

    public static bool ShouldMark(Item item, long valueRub){
        if(item == null) return false;

        RefreshAllowlists();

        if(valueRub >= Settings.LootMarkerMinValueRub.Value) return true;

        if(_templateAllowlist.Count > 0 && _templateAllowlist.Contains(item.TemplateId.ToString())) return true;

        return MatchesCategory(item);
    }

    private static bool MatchesCategory(Item item){
        var categories = Settings.LootMarkerCategories.Value;

        if(string.IsNullOrWhiteSpace(categories)) return false;

        var parent = item.Template?.ParentId.ToString() ?? string.Empty;

        foreach(var token in categories.Split(
                                              [
                                                  ','
                                              ],
                                              StringSplitOptions.RemoveEmptyEntries
                                             )){
            var category = token.Trim();

            if(category.Length == 0) continue;

            if(parent.IndexOf(category, StringComparison.OrdinalIgnoreCase) >= 0) return true;
        }

        return false;
    }

    private static void RefreshAllowlists(){
        var templateConfig = Settings.LootMarkerTemplateIds.Value ?? string.Empty;

        if(templateConfig == _lastTemplateConfig && Settings.LootMarkerCategories.Value == _lastCategoryConfig) return;

        _lastTemplateConfig = templateConfig;
        _lastCategoryConfig = Settings.LootMarkerCategories.Value ?? string.Empty;
        _templateAllowlist  = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach(var token in templateConfig.Split(
                                                  [
                                                      ','
                                                  ],
                                                  StringSplitOptions.RemoveEmptyEntries
                                                 ))
            _templateAllowlist.Add(token.Trim());
    }
}
