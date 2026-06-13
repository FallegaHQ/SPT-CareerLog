using Softwyx.CareerLog.Ui.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Shared;

/// <summary>Scroll body lines and stat rows. Used only via <see cref="ScrollContentBuilder"/>.</summary>
internal static class ScrollRowBuilder{
    internal static void AddSubHeader(Transform items, string line, ScrollContentBuilder.ScrollTextStyle style){
        if(!items || string.IsNullOrEmpty(line)) return;

        CreateSubHeaderText(items, line, style);
    }

    internal static void AddBodyLine(Transform items, string line, ScrollContentBuilder.ScrollTextStyle style){
        if(!items || string.IsNullOrEmpty(line)) return;

        CreateBodyText(items, line, style);
    }

    internal static void AddLocalizedBodyLine(
        Transform items, string localizationKey, ScrollContentBuilder.ScrollTextStyle style
    ){
        if(!items || string.IsNullOrEmpty(localizationKey)) return;

        CreateLocalizedBodyText(items, localizationKey, style);
    }

    internal static void AddStatRow(
        Transform items, string caption, string value, ScrollContentBuilder.ScrollTextStyle style
    ){
        if(!items) return;

        CreateStatRow(items, caption, value, style);
    }

    internal static void AddLocalizedStatRow(
        Transform items, string captionKey, string value, ScrollContentBuilder.ScrollTextStyle style
    ){
        if(!items) return;

        CreateStatRow(items, captionKey, value, style, true);
    }

    internal static void ConfigureText(
        TextMeshProUGUI      text, string value, float fontSize, Color color, TMP_FontAsset font,
        TextAlignmentOptions alignment
    ){
        text.text               = value;
        text.fontSize           = fontSize;
        text.color              = color;
        text.alignment          = alignment;
        text.fontStyle          = FontStyles.Normal;
        text.richText           = true;
        text.lineSpacing        = Typography.LineSpacing;
        text.overflowMode       = TextOverflowModes.Ellipsis;
        text.enableWordWrapping = false;

        if(font) text.font = font;
    }

    private static void CreateSubHeaderText(Transform parent, string text, ScrollContentBuilder.ScrollTextStyle style){
        var block = new GameObject(
                                   "SubHeaderBlock",
                                   typeof(RectTransform),
                                   typeof(LayoutElement),
                                   typeof(VerticalLayoutGroup)
                                  );
        block.transform.SetParent(parent, false);

        var blockLayout = block.GetComponent<VerticalLayoutGroup>();
        blockLayout.spacing                = 0f;
        blockLayout.padding                = new RectOffset(0, 0, 0, (int) Spacing.SubHeaderBottomSpace);
        blockLayout.childAlignment         = TextAnchor.UpperLeft;
        blockLayout.childControlWidth      = true;
        blockLayout.childControlHeight     = true;
        blockLayout.childForceExpandWidth  = true;
        blockLayout.childForceExpandHeight = false;

        var blockElement = block.GetComponent<LayoutElement>();
        blockElement.flexibleWidth = 1f;
        blockElement.minHeight     = Typography.SubHeader + Spacing.SubHeaderRuleHeight + 6f;

        var labelObject = new GameObject(
                                         ScrollContentBuilder.BodyObjectName,
                                         typeof(RectTransform),
                                         typeof(LayoutElement)
                                        );
        labelObject.transform.SetParent(block.transform, false);

        var labelElement = labelObject.GetComponent<LayoutElement>();
        labelElement.flexibleWidth = 1f;
        labelElement.minHeight     = Typography.SubHeader + 4f;

        var tmp = labelObject.AddComponent<TextMeshProUGUI>();
        ConfigureText(
                      tmp,
                      text,
                      Typography.SubHeader,
                      Colors.Text.SubHeader,
                      style.Font,
                      TextAlignmentOptions.BottomLeft
                     );
        tmp.fontStyle          = FontStyles.Bold | FontStyles.UpperCase;
        tmp.enableWordWrapping = false;

        var ruleObject = new GameObject("SubHeaderRule", typeof(RectTransform), typeof(LayoutElement), typeof(Image));
        ruleObject.transform.SetParent(block.transform, false);

        var ruleElement = ruleObject.GetComponent<LayoutElement>();
        ruleElement.flexibleWidth   = 1f;
        ruleElement.flexibleHeight  = 0f;
        ruleElement.minHeight       = Spacing.SubHeaderRuleHeight;
        ruleElement.preferredHeight = Spacing.SubHeaderRuleHeight;
        ruleElement.layoutPriority  = 1;

        var ruleImage = ruleObject.GetComponent<Image>();
        ruleImage.color         = Colors.Text.SubHeaderRule;
        ruleImage.raycastTarget = false;
        ruleImage.type          = Image.Type.Simple;
        ruleImage.useSpriteMesh = true;
    }

