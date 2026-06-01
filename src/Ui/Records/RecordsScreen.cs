using Comfort.Common;
using EFT;
using EFT.HandBook;
using EFT.InputSystem;
using EFT.UI;
using EFT.UI.Screens;
using Softwyx.CareerLog.Infrastructure;
using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Persistence;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Records.Content;
using Softwyx.CareerLog.Ui.Records.Financial;
using Softwyx.CareerLog.Ui.Records.Layout;
using Softwyx.CareerLog.Ui.Records.Menu;
using Softwyx.CareerLog.Ui.Records.Map;
using Softwyx.CareerLog.Ui.Records.Overview;
using Softwyx.CareerLog.Ui.Records.PlayerModel;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records;

internal sealed class RecordsScreen : EftScreen<RecordsScreenController, RecordsScreen>{
    private DefaultUIButton _backButton;
    private TextMeshProUGUI _captionText;
    private PanelRegions    _panels;
    private TabBarView      _tabBar;
    private Transform       _screenRoot;
    private Profile         _profile;

    internal static RecordsScreen CreateFromHandbookTemplate(HandbookScreen template){
        if(!template) return null;

        var clone = Instantiate(template.gameObject, template.transform.parent);
        clone.name = UiHierarchy.RecordsScreen.ScreenObjectName;
        clone.SetActive(false);

        var handbook = clone.GetComponent<HandbookScreen>();
        var backButton = EftScreenFieldBinder.GetField<DefaultUIButton>(
                                                                        handbook,
                                                                        GameAssemblyNames.HandbookScreenFields.
                                                                            BackButton
                                                                       );

        Destroy(handbook);

        ScreenLayoutConfigurer.Apply(clone.transform);

        var records = clone.AddComponent<RecordsScreen>();
        records.BindFromMenu(backButton, clone.transform);

        return records;
    }

    private void BindFromMenu(DefaultUIButton backButton, Transform root){
        _screenRoot  = root;
        _backButton  = backButton;
        _captionText = CaptionHelper.Apply(root);
        _panels      = LeftPanelLayout.Resolve(root);
        _tabBar      = new TabBarView(_panels.TabBarRoot, OnTabSelected);
        _tabBar.EnsureBuilt();

        ScrollSettings.ApplyClampedToDescendants(root);

        WireBackButton();
    }

    private void Awake(){
        WireBackButton();
    }

    public override void Show(RecordsScreenController controller){
        Show(controller.Profile);
    }

    private void Show(Profile profile){
        _profile = profile;
        NavigationState.Reset();
        ShowGameObject();

        _captionText = CaptionHelper.Apply(_screenRoot) ?? _captionText;
        _panels      = LeftPanelLayout.Resolve(_screenRoot);

        if(_backButton) _backButton.Interactable = true;

        _tabBar?.RefreshHighlight();

        if(_panels.RightTabContentRoot) ScrollHost.SetScrollChainActive(_panels.RightTabContentRoot, true);

        ScrollSettings.ApplyClampedToDescendants(_screenRoot);
        TabContentBuilder.Rebuild(
                                  _panels,
                                  profile,
                                  _captionText,
                                  OnRaidSelected,
                                  OnRaidBack,
                                  OnRaidsPageChanged,
                                  OnRaidMapOpen,
                                  OnSideFilterChanged,
                                  OnFinancialChanged,
                                  OnFinancialTablePageChanged,
                                  OnFinancialSnapshotSelected,
                                  OnOverviewOpen
                                 );
        PanelLayout.Refresh(_panels);
        CharacterModelPresenter.EnsureInstalled(_screenRoot);
        CharacterModelPresenter.RequestShow(this, profile);
    }

    public override ETranslateResult TranslateCommand(ECommand command){
        if(!command.IsCommand(ECommand.Escape)) return GetDefaultBlockResult(command);

        if(SnapshotModal.IsOpen){
            SnapshotModal.Hide();
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuEscape);

            return ETranslateResult.BlockAll;
        }

        if(ProfileOverviewModal.IsOpen){
            ProfileOverviewModal.Hide();
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuEscape);

