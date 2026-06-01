using EFT.UI;
using TMPro;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Shared;

/// <summary>
/// Attaches <see cref="LocalizedText"/> so copy updates when the game locale changes
/// (keys merged via <c>LocaleUpdatePatch</c>).
/// </summary>
internal static class LocalizedTextView{
    public static void BindKey(Transform textRoot, string localizationKey, string labelChildName = "Label"){
        if(!textRoot || string.IsNullOrEmpty(localizationKey)) return;

        var target = string.IsNullOrEmpty(labelChildName) ? textRoot : textRoot.Find(labelChildName) ?? textRoot;

        EnsureLocalizedText(target, localizationKey);
    }

    private static void EnsureLocalizedText(Transform target, string localizationKey){
        if(!target || string.IsNullOrEmpty(localizationKey)) return;

        EnsureTmp(target);

        var localizedText = target.GetComponent<LocalizedText>() ?? target.gameObject.AddComponent<LocalizedText>();

        localizedText.enabled         = true;
        localizedText.LocalizationKey = localizationKey;
    }

    private static void EnsureTmp(Transform target){
        if(target.GetComponent<TextMeshProUGUI>() || target.GetComponent<TMP_Text>()) return;

        target.gameObject.AddComponent<TextMeshProUGUI>();
    }
}
