using System.Diagnostics.CodeAnalysis;

namespace Softwyx.CareerLog;

/// <summary>Plugin identity. <see cref="PLUGIN_VERSION"/> is generated in PluginInfo.Version.g.cs on each build.</summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static partial class PluginInfo{
    public const  string PLUGIN_GUID = "com.softwyx.careerlog";
    public const  string PLUGIN_NAME = "Career Log";
    private const string LogPrefix   = "CLG";

    public static string Format(string message){
        return $"[{LogPrefix}] {message}";
    }
}
