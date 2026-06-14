using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Menu;

/// <summary>Caption row on the handbook-cloned Records screen -- layout matched to vanilla <c>HandbookScreen/Caption</c>.</summary>
internal static class CaptionHelper{
    private const float CaptionTitleFontSize = 36f;
    // Vanilla HandbookScreen/Caption (REPL export); EditBuildScreen uses a different pattern.
    private static readonly Vector2 CaptionIconSizeDelta        = new(42f, 48f);
    private static readonly Vector2 CaptionIconAnchoredPosition = new(42f, 0f);

    public static TextMeshProUGUI Apply(Transform recordsRoot){
        if(!recordsRoot) return null;

        var caption = recordsRoot.Find(UiHierarchy.RecordsScreen.Caption);

        if(!caption) return null;

        var label = caption.Find(UiHierarchy.RecordsScreen.CaptionLabel);

        if(label) LocalizedTextView.BindKey(label, LocaleKeys.RecordsCaption, null);

        ApplyCaptionIcon(caption);
        ApplyCaptionLabel(label);

        return label
                   ? label.GetComponent<TextMeshProUGUI>() ?? label.GetComponent<TMP_Text>() as TextMeshProUGUI
                   : null;
    }

    private static void ApplyCaptionIcon(Transform caption){
        var iconTransform = caption.Find(UiHierarchy.RecordsScreen.CaptionIcon);

        if(!iconTransform) return;

        var iconRect = iconTransform as RectTransform;

        if(iconRect){
            iconRect.anchorMin        = new Vector2(0f, 1f);
            iconRect.anchorMax        = new Vector2(0f, 1f);
            iconRect.pivot            = new Vector2(1f, 1f);
            iconRect.sizeDelta        = CaptionIconSizeDelta;
            iconRect.anchoredPosition = CaptionIconAnchoredPosition;
        }

        var iconImage = iconTransform.GetComponent<Image>();

        if(!iconImage) return;

        var sprite = ScreenIcon.Get();

        if(sprite){
            iconImage.sprite         = sprite;
            iconImage.overrideSprite = sprite;
            iconImage.color          = Color.white;
            iconImage.material       = null;
        }

        iconImage.preserveAspect = true;
    }

    private static void ApplyCaptionLabel(Transform label){
        if(!label) return;

        var tmp = label.GetComponent<TextMeshProUGUI>() ?? label.GetComponent<TMP_Text>() as TextMeshProUGUI;

        if(!tmp) return;

        tmp.fontSize         = CaptionTitleFontSize;
        tmp.color            = Colors.Text.ScreenCaption;
        tmp.fontStyle        = FontStyles.Normal;
        tmp.alignment        = TextAlignmentOptions.Top;
        tmp.enableAutoSizing = false;
        tmp.characterSpacing = 0f;
    }
}
