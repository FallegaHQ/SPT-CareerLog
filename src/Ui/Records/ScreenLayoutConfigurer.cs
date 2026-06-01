using EFT.HandBook;
using EFT.UI;
using Softwyx.CareerLog.Interop;
using UnityEngine;
using Softwyx.CareerLog.Ui.Records.Layout;
using Softwyx.CareerLog.Ui.Records.Menu;
using Softwyx.CareerLog.Ui.Records.PlayerModel;

namespace Softwyx.CareerLog.Ui.Records;

/// <summary>Handbook clone → two-panel Records shell (left stats and player model / right tabs).</summary>
internal static class ScreenLayoutConfigurer{
    public static void Apply(Transform recordsRoot){
        if(!recordsRoot) return;

        CaptionHelper.Apply(recordsRoot);
        HideSearchBar(recordsRoot);
        DisableHandbookLogic(recordsRoot);
        LeftPanelLayout.PrepareShell(recordsRoot);
        CharacterModelPresenter.EnsureInstalled(recordsRoot);
    }

    private static void HideSearchBar(Transform recordsRoot){
        SetChildActive(recordsRoot, UiHierarchy.RecordsScreen.SearchInputField, false);

        var panels     = recordsRoot.Find(UiHierarchy.RecordsScreen.Panels);
        var categories = panels?.Find(UiHierarchy.RecordsScreen.CategoriesPanel);

        if(categories) SetChildActive(categories, UiHierarchy.RecordsScreen.SearchInputField, false);
    }

    private static void DisableHandbookLogic(Transform recordsRoot){
        foreach(var panel in recordsRoot.GetComponentsInChildren<HandbookCategoriesPanel>(true)) panel.enabled = false;

        foreach(var panel in recordsRoot.GetComponentsInChildren<EntitiesPanel>(true)) panel.enabled = false;

        foreach(var panel in recordsRoot.GetComponentsInChildren<BrowseCategoriesPanel>(true)) panel.enabled = false;
    }

    private static void SetChildActive(Transform parent, string childName, bool active){
        if(!parent || string.IsNullOrEmpty(childName)) return;

        var child = parent.Find(childName);

        if(child) child.gameObject.SetActive(active);
    }
}
