using System;
using System.Collections.Generic;
using EFT;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Formatting;
using Softwyx.CareerLog.Ui.Records.Financial;
using Softwyx.CareerLog.Ui.Records.Layout;
using Softwyx.CareerLog.Ui.Records.Overview;
using Softwyx.CareerLog.Ui.Shared;
using TMPro;
using UnityEngine;

namespace Softwyx.CareerLog.Ui.Records.Content;

internal static class TabContentBuilder{
    public static void Rebuild(
        PanelRegions       panels, Profile profile, TextMeshProUGUI styleSource, Action<string> onRaidSelected = null,
        Action             onRaidBack = null, Action<int> onRaidsPageChanged = null, Action onRaidMap = null,
        Action<SideFilter> onSideFilterChanged = null, Action onFinancialChanged = null,
        Action<int>        onFinancialTablePageChanged = null, Action<StashSnapshot> onFinancialSnapshotSelected = null,
        Action             onOverviewOpen = null
    ){
        if(!panels.IsValid) return;

        var style = ScrollContentBuilder.ResolveStyle(styleSource);

        RebuildLeftSummary(panels.LeftStatsRoot, profile, style, onSideFilterChanged, onOverviewOpen);
        RebuildActiveTab(
                         panels.RightTabHostRoot,
                         panels.RightTabContentRoot,
                         profile,
                         styleSource,
                         onRaidSelected,
                         onRaidBack,
                         onRaidsPageChanged,
                         onRaidMap,
                         onFinancialChanged,
                         onFinancialTablePageChanged,
                         onFinancialSnapshotSelected
                        );
    }

    public static void RebuildActiveTab(
        Transform hostRoot, Transform tabContentRoot, Profile profile, TextMeshProUGUI styleSource,
        Action<string> onRaidSelected = null, Action onRaidBack = null, Action<int> onRaidsPageChanged = null,
        Action onRaidMap = null, Action onFinancialChanged = null, Action<int> onFinancialTablePageChanged = null,
        Action<StashSnapshot> onFinancialSnapshotSelected = null
    ){
        if(!tabContentRoot) return;

        PanelLayout.RunContentRebuild(
                                      hostRoot,
                                      tabContentRoot,
                                      () => BuildActiveTabContent(
                                                                  hostRoot,
                                                                  tabContentRoot,
                                                                  profile,
                                                                  styleSource,
                                                                  onRaidSelected,
                                                                  onRaidBack,
                                                                  onRaidsPageChanged,
                                                                  onRaidMap,
                                                                  onFinancialChanged,
                                                                  onFinancialTablePageChanged,
                                                                  onFinancialSnapshotSelected
                                                                 )
                                     );
    }

