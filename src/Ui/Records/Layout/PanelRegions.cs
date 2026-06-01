using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Layout;

internal readonly struct PanelRegions(
    Transform leftStatsRoot,
    Transform rightTabHostRoot,
    Transform rightTabContentRoot,
    Transform tabBarRoot
){
    public readonly Transform LeftStatsRoot       = leftStatsRoot;
    public readonly Transform RightTabHostRoot    = rightTabHostRoot;
    public readonly Transform RightTabContentRoot = rightTabContentRoot;
    public readonly Transform TabBarRoot          = tabBarRoot;

    public bool IsValid => LeftStatsRoot && RightTabHostRoot && RightTabContentRoot;
}
