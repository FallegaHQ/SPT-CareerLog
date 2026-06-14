using Softwyx.CareerLog.Interop;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.SessionEnd;

/// <summary>
///     Post-raid session-end screens: use the cloned vanilla <c>Scroll View</c>.
/// </summary>
internal static class SessionEndScrollPaths{
    private static Transform FindScrollViewport(Transform screenRoot){
        if(!screenRoot) return null;

        return screenRoot.Find(
                               $"{UiHierarchy.SessionEndStatistics.ScrollView}/"
                             + $"{UiHierarchy.SessionEndStatistics.Viewport}"
                              );
    }

    public static Transform FindScrollContent(Transform screenRoot){
        var viewport = FindScrollViewport(screenRoot);

        return viewport ? viewport.Find(UiHierarchy.SessionEndStatistics.Content) : null;
    }

    public static void HideBlockingPreview(Transform screenRoot){
        if(!screenRoot) return;

        var preview = screenRoot.Find(UiHierarchy.SessionEndStatistics.Preview);

        if(preview) preview.gameObject.SetActive(false);
    }

    public static void SetScrollChainActive(Transform content, bool active){
        if(!content) return;

        var viewport = content.parent;
        var scroll   = viewport ? viewport.parent : null;

        content.gameObject.SetActive(active);

        if(viewport) viewport.gameObject.SetActive(active);

        if(scroll) scroll.gameObject.SetActive(active);
    }

    public static void BringScrollToFront(Transform content){
        if(!content) return;

        var scroll = content.parent?.parent;

        if(scroll) scroll.SetAsLastSibling();
    }
}
