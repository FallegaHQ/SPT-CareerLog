using EFT;

namespace Softwyx.CareerLog.Interop;

/// <summary>Obfuscated Assembly-CSharp member names. Referenced instead of raw strings in mod logic.</summary>
internal static class GameAssemblyNames{
    internal static class BaseScreenFields{
        public const string ScreenController = "ScreenController";
    }

    internal static class MenuScreenFields{
        public const string PlayButton    = "_playButton";
        public const string PlayerButton  = "_playerButton";
        public const string TradeButton   = "_tradeButton";
        public const string HideoutButton = "_hideoutButton";
        public const string IsMinimized   = "_inGameScreenStatus";
    }

    internal static class MenuScreenMethods{
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

    internal static class HandbookScreenFields{
        public const string BackButton = "_backButton";
    }

    internal static class SessionResultStatisticsFields{
        public const string NextButton   = "_nextButton";
        public const string BackButton   = "_backButton";
        public const string StatsSpawn   = "_statsSpawn";
        public const string LocationName = "_locationName";
    }

    internal static class PostRaidHealthScreenMethods{
        public const string ContinueFromStatistics = "ProceedFromKillList";
        public const string ContinueToKillList     = "CG_get_ExitStatusScreenController";
        public const string ContinueToStatistics   = "ShowStatistics";
    }

    internal static class PostRaidHealthScreenProperties{
        public const string Profile = "SelectedProfile";
    }

    internal static class PostRaidHealthScreenFields{
        public const string Location = "_location";
    }

    internal static class LocaleManagerProperties{
        /// <summary>Requested / selected UI language (e.g. <c>en</c>, <c>ru</c>).</summary>
        public const string SelectedLanguage = "_culture";
    }

    internal static class InventoryScreenFields{
        public const string InventoryController = "inventoryController_0";
    }

    internal static class ItemUiContextFields{
        public const string InventoryController = "inventoryController_0";
    }

    internal static class InventoryPlayerModelWithStatsFields{
        public const string PlayerModelView = "_playerModelView";
        public const string Rotator         = "_rotator";
        public const string DragTrigger     = "_dragTrigger";
    }

    internal static class LocaleManagerMethods{
        public const string UpdateApplicationLanguage = nameof(LocalizationManager.UpdateApplicationLanguage);
    }
}
