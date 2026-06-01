using System;
using System.Collections.Generic;
using Softwyx.CareerLog.Persistence.Financial;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal readonly struct ChartPlotLayout(
    int  width,
    int  height,
    int  padLeft,
    int  padRight,
    int  padTop,
    int  padBottom,
    long minWorth,
    long maxWorth,
    int  pointCount
){
    public int Width{
        get;
    } = width;

    public int Height{
        get;
    } = height;

    public int PadLeft{
        get;
    } = padLeft;

    public int PadRight{
        get;
    } = padRight;

    public int PadTop{
        get;
    } = padTop;

    public int PadBottom{
        get;
    } = padBottom;

    private long MinWorth{
        get;
    } = minWorth;

    private long MaxWorth{
        get;
    } = maxWorth;

    public int PointCount{
        get;
    } = pointCount;

    public int PlotWidth{
        get;
    } = width - padLeft - padRight;

    public int PlotHeight{
        get;
    } = height - padTop - padBottom;

    /// <summary>Fraction of plot height reserved above the highest data value.</summary>
    public const float TopHeadroomRatio = 0.10f;

    public static long ComputeDisplayMax(long dataMax){
        if(dataMax <= 0L) return 1L;

        return (long) Math.Ceiling(dataMax / (1.0 - TopHeadroomRatio));
    }

    public float MapIndexToX(int index){
        if(PointCount <= 1) return PadLeft + PlotWidth * 0.5f;

        return PadLeft + PlotWidth * (index / (float) (PointCount - 1));
    }

    public Vector2 MapPoint(ChartPoint point, int index){
        var xRatio = PointCount <= 1 ? 0.5f : index / (float) (PointCount - 1);
        var span   = Math.Max(1L, MaxWorth - MinWorth);
        var yRatio = (point.TotalWorth - MinWorth) / (double) span;

        return new Vector2(PadLeft + PlotWidth * xRatio, PadBottom + PlotHeight * (float) yRatio);
    }

    private float PlotLeftRatio => PadLeft / (float) Width;

    private float PlotWidthRatio => PlotWidth / (float) Width;

    public static float HoverBandWidthLocal => 20f;

    public int HitTestIndex(
        RectTransform hitRect, Vector2 localPoint, ChartStyle style, IReadOnlyList<ChartPoint> points
    ){
        if(PointCount <= 0 || !hitRect || points == null || points.Count == 0) return -1;

        return style == ChartStyle.Bar
                   ? HitTestBarIndex(hitRect, localPoint, points)
                   : HitTestLineIndex(hitRect, localPoint);
    }

    private int HitTestLineIndex(RectTransform hitRect, Vector2 localPoint){
        var halfBand = HoverBandWidthLocal * 0.5f;
        var best     = -1;
        var bestDist = float.MaxValue;

        for(var i = 0; i < PointCount; i++){
            var x    = MapIndexToLocalX(hitRect, i);
            var dist = Mathf.Abs(localPoint.x - x);

            if(dist > halfBand || dist >= bestDist) continue;

            bestDist = dist;
            best     = i;
        }

        return best;
    }

    private int HitTestBarIndex(RectTransform hitRect, Vector2 localPoint, IReadOnlyList<ChartPoint> points){
        for(var i = PointCount - 1; i >= 0; i--)
            if(IsPointInBar(hitRect, localPoint, points[i], i))
                return i;

        return -1;
    }

    private bool IsPointInBar(RectTransform hitRect, Vector2 localPoint, ChartPoint point, int index){
        var centerX = MapIndexToLocalX(hitRect, index);
        var half    = BarWidthLocal(hitRect) * 0.5f;

        if(localPoint.x < centerX - half || localPoint.x > centerX + half) return false;

        var bottom = MapTextureYToLocalY(hitRect, PadBottom);
        var top = MapTextureYToLocalY(
                                      hitRect,
                                      MapPoint(point, index).
                                          y
                                     );

        return localPoint.y >= Mathf.Min(bottom, top) && localPoint.y <= Mathf.Max(bottom, top);
    }

    private float BarWidthLocal(RectTransform hitRect){
        if(!hitRect) return 6f;

        var plotWidthLocal = PlotWidthRatio * hitRect.rect.width;

        return Mathf.Max(4f, plotWidthLocal / Mathf.Max(1, PointCount) * 0.32f);
    }

    private float MapTextureYToLocalY(RectTransform hitRect, float textureY){
        var rect  = hitRect.rect;
        var yNorm = Mathf.Clamp01((textureY - PadBottom) / PlotHeight);

        return Mathf.Lerp(rect.yMin, rect.yMax, yNorm);
    }

    public float MapIndexToLocalX(RectTransform hitRect, int index){
        var rect  = hitRect.rect;
        var ratio = PointCount <= 1 ? 0.5f : index / (float) (PointCount - 1);
        var xNorm = PlotLeftRatio + PlotWidthRatio * ratio;

        return Mathf.Lerp(rect.xMin, rect.xMax, xNorm);
    }
}
