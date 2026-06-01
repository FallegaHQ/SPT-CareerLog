using System.Diagnostics.CodeAnalysis;
using EFT.UI;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Ui.Records.Menu;
using SPT.Reflection.Patching;
using System.Reflection;
using Softwyx.CareerLog.Interop;

namespace Softwyx.CareerLog.Patches;

/// <summary>Hide the RECORDS button when the menu is minimized (in-raid pause).</summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class MenuScreenMinimizePatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(MenuScreen).GetMethod(
                                            GameAssemblyNames.MenuScreenMethods.SetMinimized,
                                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                           );
    }

    [PatchPostfix]
    private static void Postfix(MenuScreen __instance){
        if(!Settings.Enabled.Value || !Settings.ShowRecordsMenuButton.Value) return;

        ButtonLayout.SyncVisibility(__instance);
    }
}
