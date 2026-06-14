using System.Collections.Generic;
using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Map.Markers;
using Softwyx.CareerLog.Persistence.Models;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Panel;

internal sealed class TrailContext{
    public LocationDefinition     Definition;
    public float                  LineWidthBaselineScale = 1f;
    public ScreenOverlay          MarkerOverlay;
    public RaidMovementIndex      MovementIndex;
    public PopoverHost            PopoverHost;
    public RaidRecord             Record;
    public Texture2D              Texture;
    public RectTransform          TrailAnchor;
    public IReadOnlyList<Vector2> TrailPoints;
}
