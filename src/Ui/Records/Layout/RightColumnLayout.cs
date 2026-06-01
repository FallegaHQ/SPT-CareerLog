using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Layout;

internal static class RightColumnLayout{
    private const float FixedHeaderHeight = Spacing.SectionHeaderMinHeight;

    internal static Transform EnsureTabBar(Transform rightPanel){
        if(!rightPanel) return null;

        EnsureColumnLayout(rightPanel);

        var tabBar = rightPanel.Find(UiHierarchy.RecordsScreen.TabBar);

        if(!tabBar){
            var tabBarObject = new GameObject(
                                              UiHierarchy.RecordsScreen.TabBar,
                                              typeof(RectTransform),
                                              typeof(HorizontalLayoutGroup),
                                              typeof(LayoutElement)
                                             );
            tabBar = tabBarObject.transform;
            tabBar.SetParent(rightPanel, false);
        }
        else if(!tabBar.GetComponent<LayoutElement>()){
            tabBar.gameObject.AddComponent<LayoutElement>();
        }

        tabBar.SetSiblingIndex(0);

        var le = tabBar.GetComponent<LayoutElement>();
        le.minHeight       = Spacing.TabBarHeight;
        le.preferredHeight = Spacing.TabBarHeight;
        le.flexibleHeight  = 0f;
        le.flexibleWidth   = 1f;

        var layout = tabBar.GetComponent<HorizontalLayoutGroup>();
        layout.spacing                = 6f;
        layout.padding                = new RectOffset(0, 0, 0, 0);
        layout.childAlignment         = TextAnchor.MiddleLeft;
        layout.childControlWidth      = true;
        layout.childControlHeight     = true;
        layout.childForceExpandWidth  = true;
        layout.childForceExpandHeight = true;

        return tabBar;
    }

    internal static Transform EnsureTabScroll(Transform rightPanel, out Transform host){
        if(!rightPanel){
            host = null;

            return null;
        }

        EnsureColumnLayout(rightPanel);

        host = rightPanel.Find(UiHierarchy.RecordsScreen.TabContentHost);

        if(!host){
            var hostObject = new GameObject(
                                            UiHierarchy.RecordsScreen.TabContentHost,
                                            typeof(RectTransform),
                                            typeof(VerticalLayoutGroup),
                                            typeof(LayoutElement)
                                           );
            host = hostObject.transform;
            host.SetParent(rightPanel, false);
        }
        else{
            if(!host.GetComponent<VerticalLayoutGroup>()) host.gameObject.AddComponent<VerticalLayoutGroup>();
            if(!host.GetComponent<LayoutElement>()) host.gameObject.AddComponent<LayoutElement>();
        }

        host.SetSiblingIndex(1);

        var hostLe = host.GetComponent<LayoutElement>();
        hostLe.minHeight       = 0f;
        hostLe.preferredHeight = 0f;
        hostLe.flexibleHeight  = 1f;
        hostLe.flexibleWidth   = 1f;

        var hostVlg = host.GetComponent<VerticalLayoutGroup>();
        hostVlg.spacing                = 0f;
        hostVlg.padding                = new RectOffset(0, 0, 0, 0);
        hostVlg.childAlignment         = TextAnchor.UpperLeft;
        hostVlg.childControlWidth      = true;
        hostVlg.childControlHeight     = true;
        hostVlg.childForceExpandWidth  = true;
        hostVlg.childForceExpandHeight = false;

        EnsureFixedHeader(host);

        var content = ScrollHost.EnsurePreviewPanelScroll(host);
        ConfigureScrollLayoutElement(content);

        return content;
    }

    private static void EnsureColumnLayout(Transform rightPanel){
        if(!rightPanel) return;

        var vlg = rightPanel.GetComponent<VerticalLayoutGroup>();

        if(!vlg) vlg = rightPanel.gameObject.AddComponent<VerticalLayoutGroup>();

        vlg.spacing                = 0f;
        vlg.padding                = new RectOffset(0, 0, 0, 0);
        vlg.childAlignment         = TextAnchor.UpperLeft;
        vlg.childControlWidth      = true;
        vlg.childControlHeight     = true;
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
    }

    private static void EnsureFixedHeader(Transform host){
        if(!host) return;

        var header = host.Find(UiHierarchy.TabHost.FixedHeader);

        if(!header){
            var headerObject = new GameObject(
                                              UiHierarchy.TabHost.FixedHeader,
                                              typeof(RectTransform),
                                              typeof(Image),
                                              typeof(HorizontalLayoutGroup),
                                              typeof(LayoutElement)
                                             );
            header = headerObject.transform;
            header.SetParent(host, false);
            header.SetAsFirstSibling();
        }
        else if(!header.GetComponent<LayoutElement>()){
            header.gameObject.AddComponent<LayoutElement>();
        }

        var le = header.GetComponent<LayoutElement>();
        le.minHeight       = FixedHeaderHeight;
        le.preferredHeight = FixedHeaderHeight;
        le.flexibleHeight  = 0f;
        le.flexibleWidth   = 1f;

        var image = header.GetComponent<Image>();
        image.color         = Colors.Surface.HeaderBackdrop;
        image.raycastTarget = false;

        var layout = header.GetComponent<HorizontalLayoutGroup>();
        layout.spacing                = 10f;
        layout.padding                = new RectOffset(10, 10, 0, 0);
        layout.childAlignment         = TextAnchor.MiddleLeft;
        layout.childControlWidth      = true;
        layout.childControlHeight     = true;
        layout.childForceExpandWidth  = false;
        layout.childForceExpandHeight = true;
    }

    private static void ConfigureScrollLayoutElement(Transform content){
        var scrollGo = content?.parent?.parent;

        if(!scrollGo) return;

        var scrollLe = scrollGo.GetComponent<LayoutElement>();

        if(!scrollLe) scrollLe = scrollGo.gameObject.AddComponent<LayoutElement>();

        scrollLe.minHeight      = 0f;
        scrollLe.flexibleHeight = 1f;
        scrollLe.flexibleWidth  = 1f;

        var scrollRect = scrollGo.GetComponent<ScrollRect>();

        if(!scrollRect) return;

        scrollRect.horizontal = false;
        ScrollSettings.ApplyClamped(scrollRect);
    }
}
