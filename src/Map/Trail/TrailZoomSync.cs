using System;
using System.Collections.Generic;
using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Map.Panel;
using Softwyx.CareerLog.Map.Viewport;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Trail;

/// <summary>Redraws the trail texture when viewport zoom changes so line thickness stays visually balanced.</summary>
internal sealed class TrailZoomSync : MonoBehaviour{
    private LocationDefinition     _definition;
    private Func<float>            _drawTimeSec;
    private RaidMovementIndex      _index;
    private float                  _lastRedrawScale;
    private float                  _lineWidthBaselineScale = 1f;
    private IReadOnlyList<Vector2> _points;
    private Texture2D              _texture;
    private ViewportZoom           _zoom;

    private void LateUpdate(){
        if(!_zoom || !_texture || _definition == null || _points == null || _index == null) return;

        var scale = _zoom.CombinedScale;

        if(Mathf.Approximately(scale, _lastRedrawScale)) return;

        Redraw();
    }

    public void Bind(ViewportZoom zoom, TrailContext ctx, Func<float> drawTimeSec){
        _zoom                   = zoom;
        _texture                = ctx?.Texture;
        _definition             = ctx?.Definition;
        _points                 = ctx?.TrailPoints;
        _index                  = ctx?.MovementIndex;
        _drawTimeSec            = drawTimeSec;
        _lineWidthBaselineScale = ctx?.LineWidthBaselineScale ?? (zoom ? zoom.CombinedScale : 1f);
        _lastRedrawScale        = -1f;
        Redraw();
    }

    public void Clear(){
        _zoom            = null;
        _texture         = null;
        _definition      = null;
        _points          = null;
        _index           = null;
        _drawTimeSec     = null;
        _lastRedrawScale = -1f;
    }

    private void Redraw(){
        if(!_texture || _definition == null || _points == null || _index == null) return;

        if(_points.Count <= 1) return;

        var seconds = _drawTimeSec?.Invoke() ?? 0f;
        var state   = _index.TrailStateAtTime(seconds);

        if(state.IsEmpty) return;

        var scale     = _zoom ? _zoom.CombinedScale : _lineWidthBaselineScale;
        var lineWidth = TrailTextureDrawer.LineWidthForScale(_lineWidthBaselineScale, scale);

        TrailTextureDrawer.DrawTrail(_texture, _definition, _points, state, lineWidth);
        _lastRedrawScale = scale;
    }
}
