using System.IO;

namespace Softwyx.CareerLog;

internal static class PluginPaths{
    private const string AssetsFolderName  = "assets";
    private const string LocalesFolderName = "locales";

    private static string PluginDirectory =>
        Path.GetDirectoryName(CareerLogPlugin.Instance?.Info.Location) ?? string.Empty;

    private static string AssetsDirectory  => Path.Combine(PluginDirectory, AssetsFolderName);
    public static  string LocalesDirectory => Path.Combine(PluginDirectory, LocalesFolderName);

    public static string AssetFile(string fileName){
        return Path.Combine(AssetsDirectory, fileName);
    }

    private static string MapsDirectory => Path.Combine(AssetsDirectory, "maps");

    public static string MapDefinitionsDirectory => Path.Combine(MapsDirectory, "defs");

    private static string MapSvgFile(string fileName){
        return Path.Combine(MapsDirectory, fileName);
    }

    /// <summary>Processed ground-layer SVG from <c>scripts/maps</c> pipeline; falls back to full source SVG.</summary>
    public static string MapDisplaySvgFile(string fileName){
        if(string.IsNullOrEmpty(fileName)) return MapSvgFile(fileName);

        var debriefPath = Path.Combine(MapsDirectory, "debrief", fileName);

        return File.Exists(debriefPath) ? debriefPath : MapSvgFile(fileName);
    }

    private static string MapMarkersDirectory => Path.Combine(MapsDirectory, "markers");

    public static string MapMarkerFile(string fileName){
        return Path.Combine(MapMarkersDirectory, fileName);
    }
}
