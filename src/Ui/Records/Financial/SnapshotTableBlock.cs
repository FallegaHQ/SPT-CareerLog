using System;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal static class SnapshotTableBlock{
    public static void Add(
        Transform parent, StashSnapshot snapshot, ScrollContentBuilder.ScrollTextStyle style, Action onClick
    ){
        if(!parent || snapshot == null) return;

        var blockObject = new GameObject(
                                         "FinancialSnapshotBlock",
                                         typeof(RectTransform),
                                         typeof(Image),
                                         typeof(Button),
                                         typeof(VerticalLayoutGroup),
                                         typeof(LayoutElement)
                                        );
        blockObject.transform.SetParent(parent, false);

        var layoutElement = blockObject.GetComponent<LayoutElement>();
        layoutElement.flexibleWidth   = 1f;
        layoutElement.minHeight       = Spacing.RowMinHeight * 3f;
        layoutElement.preferredHeight = -1f;

        var image = blockObject.GetComponent<Image>();
        image.color = Color.white;

        var button = blockObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.transition    = Selectable.Transition.ColorTint;
        button.interactable  = onClick != null;

        if(onClick != null) button.onClick.AddListener(() => onClick());

        ControlButtonColors.ApplyListRow(button, onClick != null);

        var layout = blockObject.GetComponent<VerticalLayoutGroup>();
        layout.padding                = new RectOffset(10, 10, 6, 6);
        layout.spacing                = 2f;
        layout.childAlignment         = TextAnchor.UpperLeft;
        layout.childControlWidth      = true;
        layout.childControlHeight     = true;
        layout.childForceExpandWidth  = true;
        layout.childForceExpandHeight = false;

        ScrollContentBuilder.AddBodyLine(blockObject.transform, ValueFormatter.LocalTime(snapshot.Utc), style);
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 blockObject.transform,
                                                 LocaleKeys.FinancialTotalWorth,
                                                 ValueFormatter.Rubles(snapshot.TotalWorth),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 blockObject.transform,
                                                 LocaleKeys.FinancialPeriodDelta,
                                                 ValueFormatter.SignedRubles(snapshot.DeltaRubles),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 blockObject.transform,
                                                 LocaleKeys.FinancialLiquidRubles,
                                                 ValueFormatter.Rubles(snapshot.Rubles),
                                                 style
                                                );
        ScrollContentBuilder.AddLocalizedStatRow(
                                                 blockObject.transform,
                                                 LocaleKeys.FinancialItemsValue,
                                                 ValueFormatter.Rubles(snapshot.ItemsValue),
                                                 style
                                                );
    }
}