    private static void CreateLocalizedBodyText(
        Transform parent, string localizationKey, ScrollContentBuilder.ScrollTextStyle style
    ){
        var bodyObject = new GameObject(
                                        ScrollContentBuilder.BodyObjectName,
                                        typeof(RectTransform),
                                        typeof(LayoutElement)
                                       );
        bodyObject.transform.SetParent(parent, false);

        var bodyElement = bodyObject.GetComponent<LayoutElement>();
        bodyElement.flexibleWidth = 1f;

        var text = bodyObject.AddComponent<TextMeshProUGUI>();
        ConfigureText(
                      text,
                      string.Empty,
                      Typography.Body,
                      style.CaptionColor,
                      style.Font,
                      TextAlignmentOptions.TopLeft
                     );
        text.enableWordWrapping = true;

        LocalizedTextView.BindKey(bodyObject.transform, localizationKey, null);
    }

    private static void CreateBodyText(Transform parent, string body, ScrollContentBuilder.ScrollTextStyle style){
        var bodyObject = new GameObject(
                                        ScrollContentBuilder.BodyObjectName,
                                        typeof(RectTransform),
                                        typeof(LayoutElement)
                                       );
        bodyObject.transform.SetParent(parent, false);

        var bodyElement = bodyObject.GetComponent<LayoutElement>();
        bodyElement.flexibleWidth = 1f;

        var text = bodyObject.AddComponent<TextMeshProUGUI>();
        ConfigureText(
                      text,
                      body,
                      Typography.Body,
                      style.CaptionColor,
                      style.Font,
                      TextAlignmentOptions.TopLeft
                     );
        text.enableWordWrapping = true;
    }

    private static void CreateStatRow(
        Transform parent, string caption, string value, ScrollContentBuilder.ScrollTextStyle style,
        bool      localizeCaption = false
    ){
        var rowObject = new GameObject(
                                       ScrollContentBuilder.RowObjectName,
                                       typeof(RectTransform),
                                       typeof(HorizontalLayoutGroup),
                                       typeof(LayoutElement)
                                      );
        rowObject.transform.SetParent(parent, false);

        var rowLayout = rowObject.GetComponent<HorizontalLayoutGroup>();
        rowLayout.spacing                = 0f;
        rowLayout.childAlignment         = TextAnchor.MiddleLeft;
        rowLayout.childControlWidth      = true;
        rowLayout.childControlHeight     = true;
        rowLayout.childForceExpandWidth  = true;
        rowLayout.childForceExpandHeight = false;

        var rowElement = rowObject.GetComponent<LayoutElement>();
        rowElement.minHeight       = Spacing.RowMinHeight;
        rowElement.preferredHeight = Spacing.RowMinHeight;
        rowElement.flexibleWidth   = 1f;
        rowElement.minWidth        = 0f;

        if(localizeCaption)
            CreateLocalizedRowCell(
                                   rowObject.transform,
                                   caption,
                                   0.55f,
                                   style,
                                   style.CaptionColor,
                                   TextAlignmentOptions.TopLeft
                                  );
        else
            CreateRowCell(rowObject.transform, caption, 0.55f, style, style.CaptionColor, TextAlignmentOptions.TopLeft);

        CreateRowCell(rowObject.transform, value, 0.45f, style, style.ValueColor, TextAlignmentOptions.TopRight);
    }

    private static void CreateLocalizedRowCell(
        Transform row,   string localizationKey, float widthShare, ScrollContentBuilder.ScrollTextStyle style,
        Color     color, TextAlignmentOptions alignment
    ){
        var cellObject = new GameObject("Cell", typeof(RectTransform), typeof(LayoutElement));
        cellObject.transform.SetParent(row, false);

        var cellElement = cellObject.GetComponent<LayoutElement>();
        cellElement.flexibleWidth   = widthShare;
        cellElement.minHeight       = Spacing.RowMinHeight;
        cellElement.preferredHeight = Spacing.RowMinHeight;

        var text = cellObject.AddComponent<TextMeshProUGUI>();
        ConfigureText(text, string.Empty, Typography.Row, color, style.Font, alignment);
        text.rectTransform.sizeDelta = Vector2.zero;

        LocalizedTextView.BindKey(cellObject.transform, localizationKey, null);
    }

    private static void CreateRowCell(
        Transform row, string textValue, float widthShare, ScrollContentBuilder.ScrollTextStyle style, Color color,
        TextAlignmentOptions alignment
    ){
        var cellObject = new GameObject("Cell", typeof(RectTransform), typeof(LayoutElement));
        cellObject.transform.SetParent(row, false);

        var cellElement = cellObject.GetComponent<LayoutElement>();
        cellElement.flexibleWidth   = widthShare;
        cellElement.minWidth        = 0f;
        cellElement.minHeight       = Spacing.RowMinHeight;
        cellElement.preferredHeight = Spacing.RowMinHeight;

        var text = cellObject.AddComponent<TextMeshProUGUI>();
        ConfigureText(text, textValue, Typography.Row, color, style.Font, alignment);
        text.rectTransform.sizeDelta = Vector2.zero;
    }
}
