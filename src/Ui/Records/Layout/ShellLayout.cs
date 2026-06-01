using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Ui.Design;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Layout;

internal static class ShellLayout{
    internal static Transform EnsurePanelsBackgroundShell(Transform recordsRoot){
        var panels = recordsRoot.Find(UiHierarchy.RecordsScreen.Panels);

        if(!panels) return CreateFallbackPanelsContainer(recordsRoot);

        panels.gameObject.SetActive(true);

        var background = panels.Find(UiHierarchy.RecordsScreen.PanelsBackground);

        if(!background) return CreateFallbackPanelsContainer(recordsRoot);

        background.gameObject.SetActive(true);

        var image = background.GetComponent<Image>();

        if(!image) return background;

        image.enabled       = true;
        image.raycastTarget = false;

        return background;
    }

    internal static void HideLegacyHandbookChrome(Transform recordsRoot){
        PanelDiscovery.SetChildActive(recordsRoot, UiHierarchy.RecordsScreen.Panel,            false);
        PanelDiscovery.SetChildActive(recordsRoot, UiHierarchy.RecordsScreen.SearchInputField, false);

        var panels = recordsRoot.Find(UiHierarchy.RecordsScreen.Panels);

        if(!panels) return;

        panels.gameObject.SetActive(true);

        PanelDiscovery.SetChildActive(panels, UiHierarchy.RecordsScreen.CategoriesPanel,    false);
        PanelDiscovery.SetChildActive(panels, UiHierarchy.RecordsScreen.SubcategoriesPanel, false);
        PanelDiscovery.SetChildActive(panels, UiHierarchy.RecordsScreen.ContentsArea,       false);
    }

    private static Transform CreateFallbackPanelsContainer(Transform recordsRoot){
        var container = recordsRoot.Find(UiHierarchy.RecordsScreen.PanelsBackground);

        if(!container){
            var containerObject = new GameObject(
                                                 UiHierarchy.RecordsScreen.PanelsBackground,
                                                 typeof(RectTransform),
                                                 typeof(Image)
                                                );
            container = containerObject.transform;
            container.SetParent(recordsRoot, false);
        }

        var rect = container as RectTransform;

        if(rect){
            rect.anchorMin        = new Vector2(0f,   0f);
            rect.anchorMax        = new Vector2(1f,   1f);
            rect.pivot            = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.offsetMin        = new Vector2(Spacing.SideMargin,  Spacing.RecordsPanelsTopInset);
            rect.offsetMax        = new Vector2(-Spacing.SideMargin, -Spacing.RecordsPanelsBottomInset);
        }

        var image = container.GetComponent<Image>();
        image.color         = Colors.Surface.PanelBackground;
        image.raycastTarget = false;

        return container;
    }
}
