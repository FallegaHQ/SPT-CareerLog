using Softwyx.CareerLog.Interop;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Layout;

internal static class PanelDiscovery{
    internal static Transform FindPanelsBackground(Transform recordsRoot){
        return recordsRoot.Find(
                                $"{UiHierarchy.RecordsScreen.Panels}/" + $"{UiHierarchy.RecordsScreen.PanelsBackground}"
                               )
            ?? recordsRoot.Find(UiHierarchy.RecordsScreen.PanelsBackground);
    }

    internal static Transform FindPanel(Transform preferredParent, Transform recordsRoot, string panelName){
        var panel = preferredParent.Find(panelName);

        return panel ? panel : recordsRoot.Find(panelName);
    }

    internal static void SetChildActive(Transform parent, string childName, bool active){
        if(!parent || string.IsNullOrEmpty(childName)) return;

        var child = parent.Find(childName);

        if(child) child.gameObject.SetActive(active);
    }
}
