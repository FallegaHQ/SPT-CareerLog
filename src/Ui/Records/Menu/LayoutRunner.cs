using System.Collections;
using EFT;
using EFT.UI;
using Softwyx.CareerLog.Compat;
using Softwyx.CareerLog.Infrastructure;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Menu;

/// <summary>
///     Defers RECORDS layout until PitFireTeam's squad button exists and has been positioned,
///     then re-applies once after Menu Overhaul finishes its stack.
/// </summary>
internal static class LayoutRunner{
    private const int PitFireTeamSettleFrames      = 2;
    private const int OverhaulExtraFrames          = 3;
    private const int InitialFrameDelay            = 4;
    private const int RevealSettleFrames           = 6;
    private const int PitFireTeamPollTimeoutFrames = 8;

    private static Coroutine _layoutCoroutine;

    public static void Schedule(MenuScreen menuScreen, Profile profile){
        if(!menuScreen) return;

        ButtonLayout.PrepareForLayout(menuScreen);

        if(!CareerLogPlugin.Instance){
            ButtonLayout.OnMenuShown(menuScreen, profile);
            ButtonLayout.RevealMenuStack(menuScreen);

            return;
        }

        if(_layoutCoroutine != null) CareerLogPlugin.Instance.StopCoroutine(_layoutCoroutine);

        _layoutCoroutine = CareerLogPlugin.Instance.StartCoroutine(RunLayout(menuScreen, profile));
    }

    private static IEnumerator RunLayout(MenuScreen menuScreen, Profile profile){
        for(var frame = 0; frame < InitialFrameDelay; frame++) yield return null;

        if(!menuScreen){
            _layoutCoroutine = null;

            yield break;
        }

        yield return WaitForPitFireTeamSquadReady(menuScreen);

        if(!menuScreen){
            _layoutCoroutine = null;

            yield break;
        }

        var menuOverhaul = ButtonLayout.IsMenuOverhaulLayoutActive(menuScreen);

        ButtonLayout.OnMenuShown(menuScreen, profile);

        if(menuOverhaul){
            for(var frame = 0; frame < OverhaulExtraFrames; frame++) yield return null;

            if(menuScreen) ButtonLayout.OnMenuShown(menuScreen, profile);
        }

        for(var frame = 0; frame < RevealSettleFrames; frame++) yield return null;

        if(menuScreen) ButtonLayout.RevealMenuStack(menuScreen);

        _layoutCoroutine = null;
    }

    /// <summary>
    ///     Poll until PitFireTeam (re)creates the squad button, then wait a few frames so their layout can finish.
    /// </summary>
    private static IEnumerator WaitForPitFireTeamSquadReady(MenuScreen menuScreen){
        if(!PitFireTeamCompat.IsLoaded) yield break;

        var waited = 0;

        while(menuScreen && !MenuScreenReflection.GetSquadControlButton(menuScreen)){
            waited++;

            if(waited >= PitFireTeamPollTimeoutFrames){
                CareerLogPlugin.Log?.LogWarning(
                                                PluginInfo.Format(
                                                                  "PitFireTeam squad button not found; applying RECORDS layout without squad anchor."
                                                                 )
                                               );

                yield break;
            }

            yield return null;
        }

        if(!menuScreen) yield break;

        CareerLogPlugin.Log?.LogInfo(PluginInfo.Format("PitFireTeam squad button found; waiting for layout settle."));

        for(var frame = 0; frame < PitFireTeamSettleFrames; frame++) yield return null;
    }
}
