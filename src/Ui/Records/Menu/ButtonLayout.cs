using System.Collections;
using System.Collections.Generic;
using EFT;
using EFT.UI;
using Softwyx.CareerLog.Compat;
using Softwyx.CareerLog.Infrastructure;
using Softwyx.CareerLog.Interop;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Menu;

/// <summary>
///     Inserts the RECORDS menu button.
///     Layout mirrors SPT-Menu-Overhaul when that mod's
///     button stack is detected (hierarchy names + x=250 anchor stack).
/// </summary>
internal static class ButtonLayout{
    private const float OverhaulButtonX            = 250f;
    private const float OverhaulButtonYStep        = 60f;
    private const float OverhaulIconScale          = 0.8f;
    private const float VanillaSlotSpacingFallback = -52f;
    private const float BetaWarningLocalY          = -5f;
    private const float BetaWarningScale           = 0.75f;
    private const float SlotStepScale              = 0.82f;
    private const float SlotExtraOffset            = 10f;

    private static DefaultUIButton    _recordsButton;
    private static MenuButtonSnapshot _vanillaSnapshot;
    private static int                _layoutMenuScreenId;
    private static bool               _layoutRevealReady;
    private static Coroutine          _revealCoroutine;

    public static bool IsMenuOverhaulLayoutActive(MenuScreen menuScreen){
        return IsMenuOverhaulLayout(menuScreen);
    }

    public static void PrepareForLayout(MenuScreen menuScreen){
        if(!menuScreen) return;

        // Schedule() can be invoked multiple times for the same MenuScreen instance (Show + ShowAction).
        // Keep the stack invisible until RevealMenuStack(); other mods may still reposition active buttons.
        _layoutRevealReady = false;
        StopRevealCoroutine();

        if(PitFireTeamCompat.ShouldPreserveCharacterButtonTemplate)
            PitFireTeamCompat.StripLayoutCanvasGroup(MenuScreenReflection.GetCharacterButton(menuScreen));

        EnsureRecordsButton(menuScreen);

        if(_recordsButton) _recordsButton.gameObject.SetActive(true);

        MenuButtonReveal.HideStack(menuScreen, _recordsButton);
    }

    public static void OnMenuShown(MenuScreen menuScreen, Profile profile){
        if(!menuScreen) return;

        ResetLayoutStateIfNeeded(menuScreen);
        EnsureRecordsButton(menuScreen);
        ApplyLayout(menuScreen);

        profile ??= ProfileResolver.FromMenuScreen(menuScreen);

        if(profile != null) MenuContext.Set(profile);

        MenuButtonReveal.HideStack(menuScreen, _recordsButton);
        SyncVisibility(menuScreen);

        if(PitFireTeamCompat.IsLoaded) PitFireTeamCompat.RepairOverlayButtons(menuScreen);
    }

    public static void RevealMenuStack(MenuScreen menuScreen){
        _layoutRevealReady = true;
        StopRevealCoroutine();

        if(!menuScreen) return;

        var showRecords = ResolveRecordsVisible(menuScreen);

        if(!CareerLogPlugin.Instance){
            MenuButtonReveal.RevealImmediate(menuScreen, showRecords ? _recordsButton : null);
            MenuButtonReveal.FinalizeStack(menuScreen, showRecords ? _recordsButton : null);
            PitFireTeamCompat.RepairOverlayButtons(menuScreen);
            SyncVisibility(menuScreen);

            return;
        }

        _revealCoroutine = CareerLogPlugin.Instance.StartCoroutine(RevealMenuStackRoutine(menuScreen, showRecords));
    }

    private static IEnumerator RevealMenuStackRoutine(MenuScreen menuScreen, bool showRecords){
        MenuButtonReveal.RevealImmediate(menuScreen, showRecords ? _recordsButton : null);

        MenuButtonReveal.FinalizeStack(menuScreen, showRecords ? _recordsButton : null);
        PitFireTeamCompat.RepairOverlayButtons(menuScreen);
        _revealCoroutine = null;
        SyncVisibility(menuScreen);

        yield break;
    }

