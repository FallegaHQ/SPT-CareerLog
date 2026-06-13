// ReSharper disable MemberCanBePrivate.Global

namespace Softwyx.CareerLog.Interop;

/// <summary>Unity transform paths and runtime object names for mod-built UI (not obfuscated field names).</summary>
internal static class UiHierarchy{
    internal static class Game{
        public const string SessionEndUi            = "Session End UI";
        public const string SessionResultStatistics = "Session Result Statistics";
    }

    internal static class InventoryCharacterTab{
        public const string OverallPanel    = "Overall Panel";
        public const string LeftSide        = "LeftSide";
        public const string CharacterPanel  = "CharacterPanel";
        public const string PlayerModelView = "PlayerModelView";

        public const string ClothingPanel = "ClothingPanel";
        public const string LevelPanel    = "Level Panel";
        public const string LevelText     = "Level";
        public const string LevelIcon     = "Level Icon";

        public const string PlayerMvObject       = "PlayerMVObject";
        public const string MenuPlayer           = "MenuPlayer";
        public const string CameraInventory      = "Camera_inventory";
        public const string PlayerMvObjectLights = "PlayerMVObjectLights";
        public const string BottomField          = "BottomField";
        public const string NicknameAndKarma     = "NicknameAndKarma";
        public const string Experience           = "Experience";
        public const string ExperienceRow        = "ExperienceRow";
        public const string ExpValue             = "ExpValue";
        public const string IconsContainer       = "IconsContainer";
        public const string DragTrigger          = "DragTrigger";
        public const string InlineLevelRow       = "InlineLevelRow";
    }

    /// <summary>RECORDS screen (handbook screen clone) object names.</summary>
    internal static class RecordsScreen{
        public const string ScreenObjectName    = "RecordsScreen";
        public const string CharacterModelClone = "RecordsPlayerModelView";

        public const string Panel            = "Panel";
        public const string Panels           = "Panels";
        public const string PanelsBackground = "Background";
        public const string Caption          = "Caption";
        public const string CaptionLabel     = "Label";
        public const string CaptionIcon      = "Handbook Icon";
        public const string SearchInputField = "SearchInputField";

        public const string CategoriesPanel    = "CategoriesPanel";
        public const string SubcategoriesPanel = "SubcategoriesPanel";
        public const string ContentsArea       = "Preview Panel";

        public const string LeftPanel        = "RecordsLeftPanel";
        public const string RightPanel       = "RecordsRightPanel";
        public const string PlayerModelSlot  = "PlayerModelSlot";
        public const string LeftStatsContent = "LeftStatsContent";
        public const string TabBar           = "TabBar";
        public const string TabContentHost   = "TabContentHost";
    }

    internal static class TabHost{
        public const string FixedHeader = "FixedHeader";
        public const string BackButton  = "BackButton";
        public const string MapButton   = "MapButton";
        public const string TitleText   = "TitleText";
        public const string Label       = "Label";
    }

    internal static class MapModal{
        public const string Overlay     = "MapModalOverlay";
        public const string Dialog      = "MapModalDialog";
        public const string MapViewport = "MapModalViewport";
        public const string CloseButton = "CloseButton";
    }

    internal static class ScrollContent{
        public const string Section = "ScrollSection";
        public const string Items   = "Items";
        public const string Header  = "Header";
        public const string Body    = "Body";
        public const string Row     = "Row";
    }

    internal static class SideFilter{
        public const string RowObjectName = "SideFilterChips";
    }

    internal static class RaidList{
        public const string PaginationBar = "RaidsPagination";
        public const string PageLabel     = "PageLabel";
    }

    internal static class SessionEndStatistics{
        public const string Preview    = "Preview";
        public const string ScrollView = "Scroll View";
        public const string Viewport   = "Viewport";
        public const string Content    = "Content";
    }

    internal static class DefaultButton{
        public const string Background    = "Background";
        public const string SizeLabel     = "SizeLabel";
        public const string IconContainer = "IconContainer";
        public const string Icon          = "Icon";
    }

    /// <summary>PiT Fireteam menu objects (clone source = Character / player button).</summary>
    internal static class PitFireTeam{
        public const string SquadScreenRoot   = "pitFireTeam_SquadControlScreen";
        public const string AddTeammateButton = "pitFireTeam_SquadControlAddTeammateButton";
    }
}
