using System;
using EFT;
using EFT.InventoryLogic;
using EFT.UI;
using Softwyx.CareerLog.Infrastructure;

namespace Softwyx.CareerLog.Interop;

/// <summary>Menu <see cref="InventoryController" /> from <see cref="ItemUiContext" /> (initialized at preloader).</summary>
internal static class ItemUiContextAccess{
    public static InventoryController GetInventoryController(){
        var context = ItemUiContext.Instance;

        return !context
                   ? null
                   : EftScreenFieldBinder.GetField<InventoryController>(
                                                                        context,
                                                                        GameAssemblyNames.ItemUiContextFields.
                                                                            InventoryController
                                                                       );
    }

    public static bool MatchesProfile(InventoryController controller, Profile profile){
        if(controller?.Profile == null || profile == null) return false;

        if(controller.Profile is Profile boundProfile)
            return string.Equals(boundProfile.ProfileId, profile.ProfileId, StringComparison.Ordinal);

        return string.Equals(controller.Profile.ProfileId, profile.ProfileId, StringComparison.Ordinal);
    }
}
