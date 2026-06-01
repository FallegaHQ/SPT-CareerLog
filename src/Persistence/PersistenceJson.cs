using Newtonsoft.Json;
using Softwyx.CareerLog.Config;

namespace Softwyx.CareerLog.Persistence;

internal static class PersistenceJson{
    private static JsonSerializerSettings SerializerSettings => new(){
                                                                         Formatting =
                                                                             Settings.CompressJson.Value
                                                                                 ? Formatting.None
                                                                                 : Formatting.Indented,
                                                                         NullValueHandling = NullValueHandling.Ignore
                                                                     };

    public static string Serialize(object value){
        return JsonConvert.SerializeObject(value, SerializerSettings);
    }

    public static T Deserialize<T>(string json){
        return JsonConvert.DeserializeObject<T>(json, SerializerSettings);
    }
}
