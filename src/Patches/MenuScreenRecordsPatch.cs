using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT.UI;
using HarmonyLib;
using Softwyx.CareerLog.Ui.Records.Menu;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

/// <summary>Register Records screen on menu prefab awake; button layout runs on Show only.</summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class MenuScreenRecordsPatch : ModulePatch{
    /// <summary>Method: <see cref="EFT.UI.MenuScreen.Awake" /></summary>
    protected override MethodBase GetTargetMethod(){
        return AccessTools.Method(typeof(MenuScreen), nameof(MenuScreen.Awake));
    }

    [PatchPostfix]
    private static void Postfix(MenuScreen __instance){
        MenuBootstrap.OnMenuScreenAwake();
    }
}
