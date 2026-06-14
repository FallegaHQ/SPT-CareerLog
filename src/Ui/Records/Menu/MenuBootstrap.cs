using EFT;
using EFT.UI;
using Softwyx.CareerLog.Collectors.Stash;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Session;

namespace Softwyx.CareerLog.Ui.Records.Menu;

/// <summary>Single entry point for menu-time RECORDS wiring (patches call these only).</summary>
internal static class MenuBootstrap{
    /// <summary>Early register when <c>CommonUI</c> awakes (handbook template available).</summary>
    public static void OnCommonUiReady(CommonUI commonUi){
        if(!ShouldRunRecordsUi()) return;

        // Idempotent -- safe if MenuScreen.Awake also registers.
        ScreenBootstrap.TryRegister(commonUi);
    }

    /// <summary>Fallback register when menu prefab awakes before CommonUI path.</summary>
    public static void OnMenuScreenAwake(){
        if(!ShouldRunRecordsUi()) return;

        ScreenBootstrap.EnsureRegistered();
    }

    /// <summary>Menu shown -- bind profile and schedule button layout (after other Show postfixes).</summary>
    public static void OnMenuShown(MenuScreen menuScreen, Profile profile){
        if(!Settings.Enabled.Value) return;

        RaidProfileAttribution.BindSessionProfile(profile);

        if(Settings.StashSnapshotOnMenuOpen.Value) SnapshotCollector.TryCaptureFromMenu(profile);

        if(!ShouldRunRecordsUi()) return;

        ScreenBootstrap.EnsureRegistered();
        LayoutRunner.Schedule(menuScreen, profile);
    }

    /// <summary>Controller-driven show -- layout only; profile already bound on Show.</summary>
    public static void OnMenuShowAction(MenuScreen menuScreen){
        if(!Settings.Enabled.Value) return;

        RaidProfileAttribution.BindSessionProfile(ProfileResolver.FromMenuScreen(menuScreen));

        if(!ShouldRunRecordsUi()) return;

        LayoutRunner.Schedule(menuScreen, ProfileResolver.FromMenuScreen(menuScreen));
    }

    private static bool ShouldRunRecordsUi(){
        return Settings.Enabled.Value && Settings.ShowRecordsMenuButton.Value;
    }
}
