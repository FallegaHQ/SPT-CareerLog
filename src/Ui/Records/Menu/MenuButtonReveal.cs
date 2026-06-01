using System.Collections;
using System.Collections.Generic;
using EFT.UI;
using Softwyx.CareerLog.Infrastructure;
using Softwyx.CareerLog.Interop;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Menu;

/// <summary>
/// Keeps menu stack buttons active (so other mods can reposition them) but invisible until Career Log reveals them.
/// </summary>
internal static class MenuButtonReveal{
    private const float StaggerSeconds = 0.045f;
    private const float FadeDuration   = 0.15f;

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

    public static IEnumerator RevealStaggered(MenuScreen menuScreen, DefaultUIButton recordsButton){
        foreach(var root in CollectStackRoots(menuScreen, recordsButton)){
            if(!menuScreen) yield break;

            yield return RevealOne(root);
            yield return new WaitForSecondsRealtime(StaggerSeconds);
        }
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

        PlayButtonEntrance(root);
        EnsureInteractable(root);
    }

    private static IEnumerator RevealOne(GameObject root){
        if(!root || !root.activeSelf) yield break;

        var group = EnsureCanvasGroup(root);
        PlayButtonEntrance(root);

        var elapsed = 0f;

        while(elapsed < FadeDuration){
            elapsed     += Time.unscaledDeltaTime;
            group.alpha =  Mathf.Clamp01(elapsed / FadeDuration);

            yield return null;
        }

        EnsureInteractable(root);
    }

    private static void PlayButtonEntrance(GameObject root){
        var animation = root.GetComponent<DefaultUIButtonAnimation>();

        if(!animation) return;

        var background = animation.Background;

        if(background){
            var pos = background.anchoredPosition;
            background.anchoredPosition = new Vector2(-Mathf.Max(24f, background.rect.width * 0.35f), pos.y);
        }

        animation.TransitionToState(EButtonAnimationState.Normal);
    }

    private static CanvasGroup EnsureCanvasGroup(GameObject root){
        var group = root.GetComponent<CanvasGroup>();

        return group ? group : root.AddComponent<CanvasGroup>();
    }
}