    private static void BuildActiveTabContent(
        Transform hostRoot, Transform tabContentRoot, Profile profile, TextMeshProUGUI styleSource,
        Action<string> onRaidSelected = null, Action onRaidBack = null, Action<int> onRaidsPageChanged = null,
        Action onRaidMap = null, Action onFinancialChanged = null, Action<int> onFinancialTablePageChanged = null,
        Action<StashSnapshot> onFinancialSnapshotSelected = null
    ){
        var style = ScrollContentBuilder.ResolveStyle(styleSource);
        PrepareColumn(tabContentRoot);
        ScrollContentBuilder.Clear(tabContentRoot);

        if(profile == null){
            TabHeaderView.Apply(hostRoot, LocaleKeys.SectionRaids, styleSource, false, null);

            var items = ScrollContentBuilder.AddItemsPanel(tabContentRoot);
            ScrollContentBuilder.AddLocalizedBodyLine(items, LocaleKeys.ProfileUnavailable, style);

            return;
        }

        var data          = ProfileRecordsReadApi.Get(profile.ProfileId);
        var filteredRaids = data.FilterRaids(NavigationState.ActiveSideFilter);

        switch(NavigationState.ActiveTab){
            case Tab.Raids:{
                var inRaidDetail = !string.IsNullOrEmpty(NavigationState.SelectedRaidId);
                TabHeaderView.Apply(
                                    hostRoot,
                                    inRaidDetail ? LocaleKeys.SectionRaidDetail : LocaleKeys.SectionRaids,
                                    styleSource,
                                    inRaidDetail,
                                    () => onRaidBack?.Invoke(),
                                    inRaidDetail,
                                    () => onRaidMap?.Invoke()
                                   );
                BuildRaidsSection(
                                  tabContentRoot,
                                  profile.ProfileId,
                                  filteredRaids,
                                  style,
                                  onRaidSelected,
                                  onRaidsPageChanged
                                 );

                break;
            }
            case Tab.Summary:
                TabHeaderView.Apply(hostRoot, LocaleKeys.SectionSummary, styleSource, false, null);
                BuildSummarySection(tabContentRoot, SnapshotsReadApi.GetSummary(profile.ProfileId), style);

                break;
            case Tab.Financial:
                TabHeaderView.Apply(hostRoot, LocaleKeys.SectionFinancial, styleSource, false, null);
                BuildFinancialSection(
                                      tabContentRoot,
                                      profile,
                                      style,
                                      onFinancialChanged,
                                      onFinancialTablePageChanged,
                                      onFinancialSnapshotSelected
                                     );

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void Clear(PanelRegions panels){
        if(panels.LeftStatsRoot) ScrollContentBuilder.Clear(panels.LeftStatsRoot);

        if(panels.RightTabContentRoot) ScrollContentBuilder.Clear(panels.RightTabContentRoot);
    }

    private static void RebuildLeftSummary(
        Transform          leftRoot,                   Profile profile, ScrollContentBuilder.ScrollTextStyle style,
        Action<SideFilter> onSideFilterChanged = null, Action  onOverviewOpen = null
    ){
        if(!leftRoot) return;

        if(profile == null){
            LeftPanelOverviewView.Build(leftRoot, null, style, onSideFilterChanged, onOverviewOpen);

            return;
        }

        var data          = ProfileRecordsReadApi.Get(profile.ProfileId);
        var filteredRaids = data.FilterRaids(NavigationState.ActiveSideFilter);
        var lifetime      = data.GetLifetimeForEntries(filteredRaids, NavigationState.ActiveSideFilter);

        LeftPanelOverviewView.Build(leftRoot, lifetime, style, onSideFilterChanged, onOverviewOpen);
    }

    private static void PrepareColumn(Transform contentRoot){
        if(!contentRoot) return;

        ScrollContentBuilder.EnsureContentLayout(contentRoot);
    }

    private static void BuildRaidsSection(
        Transform                            contentRoot, string         profileId, IReadOnlyList<RaidIndexEntry> raids,
        ScrollContentBuilder.ScrollTextStyle style,       Action<string> onRaidSelected, Action<int> onRaidsPageChanged
    ){
        if(raids == null || raids.Count == 0){
            var items = ScrollContentBuilder.AddItemsPanel(contentRoot);
            ScrollContentBuilder.AddLocalizedBodyLine(items, LocaleKeys.NoRaids, style);
            NavigationState.ClearRaidSelection();

            return;
        }

        var selectedId = NavigationState.SelectedRaidId;

        if(!string.IsNullOrEmpty(selectedId) && !RaidsContainId(raids, selectedId))
            NavigationState.ClearRaidSelection();

        selectedId = NavigationState.SelectedRaidId;

        if(!string.IsNullOrEmpty(selectedId)){
            BuildRaidDetailSection(contentRoot, profileId, selectedId, style);

            return;
        }

        if(onRaidSelected == null){
            var readOnlyItems = ScrollContentBuilder.AddItemsPanel(contentRoot);

            foreach(var raid in raids)
                ScrollContentBuilder.AddBodyLine(readOnlyItems, RaidSummaryFormatter.FormatRaidListLine(raid), style);

            return;
        }

        BuildRaidsListSection(contentRoot, raids, style, onRaidSelected, onRaidsPageChanged);
    }

    private static void BuildRaidsListSection(
        Transform      contentRoot,    IReadOnlyList<RaidIndexEntry> raids, ScrollContentBuilder.ScrollTextStyle style,
        Action<string> onRaidSelected, Action<int>                   onRaidsPageChanged
    ){
        var items = ScrollContentBuilder.AddItemsPanel(contentRoot);

        ScrollContentBuilder.AddLocalizedBodyLine(items, LocaleKeys.RaidDetailHint, style);

        const int pageSize  = NavigationState.RaidsPageSize;
        var       pageCount = (raids.Count + pageSize - 1) / pageSize;
        var       pageIndex = NavigationState.RaidsListPage;

        if(pageIndex >= pageCount) NavigationState.SetRaidsPage(pageCount - 1);

        pageIndex = NavigationState.RaidsListPage;
        var start = pageIndex * pageSize;
        var end   = Mathf.Min(start + pageSize, raids.Count);

        for(var i = start; i < end; i++) RowView.Add(items, raids[i], onRaidSelected);

        if(pageCount > 1 && onRaidsPageChanged != null)
            PaginationView.Add(items, pageIndex, pageCount, style, onRaidsPageChanged);
    }

    private static bool RaidsContainId(IReadOnlyList<RaidIndexEntry> raids, string raidId){
        foreach(var raid in raids)
            if(raid != null && string.Equals(raid.RaidId, raidId, StringComparison.Ordinal))
                return true;

        return false;
    }

    private static void BuildRaidDetailSection(
        Transform contentRoot, string profileId, string raidId, ScrollContentBuilder.ScrollTextStyle style
    ){
        var raid = ProfileRecordsReadApi.LoadRaid(profileId, raidId);

        var items = ScrollContentBuilder.AddItemsPanel(contentRoot);

        if(raid == null){
            ScrollContentBuilder.AddLocalizedBodyLine(items, LocaleKeys.RaidDetailMissing, style);

            return;
        }

        ScrollContentBuilder.AddBodyLine(items, RaidSummaryFormatter.FormatRaidHeader(raid), style);

        var outcomeNote = RaidSummaryFormatter.FormatRaidOutcomeNote(raid);

        if(!string.IsNullOrEmpty(outcomeNote)) ScrollContentBuilder.AddBodyLine(items, outcomeNote, style);

        LootSummarySection.Add(contentRoot, raid, style);

        var counterSection = ScrollContentBuilder.AddSection(contentRoot, LocaleKeys.SectionSessionCounters, style);
        var counterItems   = ScrollContentBuilder.GetItemsContainer(counterSection);

        CounterRows.AddCategorizedRaidCounters(counterItems, raid, style);
    }

    private static void BuildSummarySection(
        Transform contentRoot, SnapshotSummaryView summary, ScrollContentBuilder.ScrollTextStyle style
    ){
        SummaryTabContent.Build(contentRoot, summary, style);
    }

    private static void BuildFinancialSection(
        Transform   contentRoot, Profile profile, ScrollContentBuilder.ScrollTextStyle style, Action onFinancialChanged,
        Action<int> onFinancialTablePageChanged, Action<StashSnapshot> onFinancialSnapshotSelected
    ){
        if(profile == null){
            var items = ScrollContentBuilder.AddItemsPanel(contentRoot);
            ScrollContentBuilder.AddLocalizedBodyLine(items, LocaleKeys.ProfileUnavailable, style);

            return;
        }

        TabContent.Build(
                         contentRoot,
                         profile.ProfileId,
                         style,
                         onFinancialChanged,
                         onFinancialTablePageChanged,
                         onFinancialSnapshotSelected
                        );
    }
}
