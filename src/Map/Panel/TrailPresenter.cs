using System.Collections.Generic;
using Softwyx.CareerLog.Map.Markers;
using Softwyx.CareerLog.Map.Trail;
using Softwyx.CareerLog.Map.Viewport;
using Softwyx.CareerLog.Persistence.Models;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Panel;

internal sealed class TrailPresenter(MonoBehaviour host){
    private List<RaidMovementValue> _drawnMarkers;
    private Coroutine               _routine;

    internal float DrawTimeSec{
        get;
        private set;
    } = -1f;

    public void Stop(){
        if(_routine == null || !host) return;

        host.StopCoroutine(_routine);
        _routine = null;
    }

    public void ResetPlaybackDraw(TrailContext ctx){
        _drawnMarkers = null;
        DrawTimeSec   = -1f;

        if(ctx?.Texture) TrailTextureDrawer.Clear(ctx.Texture);
    }

    public void ApplyFrame(
        TrailContext ctx, ViewportZoom zoom, float seconds, IReadOnlyList<RaidMovementValue> markers
    ){
        DrawTimeSec = seconds;

        if(!CanDraw(ctx)) return;

        if(ctx.TrailPoints == null || ctx.TrailPoints.Count == 0 || ctx.MovementIndex == null){
            TrailTextureDrawer.Clear(ctx.Texture);
            PresentMarkersIfChanged(ctx, markers);

            return;
        }

        var state = ctx.MovementIndex.TrailStateAtTime(seconds);

        if(state.IsEmpty){
            TrailTextureDrawer.Clear(ctx.Texture);
            PresentMarkersIfChanged(ctx, markers);

            return;
        }

        var currentScale = zoom ? zoom.CombinedScale : 1f;
        var lineWidth    = TrailTextureDrawer.LineWidthForScale(ctx.LineWidthBaselineScale, currentScale);

        TrailTextureDrawer.DrawTrail(ctx.Texture, ctx.Definition, ctx.TrailPoints, state, lineWidth);
        PresentMarkersIfChanged(ctx, markers);
    }

    private void PresentMarkersIfChanged(TrailContext ctx, IReadOnlyList<RaidMovementValue> markers){
        if(MarkersUnchanged(markers)) return;

        PresentMarkers(ctx, markers);
        _drawnMarkers = markers == null
                            ? null
                            :[
                                 ..markers
                             ];
    }

    private bool MarkersUnchanged(IReadOnlyList<RaidMovementValue> markers){
        if(markers == null || markers.Count == 0) return _drawnMarkers == null || _drawnMarkers.Count == 0;

        if(_drawnMarkers == null || _drawnMarkers.Count != markers.Count) return false;

        for(var i = 0; i < markers.Count; i++)
            if(!ReferenceEquals(_drawnMarkers[i], markers[i]))
                return false;

        return true;
    }

    private static bool CanDraw(TrailContext ctx){
        return ctx != null && ctx.Texture && ctx.Definition != null && ctx.TrailAnchor && ctx.MarkerOverlay;
    }

    private static void PresentMarkers(TrailContext ctx, IReadOnlyList<RaidMovementValue> markers){
        Overlay.Rebuild(ctx.MarkerOverlay, ctx.TrailAnchor, markers, ctx.Definition, ctx.Record, ctx.PopoverHost);
    }
}
