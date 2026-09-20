using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT;
using HarmonyLib;
using Softwyx.CareerLog.Localization;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

/// <summary>
///     Merges mod locale JSON when the game applies a UI language
///     (<see cref="LocalizationManager.UpdateApplicationLanguage" />).
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class LocaleApplicationLanguagePatch : ModulePatch{
    /// <summary>Method: <see cref="EFT.LocalizationManager.UpdateApplicationLanguage" /></summary>
    protected override MethodBase GetTargetMethod(){
        return AccessTools.Method(
                                  typeof(LocalizationManager),
                                  nameof(LocalizationManager.UpdateApplicationLanguage)
                                 );
    }

    [PatchPostfix]
    private static void Postfix(LocalizationManager __instance){
        if(__instance == null) return;

        var localeId = __instance.Culture;

        LocaleLoader.LoadLocale(localeId);
    }
}
