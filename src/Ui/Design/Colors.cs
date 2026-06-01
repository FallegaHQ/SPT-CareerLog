using UnityEngine;

namespace Softwyx.CareerLog.Ui.Design;

/// <summary>Shared UI color tokens.</summary>
internal static class Colors{
    internal static class Text{
        public static readonly Color SectionTitle  = new(0.906f, 0.894f, 0.820f, 1f);
        public static readonly Color SectionAccent = new(0.920f, 0.875f, 0.700f, 1f);
        public static readonly Color SubHeader     = new(0.820f, 0.780f, 0.620f, 1f);
        public static readonly Color SubHeaderRule = new(0.620f, 0.580f, 0.420f, 0.55f);
        public static readonly Color Caption       = new(0.773f, 0.765f, 0.698f, 1f);
        public static readonly Color Value         = new(0.714f, 0.757f, 0.780f, 1f);
        public static readonly Color ValuePositive = new(0.95f, 0.91f, 0.55f, 1f);
        public static readonly Color ValueNegative = new(0.88f, 0.55f, 0.50f, 1f);
        public static readonly Color TrendUp       = new(107f / 255f, 203f / 255f, 119f / 255f, 1f);
        public static readonly Color TrendDown     = new(229f / 255f, 115f / 255f, 115f / 255f, 1f);
        public static readonly Color TrendFlat     = new(160f / 255f, 160f / 255f, 160f / 255f, 1f);
        /// <summary>Handbook-style screen caption (e.g. RECORDS title).</summary>
        public static readonly Color ScreenCaption = new(0.976f, 0.976f, 0.890f, 1f);

        internal static string ColoredValue(long rub, Color color, string suffix = " ₽"){
            return WrapRichText($"{rub}{suffix}", color);
        }

        internal static string WrapRichText(string text, Color color){
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
        }
    }

    internal static class Surface{
        public static readonly Color HeaderBackdrop     = new(0.455f, 0.467f, 0.459f, 0.188f);
        public static readonly Color ItemsPanelBackdrop = new(0.04f, 0.04f, 0.04f, 0.22f);
        public static readonly Color PanelBackground    = new(0.06f, 0.06f, 0.06f, 0.92f);
        public static readonly Color ScrollViewport     = new(1f, 1f, 1f, 0.01f);
    }

    internal static class Control{
        public static readonly Color ButtonSurface      = new(0.18f, 0.18f, 0.18f, 0.55f);
        public static readonly Color ButtonHighlight    = new(0.28f, 0.27f, 0.24f, 0.85f);
        public static readonly Color ButtonPressed      = new(0.22f, 0.21f, 0.19f, 0.9f);
        public static readonly Color Disabled           = new(0.12f, 0.12f, 0.12f, 0.35f);
        public static readonly Color GraphicWhite       = Color.white;
        public static readonly Color DragHitTransparent = new(0f, 0f, 0f, 0f);
    }

    internal static class ListRow{
        public static readonly Color Idle    = new(0.12f, 0.12f, 0.12f, 0.5f);
        public static readonly Color Hover   = new(0.22f, 0.21f, 0.19f, 0.82f);
        public static readonly Color Pressed = new(0.18f, 0.17f, 0.16f, 0.88f);
    }

    internal static class FilterChip{
        public static readonly Color Idle          = new(0.18f, 0.18f, 0.18f, 0.72f);
        public static readonly Color Hover         = new(0.26f, 0.25f, 0.22f, 0.9f);
        public static readonly Color Pressed       = new(0.22f, 0.21f, 0.19f, 0.88f);
        public static readonly Color Active        = new(0.34f, 0.33f, 0.28f, 0.96f);
        public static readonly Color ActiveHover   = new(0.38f, 0.37f, 0.32f, 1f);
        public static readonly Color ActivePressed = new(0.30f, 0.29f, 0.25f, 1f);
    }

    internal static class TabBar{
        public static readonly Color Idle          = new(0.20f, 0.20f, 0.20f, 0.75f);
        public static readonly Color Hover         = new(0.28f, 0.27f, 0.24f, 0.92f);
        public static readonly Color Pressed       = new(0.24f, 0.23f, 0.21f, 0.88f);
        public static readonly Color Active        = new(0.35f, 0.34f, 0.30f, 0.95f);
        public static readonly Color ActiveHover   = new(0.40f, 0.39f, 0.34f, 1f);
        public static readonly Color ActivePressed = new(0.32f, 0.31f, 0.27f, 1f);
    }

    internal static class Chart{
        public static readonly Color32 Background = new(10, 10, 10, 210);
        public static readonly Color32 GridLine   = new(110, 110, 110, 200);
        public static readonly Color32 GridLineV  = new(95, 95, 95, 170);
        public static readonly Color32 Line       = new(255, 200, 60, 245);
        public static readonly Color32 Bar        = new(180, 210, 230, 230);
        public static readonly Color32 Point      = new(255, 220, 100, 255);
    }

    internal static class Map{
        public static readonly Color32 TrailLine = new(255, 200, 60, 230);

        public static readonly Color PopoverBackground      = new(0.06f, 0.06f, 0.06f, 0.85f);
        public static readonly Color PopoverBackgroundHover = new(0.06f, 0.06f, 0.06f, 0.10f);
    }
}
