using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Map.Markers;
using Softwyx.CareerLog.Persistence.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Panel;

internal sealed class TrailContext{
    public Texture2D              Texture;
    public LocationDefinition     Definition;
    public RaidMovementIndex      MovementIndex;
    public IReadOnlyList<Vector2> TrailPoints;
    public RectTransform          TrailAnchor;
    public ScreenOverlay          MarkerOverlay;
    public RaidRecord             Record;
    public PopoverHost            PopoverHost;
    public float                  LineWidthBaselineScale = 1f;
}
