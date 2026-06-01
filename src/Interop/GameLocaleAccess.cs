using System;

namespace Softwyx.CareerLog.Interop;

/// <summary>Read labels from the merged game locale (<see cref="LocaleManagerClass"/>).</summary>
internal static class GameLocaleAccess{
    public static string TryLocalize(string key){
        if(string.IsNullOrEmpty(key)) return null;

        try{
            var localized = key.Localized();

            if(string.IsNullOrEmpty(localized)) return null;

            return string.Equals(localized, key, StringComparison.OrdinalIgnoreCase) ? null : localized;
        }
        catch(Exception ex){
            CareerLogPlugin.Log?.LogDebug(PluginInfo.Format($"Game locale lookup failed for '{key}': {ex.Message}"));

            return null;
        }
    }

    public static string TryLocalize(params string[] keys){
        if(keys == null) return null;

        foreach(var key in keys){
            if(string.IsNullOrEmpty(key)) continue;

            var localized = TryLocalize(key);

            if(!string.IsNullOrEmpty(localized)) return localized;
        }

        return null;
    }
}
