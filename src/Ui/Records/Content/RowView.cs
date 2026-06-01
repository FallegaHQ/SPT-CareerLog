using System;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Formatting;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Content;

internal static class RowView{
    public static void Add(Transform parent, RaidIndexEntry raid, Action<string> onSelected){
        if(!parent || raid == null || string.IsNullOrEmpty(raid.RaidId)) return;

        var rowObject = new GameObject(
                                       $"RaidRow_{raid.RaidId}",
                                       typeof(RectTransform),
                                       typeof(Image),
                                       typeof(Button),
                                       typeof(LayoutElement)
                                      );
        rowObject.transform.SetParent(parent, false);

        var layoutElement = rowObject.GetComponent<LayoutElement>();
        layoutElement.minHeight       = Spacing.RowMinHeight;
        layoutElement.preferredHeight = Spacing.RowMinHeight;
        layoutElement.flexibleWidth   = 1f;

        var image = rowObject.GetComponent<Image>();
        image.color = Color.white;

        var button = rowObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.transition    = Selectable.Transition.ColorTint;
        button.onClick.AddListener(() => onSelected?.Invoke(raid.RaidId));

        ControlButtonColors.ApplyListRow(button);

        var labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(rowObject.transform, false);
        StretchFull(labelObject.GetComponent<RectTransform>());

        var tmp = labelObject.GetComponent<TextMeshProUGUI>();
        tmp.text               = RaidSummaryFormatter.FormatRaidListLine(raid);
        tmp.fontSize           = Typography.Body;
        tmp.color              = Colors.Text.Caption;
        tmp.alignment          = TextAlignmentOptions.MidlineLeft;
        tmp.enableWordWrapping = false;
        tmp.overflowMode       = TextOverflowModes.Ellipsis;
        tmp.raycastTarget      = false;
    }

    private static void StretchFull(RectTransform rect){
        rect.anchorMin        = Vector2.zero;
        rect.anchorMax        = Vector2.one;
        rect.offsetMin        = new Vector2(10f,  0f);
        rect.offsetMax        = new Vector2(-10f, 0f);
        rect.anchoredPosition = Vector2.zero;
    }
}
