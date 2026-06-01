using System.Diagnostics.CodeAnalysis;
using EFT;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;
using System.Reflection;

namespace Softwyx.CareerLog.Patches;

/// <summary>
/// Postfix on <see cref="Player.OnGameSessionEnd"/> -- snapshot session counters and persist raid JSON.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class PlayerRaidEndPatch : ModulePatch{
    private const BindingFlags PlayerMethodFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    protected override MethodBase GetTargetMethod(){
        return typeof(Player).GetMethod(nameof(Player.OnGameSessionEnd), PlayerMethodFlags);
    }

    [PatchPostfix]
    private static void Postfix(Player __instance, ExitStatus exitStatus, float pastTime, string locationId){
        if(!__instance || !__instance.IsYourPlayer) return;

        CareerLogSession.OnRaidEnded(__instance.Profile, __instance, exitStatus, pastTime, locationId);
    }
}
