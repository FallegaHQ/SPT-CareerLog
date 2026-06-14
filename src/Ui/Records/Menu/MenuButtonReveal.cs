using System.Collections.Generic;
using EFT.UI;
using Softwyx.CareerLog.Compat;
using Softwyx.CareerLog.Infrastructure;
using Softwyx.CareerLog.Interop;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Menu;

/// <summary>
///     Keeps menu stack buttons active (so other mods can reposition them) but invisible until Career Log reveals them.
/// </summary>
internal static class MenuButtonReveal{
    public static void HideStack(MenuScreen menuScreen, DefaultUIButton recordsButton){
        if(!menuScreen) return;

        foreach(var root in CollectStackRoots(menuScreen, recordsButton)) SetHidden(root);
    }

    public static void HideRoot(GameObject root){
        SetHidden(root);
    }

    public static void RevealImmediate(MenuScreen menuScreen, DefaultUIButton recordsButton){
        foreach(var root in CollectStackRoots(menuScreen, recordsButton)) SetVisibleImmediate(root);
    }

    public static void FinalizeStack(MenuScreen menuScreen, DefaultUIButton recordsButton){
        foreach(var root in CollectStackRoots(menuScreen, recordsButton)) EnsureInteractable(root);
    }

    public static void EnsureInteractable(GameObject root){
        if(!root || !root.activeSelf) return;

        var group = root.GetComponent<CanvasGroup>();

        if(group){
            group.alpha          = 1f;
            group.interactable   = true;
            group.blocksRaycasts = true;
        }

        var button = root.GetComponent<DefaultUIButton>();

        if(!button) return;

        button.Interactable = true;

        var animated = root.GetComponent<TweenAnimatedButton>();

        if(animated) animated.Interactable = true;
    }

    private static IEnumerable<GameObject> CollectStackRoots(MenuScreen menuScreen, DefaultUIButton recordsButton){
        yield return RootOf(MenuScreenReflection.GetPlayButton(menuScreen));

        if(!PitFireTeamCompat.ShouldPreserveCharacterButtonTemplate)
            yield return RootOf(MenuScreenReflection.GetCharacterButton(menuScreen));

        yield return RootOf(MenuScreenReflection.GetSquadControlButton(menuScreen));
        yield return RootOf(recordsButton);
        yield return RootOf(MenuScreenReflection.GetTradeButton(menuScreen));
        yield return RootOf(MenuScreenReflection.GetHideoutButton(menuScreen));
        yield return HierarchyRoot(menuScreen, GameAssemblyNames.MenuScreenHierarchy.ExitButtonGroup);
        yield return HierarchyRoot(menuScreen, GameAssemblyNames.MenuScreenHierarchy.BetaWarningPanel);
    }

    private static GameObject RootOf(DefaultUIButton button){
        return button ? button.gameObject : null;
    }

    private static GameObject HierarchyRoot(MenuScreen menuScreen, string hierarchyName){
        var transform = menuScreen?.transform.Find(hierarchyName);

        return transform ? transform.gameObject : null;
    }

    private static void SetHidden(GameObject root){
        if(!root || !root.activeSelf) return;

        var group = EnsureCanvasGroup(root);
        group.alpha          = 0f;
        group.interactable   = false;
        group.blocksRaycasts = false;
    }

    private static void SetVisibleImmediate(GameObject root){
        if(!root || !root.activeSelf) return;

        EnsureInteractable(root);
    }

    private static CanvasGroup EnsureCanvasGroup(GameObject root){
        var group = root.GetComponent<CanvasGroup>();

        return group ? group : root.AddComponent<CanvasGroup>();
    }
}
