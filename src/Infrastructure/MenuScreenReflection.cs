using EFT.UI;
using HarmonyLib;
using Softwyx.CareerLog.Interop;
using UnityEngine;

namespace Softwyx.CareerLog.Infrastructure;

/// <summary>Cached Harmony access for <see cref="MenuScreen"/> UI types and hierarchy.</summary>
internal static class MenuScreenReflection{
    public static MenuScreenController GetController(MenuScreen menuScreen){
        if(!menuScreen) return null;

        return Traverse.Create(menuScreen).
                        Field(GameAssemblyNames.BaseScreenFields.ScreenController).
                        GetValue<MenuScreenController>();
    }

    public static bool IsMinimized(MenuScreen menuScreen){
        return EftScreenFieldBinder.GetField<bool>(menuScreen, GameAssemblyNames.MenuScreenFields.IsMinimized);
    }

    public static DefaultUIButton GetPlayerButton(MenuScreen menuScreen){
        return EftScreenFieldBinder.GetField<DefaultUIButton>(
                                                              menuScreen,
                                                              GameAssemblyNames.MenuScreenFields.PlayerButton
                                                             );
    }

    public static DefaultUIButton GetTradeButton(MenuScreen menuScreen){
        return EftScreenFieldBinder.GetField<DefaultUIButton>(
                                                              menuScreen,
                                                              GameAssemblyNames.MenuScreenFields.TradeButton
                                                             );
    }

    public static DefaultUIButton GetHideoutButton(MenuScreen menuScreen){
        return EftScreenFieldBinder.GetField<DefaultUIButton>(
                                                              menuScreen,
                                                              GameAssemblyNames.MenuScreenFields.HideoutButton
                                                             );
    }

    public static DefaultUIButton GetPlayButton(MenuScreen menuScreen){
        if(!menuScreen) return null;

        var hierarchyButton = menuScreen.transform.
                                         Find(GameAssemblyNames.MenuScreenHierarchy.PlayButton)?.
                                         GetComponent<DefaultUIButton>();

        return hierarchyButton
            ?? EftScreenFieldBinder.GetField<DefaultUIButton>(
                                                              menuScreen,
                                                              GameAssemblyNames.MenuScreenFields.PlayButton
                                                             );
    }

    public static DefaultUIButton GetCharacterButton(MenuScreen menuScreen){
        if(!menuScreen) return null;

        var hierarchyButton = menuScreen.transform.
                                         Find(GameAssemblyNames.MenuScreenHierarchy.CharacterButton)?.
                                         GetComponent<DefaultUIButton>();

        return hierarchyButton ?? GetPlayerButton(menuScreen);
    }

    public static DefaultUIButton GetSquadControlButton(MenuScreen menuScreen){
        var playerButton = GetPlayerButton(menuScreen);

        if(!playerButton?.transform.parent) return null;

        return playerButton.transform.
                            parent.
                            Find(GameAssemblyNames.MenuScreenHierarchy.PitFireTeamSquadButton)?.
                            GetComponent<DefaultUIButton>();
    }

    public static RectTransform GetHierarchyButtonRect(MenuScreen menuScreen, string hierarchyName){
        return menuScreen?.transform.
                           Find(hierarchyName)?.
                           GetComponent<RectTransform>();
    }
}
