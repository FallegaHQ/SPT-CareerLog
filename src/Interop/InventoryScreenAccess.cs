using EFT;
using EFT.InventoryLogic;
using EFT.UI;
using Softwyx.CareerLog.Infrastructure;

namespace Softwyx.CareerLog.Interop;

/// <summary>Read-only access to the menu inventory screen (character tab) model + controller.</summary>
internal static class InventoryScreenAccess{
    public static InventoryScreen GetInventoryScreen(){
        return !MonoBehaviourSingleton<CommonUI>.Instantiated
                   ? null
                   : MonoBehaviourSingleton<CommonUI>.Instance.InventoryScreen;
    }

    private static InventoryController GetInventoryController(){
        var screen = GetInventoryScreen();

        return !screen
                   ? null
                   : EftScreenFieldBinder.GetField<InventoryController>(
                                                                        screen,
                                                                        GameAssemblyNames.InventoryScreenFields.
                                                                            InventoryController
                                                                       );
    }

    /// <summary>Live menu controller when available; avoids synthetic controllers that break weapons and bottom-field text.</summary>
    public static InventoryController ResolvePreviewController(Profile profile){
        if(profile == null) return null;

        var fromContext = ItemUiContextAccess.GetInventoryController();

        if(fromContext != null){
            if(ItemUiContextAccess.MatchesProfile(fromContext, profile)) return fromContext;

            CareerLogPlugin.Log?.LogWarning(
                                            PluginInfo.Format(
                                                              "Records player model -- ItemUiContext controller profile mismatch; using menu controller anyway."
                                                             )
                                           );

            return fromContext;
        }

        var fromScreen = GetInventoryController();

        if(fromScreen != null) return fromScreen;

        CareerLogPlugin.Log?.LogWarning(
                                        PluginInfo.Format(
                                                          "Records player model -- no menu InventoryController; creating preview controller."
                                                         )
                                       );

        return new InventoryController(profile, false);
    }
}
