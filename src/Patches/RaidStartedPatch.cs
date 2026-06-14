using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

/// <summary>Postfix on <see cref="GameWorld.OnGameStarted" /> -- start raid session buffer.</summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class RaidStartedPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
    }

    [PatchPostfix]
    private static void Postfix(GameWorld __instance){
        if(!Settings.Enabled.Value) return;

        CareerLogSession.OnRaidStarted(__instance);
    }
}
