using EFT;
using EFT.UI.Screens;
using Softwyx.CareerLog.Infrastructure;

namespace Softwyx.CareerLog.Ui.Records;

internal sealed class RecordsScreenController(Profile profile) : RecordsScreenControllerBase{
    public Profile Profile{
        get;
    } = profile;

    public override EEftScreenType ScreenType => ScreenTypes.Records;

    public override bool MainEnvironment => false;

    public override bool KeyScreen => false;

    public override EStateSwitcher ShowEnvironment => EStateSwitcher.Enabled;

    public override EStateSwitcher ShowEnvironmentCamera => EStateSwitcher.Enabled;

    public override EStateSwitcher EnvironmentOverlay => EStateSwitcher.Enabled;

    public override EStateSwitcher TaskBarButtonsAvailability => EStateSwitcher.Enabled;

    public override EStateSwitcher MenuChatBarVisibility => EStateSwitcher.Enabled;
}
