using Softwyx.CareerLog.Persistence.Financial;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal sealed class ChartPanel : MonoBehaviour{
    private RawImage                             _chartImage;
    private Image                                _hoverBand;
    private RectTransform                        _hoverBandRect;
    private ChartTooltip                         _tooltip;
    private ChartHost                            _host;
    private List<ChartPoint>                     _points;
    private ChartStyle                           _style;
    private ChartPlotLayout                      _layout;
    private ScrollContentBuilder.ScrollTextStyle _textStyle;
    private Action<StashSnapshot>                _onSnapshotClicked;
    private int                                  _hoverIndex = -1;

    public static void Install(
        Transform             parent, IReadOnlyList<ChartPoint> points, ScrollContentBuilder.ScrollTextStyle style,
        Action<StashSnapshot> onSnapshotClicked
    ){
        var rootObject = new GameObject("ChartPanel", typeof(RectTransform), typeof(LayoutElement), typeof(ChartPanel));
        rootObject.transform.SetParent(parent, false);

        var layoutElement = rootObject.GetComponent<LayoutElement>();
        layoutElement.minHeight       = 560f;
        layoutElement.preferredHeight = 560f;
        layoutElement.flexibleHeight  = 1f;
        layoutElement.flexibleWidth   = 1f;

        var panel = rootObject.GetComponent<ChartPanel>();
        panel.Bind(points, style, onSnapshotClicked);
    }

    private void Bind(
        IReadOnlyList<ChartPoint> points, ScrollContentBuilder.ScrollTextStyle style,
        Action<StashSnapshot>     onSnapshotClicked
    ){
        _textStyle = style;
        _points = points == null
                      ? []
                      :[
                           ..points
                       ];
        _style             = NavigationState.ChartStyle;
        _onSnapshotClicked = onSnapshotClicked;

        BuildChrome();
        Redraw(-1);
    }

    private void BuildChrome(){
        var root = transform as RectTransform;

        var chartObject = new GameObject("ChartImage", typeof(RectTransform), typeof(RawImage), typeof(ChartHost));
        chartObject.transform.SetParent(root, false);
        Stretch(chartObject.GetComponent<RectTransform>());

        _chartImage               = chartObject.GetComponent<RawImage>();
        _chartImage.color         = Colors.Control.GraphicWhite;
        _chartImage.raycastTarget = false;
        _host                     = chartObject.GetComponent<ChartHost>();

        var hitObject = new GameObject("HitArea", typeof(RectTransform), typeof(Image));
        hitObject.transform.SetParent(root, false);
        Stretch(hitObject.GetComponent<RectTransform>());
        var hitImage = hitObject.GetComponent<Image>();
        hitImage.color         = Colors.Control.DragHitTransparent;
        hitImage.raycastTarget = true;
        hitObject.AddComponent<ChartHitArea>().
                  Bind(this);

        var hoverObject = new GameObject("HoverBand", typeof(RectTransform), typeof(Image));
        hoverObject.transform.SetParent(hitObject.transform, false);
        _hoverBandRect           = hoverObject.GetComponent<RectTransform>();
        _hoverBand               = hoverObject.GetComponent<Image>();
        _hoverBand.color         = new Color(1f, 0.85f, 0.35f, 0.14f);
        _hoverBand.raycastTarget = false;
        _hoverBandRect.gameObject.SetActive(false);

        _tooltip = ChartTooltip.Install(hitObject.transform, _textStyle);
    }

    internal void OnPointerMove(PointerEventData eventData){
        var hitRect = transform.Find("HitArea") as RectTransform;

        if(!hitRect || _points == null || _points.Count == 0) return;

        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                                                    hitRect,
                                                                    eventData.position,
                                                                    eventData.pressEventCamera,
                                                                    out var local
                                                                   ))
            return;

        var index = _layout.HitTestIndex(hitRect, local, _style, _points);

        if(index == _hoverIndex) return;

        SetHoverIndex(index, eventData.position, hitRect);
    }

    internal void OnPointerExit(){
        SetHoverIndex(-1, Vector2.zero, null);
    }

    internal void OnPointerClick(){
        if(_hoverIndex < 0 || _hoverIndex >= _points.Count) return;

        var snapshot = _points[_hoverIndex].Snapshot;

        if(snapshot != null) _onSnapshotClicked?.Invoke(snapshot);
    }

    private void SetHoverIndex(int index, Vector2 screenPosition, RectTransform hitRect){
        _hoverIndex = index;

        if(index < 0 || index >= _points.Count || !hitRect){
            _hoverBandRect.gameObject.SetActive(false);
            _tooltip.Hide();
            Redraw(-1);

            return;
        }

        Redraw(index);

        if(_style == ChartStyle.Bar){
            _hoverBandRect.gameObject.SetActive(false);
        }
        else{
            var bandWidth = ChartPlotLayout.HoverBandWidthLocal;
            var x         = _layout.MapIndexToLocalX(hitRect, index);
            _hoverBandRect.gameObject.SetActive(true);
            _hoverBandRect.anchorMin        = new Vector2(0.5f,      0f);
            _hoverBandRect.anchorMax        = new Vector2(0.5f,      1f);
            _hoverBandRect.pivot            = new Vector2(0.5f,      0.5f);
            _hoverBandRect.sizeDelta        = new Vector2(bandWidth, 0f);
            _hoverBandRect.anchoredPosition = new Vector2(x,         0f);
        }

        _tooltip.Show(_points[index], screenPosition, hitRect);
    }

    private void Redraw(int highlightIndex){
        if(_host?.Texture){
            Destroy(_host.Texture);
            _host.Texture = null;
        }

        var (texture, layout) = ChartTextureDrawer.Draw(_points, _style, highlightIndex);
        _layout               = layout;
        if(_host) _host.Texture = texture;
        _chartImage.texture = texture;
    }

    private static void Stretch(RectTransform rect){
        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.offsetMin        = Vector2.zero;
        rect.offsetMax        = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }
}
