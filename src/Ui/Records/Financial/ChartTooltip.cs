using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Financial;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Financial;

internal sealed class ChartTooltip : MonoBehaviour{
    private RectTransform   _panel;
    private TextMeshProUGUI _body;

    public static ChartTooltip Install(Transform parent, ScrollContentBuilder.ScrollTextStyle style){
        var rootObject = new GameObject("ChartTooltip", typeof(RectTransform), typeof(ChartTooltip));
        rootObject.transform.SetParent(parent, false);

        var tooltip = rootObject.GetComponent<ChartTooltip>();
        tooltip.Build(style);
        tooltip.Hide();

        return tooltip;
    }

    private void Build(ScrollContentBuilder.ScrollTextStyle style){
        _panel = transform as RectTransform;
        if(_panel) _panel.sizeDelta = new Vector2(240f, 84f);

        var backdrop = gameObject.AddComponent<Image>();
        backdrop.color         = Colors.Surface.PanelBackground;
        backdrop.raycastTarget = false;

        var bodyObject = new GameObject("Body", typeof(RectTransform), typeof(TextMeshProUGUI));
        bodyObject.transform.SetParent(transform, false);
        var bodyRect = bodyObject.GetComponent<RectTransform>();
        bodyRect.anchorMin = Vector2.zero;
        bodyRect.anchorMax = Vector2.one;
        bodyRect.offsetMin = new Vector2(8f,  6f);
        bodyRect.offsetMax = new Vector2(-8f, -6f);

        _body                    = bodyObject.GetComponent<TextMeshProUGUI>();
        _body.fontSize           = Typography.Body;
        _body.color              = Colors.Text.Caption;
        _body.alignment          = TextAlignmentOptions.TopLeft;
        _body.richText           = true;
        _body.enableWordWrapping = true;
        _body.raycastTarget      = false;

        if(style.Font) _body.font = style.Font;
    }

    public void Show(ChartPoint point, Vector2 screenPosition, RectTransform bounds){
        if(point.Snapshot == null){
            Hide();

            return;
        }

        var snap = point.Snapshot;

        _body.text = LocaleLoader.Format(
                                         LocaleKeys.ChartTooltip,
                                         ValueFormatter.LocalTime(snap.Utc),
                                         ValueFormatter.Rubles(snap.TotalWorth),
                                         ValueFormatter.TrendRichText(snap.DeltaRubles)
                                        );

        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(bounds, screenPosition, null, out var local))
            return;

        var half = _panel.sizeDelta * 0.5f;
        var pos  = local + new Vector2(14f, 18f);

        pos.x = Mathf.Clamp(pos.x, bounds.rect.xMin + half.x, bounds.rect.xMax - half.x);
        pos.y = Mathf.Clamp(pos.y, bounds.rect.yMin + half.y, bounds.rect.yMax - half.y);

        _panel.anchoredPosition = pos;
    }

    public void Hide(){
        gameObject.SetActive(false);
    }
}
