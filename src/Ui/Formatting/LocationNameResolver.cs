using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Interop;

namespace Softwyx.CareerLog.Ui.Formatting;

internal static class LocationNameResolver{
    public static string Resolve(string locationId, LocationSettingsClass.Location location = null){
        if(location != null && !string.IsNullOrWhiteSpace(location.LocalizedName)) return location.LocalizedName;

        if(string.IsNullOrEmpty(locationId)) return LocaleLoader.Format(LocaleKeys.LocationUnknown);

        var fromGame = GameLocaleAccess.TryLocalize(locationId);

        if(!string.IsNullOrEmpty(fromGame)) return fromGame;

        var modKey = LocaleKeys.Location(locationId);
        var mod    = LocaleLoader.Format(modKey);

        return string.Equals(mod, modKey, System.StringComparison.Ordinal) ? locationId : mod;
    }
}
