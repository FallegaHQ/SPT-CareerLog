using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Design;

/// <summary>Shared ColorTint presets for nav buttons and selectable list rows.</summary>
internal static class ControlButtonColors{
    private const float FadeDuration = 0.08f;

    public static void ApplyNav(Button button, bool enabled){
        if(!button) return;

        var colors = button.colors;
        colors.colorMultiplier = 1f;
        colors.fadeDuration    = FadeDuration;

        if(enabled){
            colors.normalColor      = Colors.Control.ButtonSurface;
            colors.highlightedColor = Colors.Control.ButtonHighlight;
            colors.pressedColor     = Colors.Control.ButtonPressed;
        }
        else{
            colors.normalColor      = Colors.Control.Disabled;
            colors.highlightedColor = Colors.Control.Disabled;
            colors.pressedColor     = Colors.Control.Disabled;
        }

        colors.disabledColor = Colors.Control.Disabled;
        button.colors        = colors;
    }

    public static void ApplyListRow(Button button, bool interactive = true){
        if(!button) return;

        var colors = button.colors;
        colors.colorMultiplier  = 1f;
        colors.fadeDuration     = FadeDuration;
        colors.normalColor      = Colors.ListRow.Idle;
        colors.highlightedColor = interactive ? Colors.ListRow.Hover : Colors.ListRow.Idle;
        colors.pressedColor     = interactive ? Colors.ListRow.Pressed : Colors.ListRow.Idle;
        colors.selectedColor    = Colors.ListRow.Idle;
        colors.disabledColor    = Colors.ListRow.Idle;
        button.colors           = colors;
    }
}
