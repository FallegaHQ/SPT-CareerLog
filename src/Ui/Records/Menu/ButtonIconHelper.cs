using EFT.UI;
using Softwyx.CareerLog.Interop;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Menu;

/// <summary>Applies menu button icons.</summary>
internal static class ButtonIconHelper{
    public static void Apply(DefaultUIButton button, bool menuOverhaulLayout){
        if(!button) return;

        var sprite = MenuIcon.Get(menuOverhaulLayout);

        if(!sprite) return;

        button.SetIcon(sprite);

        if(!menuOverhaulLayout) return;

        SetImageSprite(button.gameObject, sprite);

        var iconTransform = button.transform.Find(
                                                  $"{UiHierarchy.DefaultButton.SizeLabel}/{UiHierarchy.DefaultButton.IconContainer}/{UiHierarchy.DefaultButton.Icon}"
                                                 );

        if(iconTransform) SetImageSprite(iconTransform.gameObject, sprite);
    }

    private static void SetImageSprite(GameObject target, Sprite sprite){
        if(!target) return;

        var image = target.GetComponent<Image>();

        if(!image) return;

        image.sprite         = sprite;
        image.overrideSprite = sprite;
    }
}
