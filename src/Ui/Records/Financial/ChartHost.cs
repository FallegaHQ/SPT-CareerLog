using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal sealed class ChartHost : MonoBehaviour{
    // ReSharper disable once InconsistentNaming
    public Texture2D Texture;

    private void OnDestroy(){
        if(Texture) Destroy(Texture);
    }
}
