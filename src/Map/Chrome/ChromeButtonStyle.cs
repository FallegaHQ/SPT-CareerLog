using Softwyx.CareerLog.Ui.Design;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Map.Chrome;

internal static class ChromeButtonStyle{
    public static void Apply(Button button, bool active = false){
        if(!button) return;

        button.transition = Selectable.Transition.ColorTint;

        var colors = button.colors;
        colors.normalColor      = active ? Colors.FilterChip.Active : Colors.Control.ButtonSurface;
        colors.highlightedColor = active ? Colors.FilterChip.ActiveHover : Colors.Control.ButtonHighlight;
        colors.pressedColor     = active ? Colors.FilterChip.ActivePressed : Colors.Control.ButtonPressed;
        colors.selectedColor    = colors.normalColor;
        colors.fadeDuration     = 0.08f;
        button.colors           = colors;
    }
}
