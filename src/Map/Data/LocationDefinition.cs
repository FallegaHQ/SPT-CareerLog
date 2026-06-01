using System.Collections.Generic;

namespace Softwyx.CareerLog.Map.Data;

internal sealed class LocationDefinition{
    public List<string> LocationIds{
        get;
        set;
    } = [];

    public string SvgFile{
        get;
        set;
    }

    public float CoordinateRotation{
        get;
        set;
    }

    public float TrailOverlayRotation{
        get;
        set;
    } = 180f;

    public float BoundsMinX{
        get;
        set;
    }

    public float BoundsMinY{
        get;
        set;
    }

    public float BoundsMaxX{
        get;
        set;
    }

    public float BoundsMaxY{
        get;
        set;
    }

    public string SvgPath => PluginPaths.MapDisplaySvgFile(SvgFile);
}
