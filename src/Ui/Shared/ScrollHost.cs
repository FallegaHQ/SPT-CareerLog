using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Ui.Design;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Shared;

/// <summary>Scroll UI helpers for Records <c>Preview Panel</c> (financial column).</summary>
internal static class ScrollHost{
    public static Transform EnsurePreviewPanelScroll(Transform previewPanel, Transform scrollTemplate = null){
        if(!previewPanel) return null;

        var existing = FindContentUnderParent(previewPanel);

        Transform content;

        if(existing){
            content = existing;
        }
        else{
            scrollTemplate ??= FindStatisticsScrollTemplate();

            content = scrollTemplate
                          ? CloneScrollHierarchy(previewPanel, scrollTemplate)
                          : BuildScrollHierarchy(previewPanel);
        }

        NormalizeScrollContent(content);

        return content;
    }

    public static void SetScrollChainActive(Transform content, bool active){
        if(!content) return;

        var viewport = content.parent;
        var scroll   = viewport ? viewport.parent : null;

        content.gameObject.SetActive(active);

        if(viewport) viewport.gameObject.SetActive(active);

        if(scroll) scroll.gameObject.SetActive(active);
    }

    private static Transform FindStatisticsScrollTemplate(){
        var sessionEnd = GameObject.Find(UiHierarchy.Game.SessionEndUi);

        if(!sessionEnd) return null;

        var screen = sessionEnd.transform.Find(UiHierarchy.Game.SessionResultStatistics);

        return !screen ? null : screen.Find(UiHierarchy.SessionEndStatistics.ScrollView);
    }

    private static Transform CloneScrollHierarchy(Transform parent, Transform templateScrollView){
        var clone = Object.Instantiate(templateScrollView.gameObject, parent);
        clone.name = UiHierarchy.SessionEndStatistics.ScrollView;
        StretchFull(clone.GetComponent<RectTransform>());

        var content = clone.transform.Find(
                                           $"{UiHierarchy.SessionEndStatistics.Viewport}/"
                                         + $"{UiHierarchy.SessionEndStatistics.Content}"
                                          );

        if(content) ScrollContentBuilder.Clear(content);

        ScrollSettings.ApplyClamped(clone.GetComponent<ScrollRect>());
        clone.transform.SetAsLastSibling();

        if(content is RectTransform) NormalizeScrollContent(content);

        return content;
    }

    private static Transform BuildScrollHierarchy(Transform parent){
        var scrollObject = new GameObject(
                                          UiHierarchy.SessionEndStatistics.ScrollView,
                                          typeof(RectTransform),
                                          typeof(ScrollRect)
                                         );
        scrollObject.transform.SetParent(parent, false);
        StretchFull(scrollObject.GetComponent<RectTransform>());

        var scroll = scrollObject.GetComponent<ScrollRect>();
        scroll.horizontal        = false;
        scroll.vertical          = true;
        scroll.movementType      = ScrollRect.MovementType.Clamped;
        scroll.inertia           = false;
        scroll.scrollSensitivity = 16f;

        var viewportObject = new GameObject(
                                            UiHierarchy.SessionEndStatistics.Viewport,
                                            typeof(RectTransform),
                                            typeof(Image),
                                            typeof(Mask)
                                           );
        viewportObject.transform.SetParent(scrollObject.transform, false);
        var viewportRect = viewportObject.GetComponent<RectTransform>();
        StretchFull(viewportRect);

        var viewportImage = viewportObject.GetComponent<Image>();
        viewportImage.color         = Colors.Surface.ScrollViewport;
        viewportImage.raycastTarget = true;

        var mask = viewportObject.GetComponent<Mask>();
        mask.showMaskGraphic = false;

        var contentObject = new GameObject(
                                           UiHierarchy.SessionEndStatistics.Content,
                                           typeof(RectTransform),
                                           typeof(VerticalLayoutGroup),
                                           typeof(ContentSizeFitter)
                                          );
        contentObject.transform.SetParent(viewportObject.transform, false);

        var contentRect   = contentObject.GetComponent<RectTransform>();
        var contentLayout = contentObject.GetComponent<VerticalLayoutGroup>();
        var contentFitter = contentObject.GetComponent<ContentSizeFitter>();
        contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        contentFitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;
        ScrollLayout.ApplyContentLayout(contentRect, contentLayout);

        scroll.viewport = viewportRect;
        scroll.content  = contentRect;

        scrollObject.transform.SetAsLastSibling();

        NormalizeScrollContent(contentObject.transform);

        return contentObject.transform;
    }

    private static void NormalizeScrollContent(Transform content){
        if(content is not RectTransform contentRect) return;

        var layout = content.GetComponent<VerticalLayoutGroup>();
        ScrollLayout.ApplyContentLayout(contentRect, layout);
        ScrollLayout.ApplyScrollContentRect(contentRect);
    }

    private static Transform FindContentUnderParent(Transform parent){
        if(!parent) return null;

        return parent.Find(
                           $"{UiHierarchy.SessionEndStatistics.ScrollView}/"
                         + $"{UiHierarchy.SessionEndStatistics.Viewport}/"
                         + $"{UiHierarchy.SessionEndStatistics.Content}"
                          );
    }

    private static void StretchFull(RectTransform rect){
        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.pivot            = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta        = Vector2.zero;
    }
}