    public static void SyncVisibility(MenuScreen menuScreen){
        if(!menuScreen) return;

        if(!_layoutRevealReady){
            MenuButtonReveal.HideStack(menuScreen, _recordsButton);

            return;
        }

        if(!_recordsButton) return;

        if(!ResolveRecordsVisible(menuScreen)){
            _recordsButton.gameObject.SetActive(false);
            MenuButtonReveal.HideRoot(_recordsButton.gameObject);

            return;
        }

        _recordsButton.gameObject.SetActive(true);
        MenuButtonReveal.EnsureInteractable(_recordsButton.gameObject);
        ConfigureClickHandler(_recordsButton);
        ResetAnimatedButton(_recordsButton);
    }

    private static bool ResolveRecordsVisible(MenuScreen menuScreen){
        if(!_recordsButton || !menuScreen) return false;

        var playerButton  = MenuScreenReflection.GetCharacterButton(menuScreen);
        var playerVisible = !playerButton || playerButton.gameObject.activeSelf;

        return playerVisible && !MenuScreenReflection.IsMinimized(menuScreen);
    }

    private static void StopRevealCoroutine(){
        if(_revealCoroutine == null || !CareerLogPlugin.Instance) return;

        CareerLogPlugin.Instance.StopCoroutine(_revealCoroutine);
        _revealCoroutine = null;
    }

    private static void EnsureRecordsButton(MenuScreen menuScreen){
        if(_recordsButton) return;

        var template = MenuScreenReflection.GetCharacterButton(menuScreen);

        if(!template){
            CareerLogPlugin.Log?.LogWarning(
                                            PluginInfo.Format(
                                                              "Menu RECORDS layout failed -- CharacterButton / player button not found."
                                                             )
                                           );

            return;
        }

        var parent = template.transform.parent ? template.transform.parent : menuScreen.transform;

        _recordsButton = Object.Instantiate(template.gameObject, parent).
                                GetComponent<DefaultUIButton>();
        _recordsButton.gameObject.name = GameAssemblyNames.MenuScreenHierarchy.RecordsButton;
        _recordsButton.gameObject.SetActive(false);
        _vanillaSnapshot = null;

        ConfigureClickHandler(_recordsButton);

        CareerLogPlugin.Log?.LogInfo(PluginInfo.Format("Menu RECORDS button created."));
    }

    private static void ApplyLayout(MenuScreen menuScreen){
        if(!_recordsButton) return;

        var recordsRect = _recordsButton.GetComponent<RectTransform>();

        if(!recordsRect) return;

        if(IsMenuOverhaulLayout(menuScreen)){
            ApplyMenuOverhaulLayout(menuScreen, recordsRect);
        }
        else{
            ApplyVanillaLayout(menuScreen, recordsRect);
            ApplyBetaWarningLayout(menuScreen);
            ConfigureVanillaAppearance(_recordsButton, MenuScreenReflection.GetCharacterButton(menuScreen));
        }
    }

    private static bool IsMenuOverhaulLayout(MenuScreen menuScreen){
        var characterRect = MenuScreenReflection.GetHierarchyButtonRect(
                                                                        menuScreen,
                                                                        GameAssemblyNames.MenuScreenHierarchy.
                                                                            CharacterButton
                                                                       );

        if(!characterRect) return false;

        return Mathf.Approximately(characterRect.anchorMin.x,        0f)
            && Mathf.Approximately(characterRect.anchorMax.x,        0f)
            && Mathf.Approximately(characterRect.anchoredPosition.x, OverhaulButtonX);
    }

