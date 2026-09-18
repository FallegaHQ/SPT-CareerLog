using System;
using System.Collections.Generic;
using EFT.InventoryLogic;
using Softwyx.CareerLog.Config;

namespace Softwyx.CareerLog.Collectors.Stash;

internal static class TopItemExclusion{
    /// <summary>Lega medal (Too expensive and I hate to see it always up the top of my lists).</summary>
    internal const string DefaultLegaMedalTemplateId = "6656560053eaaa7a23349c86";

    private static HashSet<string> _templateDenylist;
    private static string          _lastTemplateConfig;
    private static string          _lastCategoryConfig;

    public static bool ShouldExclude(Item item){
        if(item == null) return true;

        if(Settings.FinancialTopItemExcludeSecureContainers.Value && IsSecureContainer(item)) return true;

        RefreshDenylists();

        var templateId = item.TemplateId.ToString();

        if(_templateDenylist.Count > 0 && _templateDenylist.Contains(templateId)) return true;

        return MatchesExcludedCategory(item);
    }

    private static bool IsSecureContainer(Item item){
        return item is MobContainer{
                           isSecured: true
                       };
    }

    private static bool MatchesExcludedCategory(Item item){
        var categories = Settings.FinancialTopItemExcludedCategories.Value;

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

    private static void RefreshDenylists(){
        var templateConfig = Settings.FinancialTopItemExcludedTemplateIds.Value ?? string.Empty;

        if(templateConfig                                    == _lastTemplateConfig
        && Settings.FinancialTopItemExcludedCategories.Value == _lastCategoryConfig)
            return;

        _lastTemplateConfig = templateConfig;
        _lastCategoryConfig = Settings.FinancialTopItemExcludedCategories.Value ?? string.Empty;
        _templateDenylist   = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach(var token in templateConfig.Split(
                                                  [
                                                      ','
                                                  ],
                                                  StringSplitOptions.RemoveEmptyEntries
                                                 ))
            _templateDenylist.Add(token.Trim());
    }
}