            return ETranslateResult.BlockAll;
        }

        if(MapModal.IsOpen){
            MapModal.Hide();
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuEscape);

            return ETranslateResult.BlockAll;
        }

        Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuEscape);
        ScreenController.CloseScreen();

        return ETranslateResult.BlockAll;
    }

    public override void Close(){
        SnapshotModal.Hide();
        ProfileOverviewModal.Hide();
        MapModal.Hide();
        CharacterModelPresenter.Hide();
        TabContentBuilder.Clear(_panels);
        ProfileRecordsReadApi.Clear();
        NavigationState.Reset();
        base.Close();
    }

    public override void OnDestroy(){
        CharacterModelPresenter.Release();
        base.OnDestroy();
    }

    private void OnTabSelected(Tab recordsTab){
        NavigationState.SetTab(recordsTab);
        _tabBar?.RefreshHighlight();

        if(_panels.RightTabContentRoot)
            TabContentBuilder.RebuildActiveTab(
                                               _panels.RightTabHostRoot,
                                               _panels.RightTabContentRoot,
                                               _profile,
                                               _captionText,
                                               OnRaidSelected,
                                               OnRaidBack,
                                               OnRaidsPageChanged,
                                               OnRaidMapOpen,
                                               OnFinancialChanged,
                                               OnFinancialTablePageChanged,
                                               OnFinancialSnapshotSelected
                                              );
    }

    private void OnSideFilterChanged(SideFilter filter){
        if(NavigationState.ActiveSideFilter == filter) return;

        NavigationState.SetSideFilter(filter);
        RebuildAll();
    }

    private void OnRaidSelected(string raidId){
        NavigationState.SelectRaid(raidId);
        RebuildRaidsTab();
    }

    private void OnRaidBack(){
        MapModal.Hide();
        NavigationState.ClearRaidSelection();
        RebuildRaidsTab();
    }

    private void OnRaidMapOpen(){
        if(_profile == null || string.IsNullOrEmpty(NavigationState.SelectedRaidId)) return;

        var raid = ProfileRecordsReadApi.LoadRaid(_profile.ProfileId, NavigationState.SelectedRaidId);

        if(raid == null) return;

        MapModal.Show(_screenRoot, raid, _captionText);
    }

    private void OnRaidsPageChanged(int pageIndex){
        NavigationState.SetRaidsPage(pageIndex);
        RebuildRaidsTab();
    }

    private void OnFinancialChanged(){
        RebuildActiveTabOnly();
    }

    private void OnFinancialTablePageChanged(int pageIndex){
        NavigationState.SetFinancialTablePage(pageIndex);
        RebuildActiveTabOnly();
    }

    private void OnFinancialSnapshotSelected(StashSnapshot snapshot){
        if(snapshot == null) return;

        SnapshotModal.Show(_screenRoot, snapshot, _captionText);
    }

    private void OnOverviewOpen(){
        if(_profile == null) return;

        var data          = ProfileRecordsReadApi.Get(_profile.ProfileId);
        var filteredRaids = data.FilterRaids(NavigationState.ActiveSideFilter);
        var lifetime      = data.GetLifetimeForEntries(filteredRaids, NavigationState.ActiveSideFilter);

        ProfileOverviewModal.Show(_screenRoot, lifetime, _captionText);
    }

    private void RebuildRaidsTab(){
        RebuildActiveTabOnly();
    }

    private void RebuildActiveTabOnly(){
        if(_panels.RightTabContentRoot)
            TabContentBuilder.RebuildActiveTab(
                                               _panels.RightTabHostRoot,
                                               _panels.RightTabContentRoot,
                                               _profile,
                                               _captionText,
                                               OnRaidSelected,
                                               OnRaidBack,
                                               OnRaidsPageChanged,
                                               OnRaidMapOpen,
                                               OnFinancialChanged,
                                               OnFinancialTablePageChanged,
                                               OnFinancialSnapshotSelected
                                              );
    }

    private void RebuildAll(){
        if(!_panels.IsValid) return;

        TabContentBuilder.Rebuild(
                                  _panels,
                                  _profile,
                                  _captionText,
                                  OnRaidSelected,
                                  OnRaidBack,
                                  OnRaidsPageChanged,
                                  OnRaidMapOpen,
                                  OnSideFilterChanged,
                                  OnFinancialChanged,
                                  OnFinancialTablePageChanged,
                                  OnFinancialSnapshotSelected,
                                  OnOverviewOpen
                                 );
        PanelLayout.Refresh(_panels);
    }

    private void WireBackButton(){
        if(!_backButton) return;

        _backButton.OnClick.RemoveAllListeners();
        _backButton.OnClick.AddListener(OnBackClicked);
    }

    private void OnBackClicked(){
        ScreenController.CloseScreen();
    }
}
