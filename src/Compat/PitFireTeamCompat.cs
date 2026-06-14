using BepInEx.Bootstrap;
using EFT.UI;
using Softwyx.CareerLog.Interop;
using UnityEngine;

namespace Softwyx.CareerLog.Compat;

/// <summary>
///     PiT Fireteam clones the menu Character button for squad UI. Career Log's layout fade must not
///     attach <see cref="CanvasGroup" /> to that template or clones inherit non-interactable state.
/// </summary>
internal static class PitFireTeamCompat{
    private const string PluginGuid = "xyz.pit.fireteam";

    public static bool IsLoaded => Chainloader.PluginInfos.ContainsKey(PluginGuid);

    /// <summary>Character button is PiT's Instantiate template -- never fade-hide via CanvasGroup.</summary>
    public static bool ShouldPreserveCharacterButtonTemplate => IsLoaded;

    public static void StripLayoutCanvasGroup(DefaultUIButton characterButton){
        if(!characterButton) return;

        var group = characterButton.GetComponent<CanvasGroup>();

        if(!group) return;

        Object.Destroy(group);
    }

    /// <summary>Reset buttons PiT cloned before Career Log stopped polluting the template.</summary>
    public static void RepairOverlayButtons(MenuScreen menuScreen){
        if(!IsLoaded || !menuScreen) return;

        var screenRoot = menuScreen.transform.Find(UiHierarchy.PitFireTeam.SquadScreenRoot);

        if(!screenRoot) return;

        RepairButtonHierarchy(screenRoot);

        var addTeammate = screenRoot.Find(UiHierarchy.PitFireTeam.AddTeammateButton);

        if(addTeammate) RepairButtonHierarchy(addTeammate);

        CareerLogPlugin.Log?.LogDebug(
                                      PluginInfo.Format("PiT Fireteam squad overlay buttons repaired for interaction.")
                                     );
    }

    private static void RepairButtonHierarchy(Transform root){
        foreach(var button in root.GetComponentsInChildren<DefaultUIButton>(true))
            RestoreButtonInteraction(button.gameObject);
    }

    private static void RestoreButtonInteraction(GameObject root){
        if(!root) return;

        var group = root.GetComponent<CanvasGroup>();

        if(group){
            group.alpha          = 1f;
            group.interactable   = true;
            group.blocksRaycasts = true;
        }

        var button = root.GetComponent<DefaultUIButton>();

        if(button) button.Interactable = true;

        var animated = root.GetComponent<TweenAnimatedButton>();

        if(animated) animated.Interactable = true;
    }
}
