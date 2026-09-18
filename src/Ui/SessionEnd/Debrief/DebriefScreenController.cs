using EFT.UI.Screens;
using JsonType;
using Softwyx.CareerLog.Infrastructure;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Ui.SessionEnd.Debrief;

internal sealed class DebriefScreenController(LocationSettings.Location location, RaidRecord raidRecord)
    : DebriefScreenControllerBase{
    public LocationSettings.Location Location{
        get;
    } = location;

    public RaidRecord RaidRecord{
        get;
    } = raidRecord;

    public override EEftScreenType ScreenType => ScreenTypes.Debrief;

    public override bool MainEnvironment => false;

    public override bool KeyScreen => true;

    public override EStateSwitcher TaskBarButtonsAvailability => EStateSwitcher.Disabled;

    public override EStateSwitcher MenuChatBarVisibility => EStateSwitcher.Disabled;
}
