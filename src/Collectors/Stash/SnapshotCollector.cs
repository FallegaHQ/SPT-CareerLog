using EFT;
using Softwyx.CareerLog.Config;

namespace Softwyx.CareerLog.Collectors.Stash;

internal static class SnapshotCollector{
    public static void TryCaptureFromMenu(Profile profile){
        if(!Settings.Enabled.Value || !Settings.StashSnapshotOnMenuOpen.Value) return;

        SnapshotAppend.TryAppendFromMenu(profile, false);
    }
}
