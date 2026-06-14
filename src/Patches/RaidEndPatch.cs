using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

/// <summary>
///     Postfix on raid end -- fallback clean-up when the world is torn down without a player session-end hook.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class RaidEndPatch : ModulePatch{
    private const BindingFlags GameWorldMethodFlags =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly string[] RaidEndMethodNames = ["OnGameSessionEnd", "OnGameEnded", "Dispose"];

    protected override MethodBase GetTargetMethod(){
        foreach(var name in RaidEndMethodNames){
            var method = typeof(GameWorld).GetMethod(name, GameWorldMethodFlags);

            if(method != null) return method;
        }

        CareerLogPlugin.Log?.LogWarning(PluginInfo.Format("RaidEndPatch: no GameWorld end method found."));

        return null;
    }

    [PatchPostfix]
    private static void Postfix(){
        CareerLogSession.OnWorldDisposed();
    }
}
