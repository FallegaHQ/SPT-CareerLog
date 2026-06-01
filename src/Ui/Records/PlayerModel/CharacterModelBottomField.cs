using System.Globalization;
using EFT;
using Softwyx.CareerLog.Interop;
using TMPro;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.PlayerModel;

internal static class CharacterModelBottomField{
    private static readonly NumberFormatInfo ExpNumberFormat = new(){
                                                                        NumberGroupSeparator = " ",
                                                                        NumberDecimalDigits  = 0
                                                                    };

    public static void Bind(Transform modelRoot, Profile profile){
        if(!modelRoot || profile == null) return;

        var bottomField = modelRoot.Find(UiHierarchy.InventoryCharacterTab.BottomField);

        if(!bottomField) return;

        bottomField.gameObject.SetActive(true);

        var nickname   = profile.Nickname ?? string.Empty;
        var experience = FormatExperience(profile);

        foreach(var tmp in bottomField.GetComponentsInChildren<TextMeshProUGUI>(true)){
            if(!tmp) continue;

            var name = tmp.gameObject.name;

            if(IsExperienceValueText(name, tmp.transform)){
                tmp.text = experience;

                continue;
            }

            if(IsNicknameText(name, tmp.transform)) tmp.text = nickname;
        }
    }

    private static string FormatExperience(Profile profile){
        return profile.Experience.ToString("N", ExpNumberFormat);
    }

    private static bool IsNicknameText(string objectName, Transform transform){
        if(objectName.Contains("Nickname", System.StringComparison.OrdinalIgnoreCase)) return true;

        if(objectName.Contains("Exp", System.StringComparison.OrdinalIgnoreCase)) return false;

        for(var node = transform.parent; node; node = node.parent)
            if(string.Equals(
                             node.name,
                             UiHierarchy.InventoryCharacterTab.NicknameAndKarma,
                             System.StringComparison.Ordinal
                            ))
                return true;

        return false;
    }

    private static bool IsExperienceValueText(string objectName, Transform transform){
        if(string.Equals(objectName, UiHierarchy.InventoryCharacterTab.ExpValue, System.StringComparison.Ordinal))
            return true;

        if(!objectName.Contains("Exp", System.StringComparison.OrdinalIgnoreCase)) return false;

        var parent = transform.parent;

        return parent
            && (string.Equals(
                              parent.name,
                              UiHierarchy.InventoryCharacterTab.Experience,
                              System.StringComparison.Ordinal
                             )
             || string.Equals(
                              parent.name,
                              UiHierarchy.InventoryCharacterTab.ExperienceRow,
                              System.StringComparison.Ordinal
                             ));
    }
}
