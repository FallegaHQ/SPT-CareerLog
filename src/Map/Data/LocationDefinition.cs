using System.Collections.Generic;

namespace Softwyx.CareerLog.Map.Data;

internal sealed class LocationDefinition(
    string svgFile,
    float  coordinateRotation,
    float  boundsMinX,
    float  boundsMinY,
    float  boundsMaxX,
    float  boundsMaxY
){
    public List<string> LocationIds{
        get;
    } = [];

    private string SvgFile{
        get;
    } = svgFile;

    public float CoordinateRotation{
        get;
    } = coordinateRotation;

    public static float TrailOverlayRotation => 180f;

    public float BoundsMinX{
        get;
    } = boundsMinX;

    public float BoundsMinY{
        get;
    } = boundsMinY;

    public float BoundsMaxX{
        get;
    } = boundsMaxX;

    public float BoundsMaxY{
        get;
    } = boundsMaxY;

    public string SvgPath => PluginPaths.MapDisplaySvgFile(SvgFile);
}