    private static void ApplyMenuOverhaulLayout(MenuScreen menuScreen, RectTransform recordsRect){
        // Menu Overhaul stack (x=250, 60px steps). MY SQUAD sits at slot 2 when PiT is loaded;
        // Trade/Hideout/Exit keep the same indices as without PiT (no extra gap below RECORDS).
        var squadButton = MenuScreenReflection.GetSquadControlButton(menuScreen);
        var stackLift   = squadButton ? OverhaulButtonYStep : 0f;
        var recordsSlot = squadButton ? 3 : 2;

        PositionOverhaulButton(menuScreen, GameAssemblyNames.MenuScreenHierarchy.PlayButton,      0, stackLift);
        PositionOverhaulButton(menuScreen, GameAssemblyNames.MenuScreenHierarchy.CharacterButton, 1, stackLift);

        if(squadButton){
            var squadRect = squadButton.GetComponent<RectTransform>();

            if(squadRect) ApplyOverhaulTransform(squadRect, -OverhaulButtonYStep * 2f + stackLift);
        }

        ApplyOverhaulTransform(recordsRect, -OverhaulButtonYStep * recordsSlot + stackLift);

        PositionOverhaulButton(menuScreen, GameAssemblyNames.MenuScreenHierarchy.TradeButton,     3);
        PositionOverhaulButton(menuScreen, GameAssemblyNames.MenuScreenHierarchy.HideoutButton,   4);
        PositionOverhaulButton(menuScreen, GameAssemblyNames.MenuScreenHierarchy.ExitButtonGroup, 5);

        CareerLogPlugin.Log?.LogInfo(
                                     PluginInfo.Format(
                                                       stackLift > 0f
                                                           ? $"Menu overhaul layout applied (pitFireTeam, stackLift={stackLift:F0})."
                                                           : "Menu overhaul layout applied."
                                                      )
                                    );

        var fontSize = ResolveCharacterFontSize(menuScreen);
        _recordsButton.SetRawText("RECORDS", fontSize);
        ApplyMenuOverhaulVisuals(_recordsButton.gameObject, false);
        ApplyRecordsIcon(_recordsButton, true);
        ResetAnimatedButton(_recordsButton);
    }

    private static void ApplyVanillaLayout(MenuScreen menuScreen, RectTransform recordsRect){
        var snapshot = CaptureVanillaSnapshot(menuScreen);

        if(!snapshot?.Player) return;

        snapshot.Restore();

        var playerRect = snapshot.Player;
        var playRect   = snapshot.Play;
        var tradeRect = MenuScreenReflection.GetTradeButton(menuScreen)?.
                                             GetComponent<RectTransform>();
        var hideoutRect = MenuScreenReflection.GetHideoutButton(menuScreen)?.
                                               GetComponent<RectTransform>();
        var squadRect = MenuScreenReflection.GetSquadControlButton(menuScreen)?.
                                             GetComponent<RectTransform>();

        var slotOffset = ResolvePitRecordsSlotOffset(tradeRect, hideoutRect, playerRect);

        ApplyStackButtonTransform(recordsRect, playerRect);
        recordsRect.SetParent(playerRect.parent, false);

        if(squadRect){
            if(!PitFireTeamCompat.IsLoaded) ShiftButtonsAbovePlayer(playRect, playerRect, slotOffset);

            recordsRect.anchoredPosition = squadRect.anchoredPosition + new Vector2(0f, slotOffset);
            recordsRect.SetSiblingIndex(squadRect.GetSiblingIndex());

            CareerLogPlugin.Log?.LogInfo(
                                         PluginInfo.Format(
                                                           $"Vanilla menu layout applied (pitFireTeam, slotOffset={slotOffset:F1})."
                                                          )
                                        );

            return;
        }

        var spacing = snapshot.SlotSpacing;
        var lift    = -spacing;

        ShiftButtonsAbovePlayer(playRect, playerRect, lift);

        recordsRect.anchoredPosition = playerRect.anchoredPosition + new Vector2(0f, spacing);
        recordsRect.SetSiblingIndex(playerRect.GetSiblingIndex() + 1);

        CareerLogPlugin.Log?.LogInfo(
                                     PluginInfo.Format(
                                                       $"Vanilla menu layout applied (spacing={spacing:F1}, lift={lift:F1})."
                                                      )
                                    );
    }

