using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Softwyx.CareerLog.Map.Data;

internal static class LocationRegistry{
    private static readonly Dictionary<string, LocationDefinition> ByLocationId = new(StringComparer.OrdinalIgnoreCase);

    private static bool _loaded;

    public static bool TryGet(string locationId, out LocationDefinition definition){
        EnsureLoaded();

        if(!string.IsNullOrEmpty(locationId)) return ByLocationId.TryGetValue(locationId, out definition);

        definition = null;

        return false;
    }

    public static bool HasMap(string locationId){
        return TryGet(locationId, out _);
    }

    private static void EnsureLoaded(){
        if(_loaded) return;

        _loaded = true;

        var defsDir = PluginPaths.MapDefinitionsDirectory;

        if(!Directory.Exists(defsDir)){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Map defs folder missing: {defsDir}"));

            return;
        }

        foreach(var file in Directory.GetFiles(defsDir, "*.json"))
            try{
                var json = File.ReadAllText(file);
                var def  = JsonConvert.DeserializeObject<LocationDefinition>(json);

                if(def?.LocationIds == null || def.LocationIds.Count == 0) continue;

                foreach(var id in def.LocationIds){
                    if(string.IsNullOrEmpty(id)) continue;

                    ByLocationId[id] = def;
                }
            }
            catch(Exception ex){
                CareerLogPlugin.Log?.LogWarning(PluginInfo.Format($"Failed to load map def {file}: {ex.Message}"));
            }

        CareerLogPlugin.Log?.LogInfo(PluginInfo.Format($"Loaded {ByLocationId.Count} location map bindings."));
    }
}
