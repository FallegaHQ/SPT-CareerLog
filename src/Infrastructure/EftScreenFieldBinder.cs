using HarmonyLib;
using TMPro;

namespace Softwyx.CareerLog.Infrastructure;

internal static class EftScreenFieldBinder{
    public static TextMeshProUGUI GetTextMeshProUGUI(object instance, string fieldName){
        if(instance == null) return null;

        var field = AccessTools.Field(instance.GetType(), fieldName);

        return field?.GetValue(instance) as TextMeshProUGUI;
    }

    public static TField GetField<TField>(object instance, string fieldName){
        if(instance == null) return default;

        var field = AccessTools.Field(instance.GetType(), fieldName);

        if(field == null) return default;

        return (TField) field.GetValue(instance);
    }
}