    private static void ApplyBetaWarningLayout(MenuScreen menuScreen){
        var panel = menuScreen.transform.Find(GameAssemblyNames.MenuScreenHierarchy.BetaWarningPanel);

        if(!panel) return;

        panel.localPosition = new Vector3(0f, BetaWarningLocalY, 0f);
        panel.localScale    = Vector3.one * BetaWarningScale;

        CareerLogPlugin.Log?.LogInfo(PluginInfo.Format("Beta warning panel repositioned for menu layout."));
    }

    private static MenuButtonSnapshot CaptureVanillaSnapshot(MenuScreen menuScreen){
        if(_vanillaSnapshot != null) return _vanillaSnapshot;

        var player = MenuScreenReflection.GetPlayerButton(menuScreen)?.
                                          GetComponent<RectTransform>();

        if(!player) return null;

        var play = MenuScreenReflection.GetPlayButton(menuScreen)?.
                                        GetComponent<RectTransform>();
        var spacing = ResolvePlayToCharacterSpacing(play, player);

        _vanillaSnapshot = new MenuButtonSnapshot(play, player, spacing);

        return _vanillaSnapshot;
    }

    private static void ResetLayoutStateIfNeeded(MenuScreen menuScreen){
        var id = menuScreen.GetInstanceID();

        if(id == _layoutMenuScreenId) return;

        _layoutMenuScreenId = id;
        _layoutRevealReady  = false;
        _vanillaSnapshot    = null;
        _recordsButton      = null;
        StopRevealCoroutine();
    }

    private static float ResolvePlayToCharacterSpacing(RectTransform play, RectTransform player){
        if(!play || play.parent != player.parent) return VanillaSlotSpacingFallback;

        var playToCharacter = player.anchoredPosition.y - play.anchoredPosition.y;

        return !Mathf.Approximately(playToCharacter, 0f) ? playToCharacter : VanillaSlotSpacingFallback;
    }

    private static void ApplyStackButtonTransform(RectTransform target, RectTransform template){
        target.anchorMin  = template.anchorMin;
        target.anchorMax  = template.anchorMax;
        target.pivot      = template.pivot;
        target.sizeDelta  = template.sizeDelta;
        target.localScale = template.localScale;
    }

    private static float ResolvePitRecordsSlotOffset(
        RectTransform tradeRect, RectTransform hideoutRect, RectTransform playerRect
    ){
        var slotStep   = ResolveMenuSlotStep(tradeRect, hideoutRect, playerRect);
        var scaledStep = slotStep * SlotStepScale;

        return scaledStep + SlotExtraOffset;
    }

    private static float ResolveMenuSlotStep(
        RectTransform tradeRect, RectTransform hideoutRect, RectTransform playerRect
    ){
        if(tradeRect && hideoutRect){
            var tradeToHideout = tradeRect.anchoredPosition.y - hideoutRect.anchoredPosition.y;

            if(Mathf.Abs(tradeToHideout) > 1f) return Mathf.Abs(tradeToHideout);
        }

        if(!tradeRect || !playerRect) return playerRect ? Mathf.Max(80f, playerRect.rect.height + 10f) : 80f;

        var playerToTrade = playerRect.anchoredPosition.y - tradeRect.anchoredPosition.y;

        if(Mathf.Abs(playerToTrade) > 1f) return Mathf.Abs(playerToTrade);

        return playerRect ? Mathf.Max(80f, playerRect.rect.height + 10f) : 80f;
    }

    private static void ShiftButtonsAbovePlayer(RectTransform playRect, RectTransform playerRect, float slotStep){
        if(playRect) playRect.anchoredPosition += new Vector2(0f, slotStep);

        if(playerRect) playerRect.anchoredPosition += new Vector2(0f, slotStep);
    }

    private static void ApplyOverhaulTransform(RectTransform rectTransform, float anchoredY){
        rectTransform.anchorMin        = new Vector2(0f, 0.5f);
        rectTransform.anchorMax        = new Vector2(0f, 0.5f);
        rectTransform.pivot            = new Vector2(0f, 0.5f);
        rectTransform.localScale       = Vector3.one;
        rectTransform.anchoredPosition = new Vector2(OverhaulButtonX, anchoredY);
    }

