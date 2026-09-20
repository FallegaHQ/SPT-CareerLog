using EFT.UI;
using EFT.UI.Screens;

namespace Softwyx.CareerLog.Interop;

/// <summary>SPT 4.1.x private/internal field and property names. Reference these instead of raw strings in mod logic.</summary>
internal static class GameAssemblyNames{
    internal static class BaseScreenFields{
        /// <summary>Field: <see cref="BaseScreen{TController,TScreen,TType}.ScreenController" /></summary>
        public const string ScreenController = "ScreenController";
    }

    internal static class MenuScreenFields{
        /// <summary>Field: <see cref="EFT.UI.MenuScreen._inGameScreenStatus" /></summary>
        public const string IsMinimized   = "_inGameScreenStatus";
    }

    internal static class MenuScreenMethods{
        /// <summary>Method: <see cref="EFT.UI.MenuScreen.ChangeScreenInGameStatus(bool)" /></summary>
        public const string SetMinimized = "ChangeScreenInGameStatus";
    }

    internal static class MenuScreenHierarchy{
        public const string PlayButton             = "PlayButton";
        public const string CharacterButton        = "CharacterButton";
        public const string TradeButton            = "TradeButton";
        public const string HideoutButton          = "HideoutButton";
        public const string ExitButtonGroup        = "ExitButtonGroup";
        public const string RecordsButton          = "RecordsButton";
        public const string BetaWarningPanel       = "BetaWarningPanel";
        public const string PitFireTeamSquadButton = "pitFireTeam_SquadControlButton";
    }

    internal static class SessionResultStatisticsFields{
        /// <summary>Field: <see cref="EFT.UI.SessionEnd.SessionResultStatistics._nextButton" /></summary>
        public const string NextButton   = "_nextButton";
        /// <summary>Field: <see cref="EFT.UI.SessionEnd.SessionResultStatistics._backButton" /></summary>
        public const string BackButton   = "_backButton";
        /// <summary>Field: <see cref="EFT.UI.SessionEnd.SessionResultStatistics._statsSpawn" /></summary>
        public const string StatsSpawn   = "_statsSpawn";
        /// <summary>Field: <see cref="EFT.UI.SessionEnd.SessionResultStatistics._locationName" /></summary>
        public const string LocationName = "_locationName";
    }

    internal static class PostRaidHealthScreenMethods{
        /// <summary>Method: <see cref="EFT.SessionResultShowOperation.CG_get_ExitStatusScreenController" /></summary>
        public const string ContinueToKillList     = "CG_get_ExitStatusScreenController";
        /// <summary>Method: <see cref="EFT.SessionResultShowOperation.ShowStatistics" /></summary>
        public const string ContinueToStatistics   = "ShowStatistics";
    }

    internal static class InventoryScreenFields{
        /// <summary>Field: <see cref="InventoryScreen._inventoryController" /></summary>
        public const string InventoryController = "_inventoryController";
    }

    internal static class ItemUiContextFields{
        /// <summary>Field: <see cref="ItemUiContext._inventoryController" /></summary>
        public const string InventoryController = "_inventoryController";
    }
}
