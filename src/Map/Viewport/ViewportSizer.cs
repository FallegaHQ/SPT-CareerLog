using Softwyx.CareerLog.Map.Data;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Viewport;

/// <summary>Re-fits the map when the host viewport receives a real layout size (modal / first frame).</summary>
internal sealed class ViewportSizer : MonoBehaviour{
    private LocationDefinition _definition;
    private RectTransform      _mapRoot;

    private void OnRectTransformDimensionsChange(){
        TryFit();
    }

    public void SetTarget(RectTransform mapRoot, LocationDefinition definition){
        _mapRoot    = mapRoot;
        _definition = definition;
        TryFit();
    }

    private void TryFit(){
        if(!_mapRoot || _definition == null) return;

        var parent = transform as RectTransform;

        if(!parent) return;

        if(!ViewportFitter.TryComputeScale(parent, _definition, out var scale)) return;

        var boundsSize = ViewportFitter.GetBoundsSize(_definition);

        if(boundsSize.x <= 0f || boundsSize.y <= 0f) return;

        var zoom = GetComponent<ViewportZoom>();

        if(zoom){
            _mapRoot.sizeDelta = boundsSize;
            zoom.SetFitScale(scale);

            return;
        }

        ViewportLayout.FitToParent(_mapRoot, _definition, parent);
    }
}
