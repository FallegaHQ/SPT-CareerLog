using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Persistence.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Markers;

/// <summary>Facade for <see cref="ScreenOverlay"/> on the map viewport.</summary>
internal static class Overlay{
    public static void Rebuild(
        ScreenOverlay      overlay,    RectTransform trailAnchor, IReadOnlyList<RaidMovementValue> markers,
        LocationDefinition definition, RaidRecord    raid,        PopoverHost                      popoverHost
    ){
        if(!overlay || !trailAnchor) return;

        overlay.Bind(trailAnchor, definition);
        overlay.SetPopoverHost(popoverHost, raid);
        overlay.Rebuild(markers);
    }
}
