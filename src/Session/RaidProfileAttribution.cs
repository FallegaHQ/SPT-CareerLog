using EFT;
using Softwyx.CareerLog.Persistence.Models;

namespace Softwyx.CareerLog.Session;

/// <summary>
///     One PMC profile id for the entire game process (set when the client profile loads; never changes until exit).
/// </summary>
internal static class RaidProfileAttribution{
    private static string SessionPmcProfileId{
        get;
        set;
    }

    private static bool IsSessionBound => !string.IsNullOrEmpty(SessionPmcProfileId);

    /// <summary>
    ///     Binds the session PMC id from a non-scav profile (menu load, first PMC raid, etc.).
    ///     Ignored after first bind.
    /// </summary>
    public static void BindSessionProfile(Profile profile){
        if(profile == null || IsScavProfile(profile) || IsSessionBound) return;

        if(string.IsNullOrEmpty(profile.ProfileId)) return;

        SessionPmcProfileId = profile.ProfileId;

        CareerLogPlugin.Log?.LogInfo(PluginInfo.Format($"Session PMC profile bound: {SessionPmcProfileId}."));
    }

    public static RaidPlayedSide GetPlayedSide(Profile profile){
        return profile is{
                             Side: EPlayerSide.Savage
                         }
                   ? RaidPlayedSide.Scav
                   : RaidPlayedSide.Pmc;
    }

    public static bool TryResolveStorageProfileId(Profile activeProfile, out string storageProfileId){
        storageProfileId = null;

        if(activeProfile == null) return false;

        if(!IsScavProfile(activeProfile)){
            BindSessionProfile(activeProfile);
            storageProfileId = SessionPmcProfileId ?? activeProfile.ProfileId;

            return !string.IsNullOrEmpty(storageProfileId);
        }

        if(!IsSessionBound){
            CareerLogPlugin.Log?.LogError(
                                          PluginInfo.Format(
                                                            "Scav raid cannot be saved -- session PMC profile id is not bound yet."
                                                           )
                                         );

            return false;
        }

        storageProfileId = SessionPmcProfileId;

        return true;
    }

    private static bool IsScavProfile(Profile profile){
        return profile.Side == EPlayerSide.Savage;
    }
}
