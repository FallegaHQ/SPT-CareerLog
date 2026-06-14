using System;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Ui.Design;
using Softwyx.CareerLog.Ui.Shared;
using Softwyx.CareerLog.Ui.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Softwyx.CareerLog.Ui.Records.Content;

internal sealed class TabBarView{
    private readonly Action<Tab> _onTabSelected;
    private readonly Transform   _tabBarRoot;
    private readonly string[] _tabLabelKeys =[
                                                 LocaleKeys.TabSummary,
                                                 LocaleKeys.TabFinancial,
                                                 LocaleKeys.TabRaids
                                             ];
    private Button[]          _tabButtons;
    private TextMeshProUGUI[] _tabLabels;

    internal TabBarView(Transform tabBarRoot, Action<Tab> onTabSelected){
        _tabBarRoot    = tabBarRoot;
        _onTabSelected = onTabSelected;
    }

    internal void EnsureBuilt(){
        if(!_tabBarRoot || _tabButtons != null) return;

        _tabButtons = new Button[_tabLabelKeys.Length];
        _tabLabels  = new TextMeshProUGUI[_tabLabelKeys.Length];

        for(var i = 0; i < _tabLabelKeys.Length; i++){
            var tab = (Tab) i;
            CreateTabButton(
                            _tabBarRoot,
                            _tabLabelKeys[i],
                            () => _onTabSelected(tab),
                            out _tabButtons[i],
                            out _tabLabels[i]
                           );
        }

        RefreshHighlight();
    }

    internal void RefreshHighlight(){
        if(_tabButtons == null) return;

        for(var i = 0; i < _tabButtons.Length; i++){
            if(!_tabButtons[i]) continue;

            var active = i == (int) NavigationState.ActiveTab;
            ApplyTabButtonState(_tabButtons[i], active);
            ApplyTabLabelState(_tabLabels[i], active);
        }
    }

    private static void ApplyTabLabelState(TextMeshProUGUI label, bool active){
        if(!label) return;

        label.color     = active ? Colors.Text.SectionAccent : Colors.Text.Caption;
        label.fontStyle = active ? FontStyles.Bold : FontStyles.Normal;
    }

    private static void CreateTabButton(
        Transform parent, string localizationKey, Action onClick, out Button button, out TextMeshProUGUI label
    ){
        button = null;
        label  = null;
        var buttonObject = new GameObject(
                                          localizationKey,
                                          typeof(RectTransform),
                                          typeof(Image),
                                          typeof(Button),
                                          typeof(LayoutElement)
                                         );
        buttonObject.transform.SetParent(parent, false);

        var layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.minHeight      = 38f;
        layoutElement.preferredWidth = 148f;
        layoutElement.flexibleWidth  = 1f;

        var image = buttonObject.GetComponent<Image>();
        image.color = Colors.Control.GraphicWhite;

        var tabButton = buttonObject.GetComponent<Button>();
        tabButton.targetGraphic = image;
        tabButton.transition    = Selectable.Transition.ColorTint;
        tabButton.onClick.AddListener(() => onClick());

        ApplyTabButtonState(tabButton, false);

        var textObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(buttonObject.transform, false);
        RectLayout.StretchToParent(textObject.GetComponent<RectTransform>());

        var tmp = textObject.GetComponent<TextMeshProUGUI>();
        tmp.fontSize           = Typography.Header;
        tmp.color              = Colors.Text.Caption;
        tmp.alignment          = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.raycastTarget      = false;

        LocalizedTextView.BindKey(textObject.transform, localizationKey, null);

        button = tabButton;
        label  = tmp;
    }

    private static void ApplyTabButtonState(Button button, bool active){
        if(!button) return;

        var colors = button.colors;
        colors.colorMultiplier = 1f;
        colors.fadeDuration    = 0.1f;

        if(active){
            colors.normalColor      = Colors.TabBar.Active;
            colors.highlightedColor = Colors.TabBar.ActiveHover;
            colors.pressedColor     = Colors.TabBar.ActivePressed;
            colors.selectedColor    = Colors.TabBar.Active;
        }
        else{
            colors.normalColor      = Colors.TabBar.Idle;
            colors.highlightedColor = Colors.TabBar.Hover;
            colors.pressedColor     = Colors.TabBar.Pressed;
            colors.selectedColor    = Colors.TabBar.Idle;
        }

        button.colors = colors;

        if(button.targetGraphic) button.targetGraphic.color = Colors.Control.GraphicWhite;
    }
}