    private static void PositionOverhaulButton(
        MenuScreen menuScreen, string buttonName, int index, float stackLift = 0f
    ){
        PositionOverhaulButtonAtY(menuScreen, buttonName, -index * OverhaulButtonYStep + stackLift);
    }

    private static void PositionOverhaulButtonAtY(MenuScreen menuScreen, string buttonName, float anchoredY){
        var rectTransform = MenuScreenReflection.GetHierarchyButtonRect(menuScreen, buttonName);

        if(!rectTransform) return;

        ApplyOverhaulTransform(rectTransform, anchoredY);
    }

    private static void ApplyMenuOverhaulVisuals(GameObject buttonObject, bool textOnly){
        SetChildActive(buttonObject, UiHierarchy.DefaultButton.Background, false);

        var sizeLabel = buttonObject.transform.Find(UiHierarchy.DefaultButton.SizeLabel);

        if(!sizeLabel) return;

        if(textOnly){
            SetChildActive(sizeLabel.gameObject, UiHierarchy.DefaultButton.IconContainer, false);

            return;
        }

        SetChildActive(sizeLabel.gameObject, UiHierarchy.DefaultButton.IconContainer, true);

        var icon = sizeLabel.Find($"{UiHierarchy.DefaultButton.IconContainer}/{UiHierarchy.DefaultButton.Icon}");

        if(!icon) return;

        icon.gameObject.SetActive(true);
        icon.localScale = Vector3.one * OverhaulIconScale;
    }

    private static void ConfigureVanillaAppearance(DefaultUIButton button, DefaultUIButton playerButton){
        if(!button || !playerButton) return;

        var fontSize = Mathf.Max(22, playerButton.HeaderSize - 2);

        button.SetRawText("RECORDS", fontSize);
        ApplyRecordsIcon(button, false);
        button.SetEnabledTooltip(string.Empty, true);
        button.SetDisabledTooltip(string.Empty);
        button.Interactable = true;

        ResetAnimatedButton(button);
    }

    private static void ApplyRecordsIcon(DefaultUIButton button, bool menuOverhaulLayout){
        if(!button) return;

        ButtonIconHelper.Apply(button, menuOverhaulLayout);
    }

    private static void ConfigureClickHandler(DefaultUIButton button){
        button.SetEnabledTooltip(string.Empty, true);
        button.SetDisabledTooltip(string.Empty);
        button.Interactable = true;

        button.OnClick.RemoveAllListeners();
        button.OnClick.AddListener(MenuContext.Open);

        button.OnMouseOver?.RemoveAllListeners();
        button.OnMouseOut?.RemoveAllListeners();
    }

    private static int ResolveCharacterFontSize(MenuScreen menuScreen){
        var character = MenuScreenReflection.GetCharacterButton(menuScreen);

        return !character ? 32 : Mathf.Max(28, character.HeaderSize);
    }

    private static void ResetAnimatedButton(DefaultUIButton button){
        var animated = button.GetComponent<TweenAnimatedButton>();

        if(!animated) return;

        animated.Interactable         = true;
        animated.transform.localScale = Vector3.one;
    }

    private static void SetChildActive(GameObject parent, string childName, bool active){
        if(!parent) return;

        var child = parent.transform.Find(childName);

        if(child) child.gameObject.SetActive(active);
    }

    private sealed class MenuButtonSnapshot{
        private readonly Dictionary<int, Vector2> _originalPositions = new();

        public MenuButtonSnapshot(RectTransform play, RectTransform player, float slotSpacing){
            Play        = play;
            Player      = player;
            SlotSpacing = slotSpacing;

            if(Play) _originalPositions[Play.GetInstanceID()] = Play.anchoredPosition;

            _originalPositions[player.GetInstanceID()] = player.anchoredPosition;
        }

        public RectTransform Play{
            get;
        }

        public RectTransform Player{
            get;
        }

        public float SlotSpacing{
            get;
        }

        public void Restore(){
            if(Play) Play.anchoredPosition = _originalPositions[Play.GetInstanceID()];

            if(Player) Player.anchoredPosition = _originalPositions[Player.GetInstanceID()];
        }
    }
}
