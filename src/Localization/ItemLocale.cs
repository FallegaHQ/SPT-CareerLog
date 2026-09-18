using EFT;

namespace Softwyx.CareerLog.Localization;

internal static class ItemLocale{
    public static string Name(string templateId){
        return string.IsNullOrEmpty(templateId) ? null : $"{templateId} Name".Localized();
    }

    public static string ShortName(string templateId){
        return string.IsNullOrEmpty(templateId) ? null : $"{templateId} ShortName".Localized();
    }
}
