using System;
using System.Collections.Generic;
using System.Linq;

namespace Softwyx.CareerLog.Localization;

/// <summary>
///     Validates all locale JSON packs, keeps a catalogue of valid entries, then merges them into the game locale manager.
/// </summary>
internal static class LocaleLoader{
    private static readonly List<string> AppliedToGame = [];
    private static Dictionary<string, Dictionary<string, string>> _catalogue = new(StringComparer.OrdinalIgnoreCase);

    private static bool IsInitialized => _catalogue.Count > 0;

    /// <summary>
    ///     Discovers every locale file, validates each, keeps only valid packs, then applies them to the game.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static bool PreloadDefaultLocale(out string error){
        error = null;

        if(!TryBuildCatalogue(out error)) return false;

        ApplyValidatedCatalogueToGame();

        return true;
    }

    public static string Format(string key, params object[] args){
        if(string.IsNullOrEmpty(key)) return string.Empty;

        if(!_catalogue.TryGetValue(LocaleFileStore.DefaultLocaleId, out var dict)
        || !dict.TryGetValue(key, out var template))
            template = key;

        if(args == null || args.Length == 0) return template;

        try{
            return string.Format(template, args);
        }
        catch{
            return template;
        }
    }

    public static void LoadLocale(string localeId){
        if(!IsInitialized || string.IsNullOrWhiteSpace(localeId)) return;

        localeId = LocaleFileStore.NormalizeLocaleId(localeId);

        if(AppliedToGame.Contains(localeId)) return;

        if(!_catalogue.TryGetValue(localeId, out var localeDict)) return;

        var manager = LocaleManagerClass.LocaleManagerClass;

        if(!manager.ContainsCulture(localeId)) return;

        manager.UpdateLocales(localeId, CopyDictionary(localeDict));
        AppliedToGame.Add(localeId);

        CareerLogPlugin.Log?.LogInfo(PluginInfo.Format($"Applied locale '{localeId}' ({localeDict.Count} entries)."));
    }

    private static bool TryBuildCatalogue(out string error){
        error      = null;
        _catalogue = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        var discoveredIds = LocaleFileStore.DiscoverLocaleIds();

        if(discoveredIds.Count == 0){
            error = $"No locale files in '{LocaleFileStore.LocalesDirectory}'.";

            return false;
        }

        if(!discoveredIds.Any(
                              id => string.Equals(
                                                  id,
                                                  LocaleFileStore.DefaultLocaleId,
                                                  StringComparison.OrdinalIgnoreCase
                                                 )
                             )){
            error = $"Required '{LocaleFileStore.DefaultLocaleId}.json' is missing from locale packs.";

            return false;
        }

        Dictionary<string, string> english = null;

        foreach(var localeId in discoveredIds){
            if(!LocaleFileStore.TryLoadFile(localeId, out var entries, out var loadError)){
                if(string.Equals(localeId, LocaleFileStore.DefaultLocaleId, StringComparison.OrdinalIgnoreCase)){
                    error = loadError;

                    return false;
                }

                CareerLogPlugin.Log?.LogWarning(
                                                PluginInfo.Format($"Skipping invalid locale '{localeId}': {loadError}")
                                               );

                continue;
            }

            if(entries.Count == 0){
                if(string.Equals(localeId, LocaleFileStore.DefaultLocaleId, StringComparison.OrdinalIgnoreCase)){
                    error = $"Locale '{localeId}' is empty.";

                    return false;
                }

                CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Skipping empty locale '{localeId}'."));

                continue;
            }

            _catalogue[localeId] = entries;

            if(string.Equals(localeId, LocaleFileStore.DefaultLocaleId, StringComparison.OrdinalIgnoreCase))
                english = entries;
        }

        if(english == null){
            error = $"Locale '{LocaleFileStore.DefaultLocaleId}' failed validation.";

            return false;
        }

        foreach(var pair in _catalogue){
            if(string.Equals(pair.Key, LocaleFileStore.DefaultLocaleId, StringComparison.OrdinalIgnoreCase)) continue;

            MergeMissingKeys(pair.Key, pair.Value, english);
        }

        CareerLogPlugin.Log?.LogInfo(
                                     PluginInfo.Format(
                                                       $"Validated locale packs: {string.Join(", ", _catalogue.Keys.OrderBy(id => id, StringComparer.OrdinalIgnoreCase))}."
                                                      )
                                    );

        return true;
    }

    private static void ApplyValidatedCatalogueToGame(){
        AppliedToGame.Clear();

        var manager = LocaleManagerClass.LocaleManagerClass;

        foreach(var pair in _catalogue.OrderBy(static p => p.Key, StringComparer.OrdinalIgnoreCase)){
            manager.UpdateLocales(pair.Key, CopyDictionary(pair.Value));
            AppliedToGame.Add(pair.Key);

            CareerLogPlugin.Log?.LogInfo(
                                         PluginInfo.Format(
                                                           $"Merged {pair.Value.Count} locale entries for '{pair.Key}'."
                                                          )
                                        );
        }
    }

    private static void MergeMissingKeys(
        string localeId, Dictionary<string, string> localeDict, Dictionary<string, string> english
    ){
        foreach(var englishEntry in english){
            if(localeDict.ContainsKey(englishEntry.Key)) continue;

            CareerLogPlugin.Log?.LogWarning(
                                            PluginInfo.Format(
                                                              $"Locale '{localeId}' is missing entry '{englishEntry.Key}'."
                                                             )
                                           );
            localeDict[englishEntry.Key] = englishEntry.Value;
        }
    }

    private static Dictionary<string, string> CopyDictionary(Dictionary<string, string> source){
        return new Dictionary<string, string>(source, StringComparer.OrdinalIgnoreCase);
    }
}
