using BepInEx.Configuration;
using Softwyx.CareerLog.Collectors.Stash;
using Softwyx.CareerLog.Persistence;
using Softwyx.CareerLog.Persistence.Financial;
using System;
using System.Collections.Generic;

namespace Softwyx.CareerLog.Config;

internal static class Settings{
    private const string GeneralSection    = "1. General";
    private const string UiSection         = "2. UI";
    private const string MovementSection   = "3. Movement & maps";
    private const string CollectorsSection = "4. Event collectors";
    private const string FinancialSection  = "5. Financial";

    public static  ConfigEntry<bool> Enabled;
    private static ConfigEntry<bool> _showPostCareerLogInChain;
    public static  ConfigEntry<bool> ShowRecordsMenuButton;
    // ReSharper disable once MemberCanBePrivate.Global
    public static ConfigEntry<string> DefaultSideFilter;

    public static ConfigEntry<float> MovementSampleIntervalSec;
    public static ConfigEntry<float> MovementDouglasPeuckerEpsilonMeters;
    public static ConfigEntry<bool>  OptimizeMovementTrail;
    public static ConfigEntry<bool>  CompressJson;
    public static ConfigEntry<bool>  RestoreVanillaStatistics;

    public static ConfigEntry<float>  KillstreakMaxGapSec;
    public static ConfigEntry<int>    KillstreakMinKills;
    public static ConfigEntry<float>  InjuryMergeWindowSec;
    public static ConfigEntry<long>   LootMarkerMinValueRub;
    public static ConfigEntry<string> LootMarkerTemplateIds;
    public static ConfigEntry<string> LootMarkerCategories;
    public static ConfigEntry<float>  HealingLargeRestorePercent;
    public static ConfigEntry<float>  LootMarkerMergeMaxGapSec;
    public static ConfigEntry<float>  LootMarkerMergeDistanceFraction;

    public static  ConfigEntry<bool>   StashSnapshotOnMenuOpen;
    private static ConfigEntry<string> _financialChartDefaultStyle;
    public static  ConfigEntry<string> FinancialTopItemExcludedTemplateIds;
    public static  ConfigEntry<string> FinancialTopItemExcludedCategories;
    public static  ConfigEntry<bool>   FinancialTopItemExcludeSecureContainers;

    // ReSharper disable once CollectionNeverQueried.Local
    private static readonly List<ConfigEntryBase> Entries = [];

    public static void Init(ConfigFile config){
        Entries.Clear();

        Enabled = config.Bind(GeneralSection, "Enabled", true, new ConfigDescription("Master toggle.", null, Basic()));

        _showPostCareerLogInChain = config.Bind(
                                                UiSection,
                                                "Show post-raid debrief in chain",
                                                true,
                                                new ConfigDescription(
                                                                      "Insert the post-raid debrief screen.",
                                                                      null,
                                                                      Basic()
                                                                     )
                                               );

        ShowRecordsMenuButton = config.Bind(
                                            UiSection,
                                            "Show Records menu button",
                                            true,
                                            new ConfigDescription(
                                                                  "Show the RECORDS button on the main menu.",
                                                                  null,
                                                                  Basic()
                                                                 )
                                           );

        DefaultSideFilter = config.Bind(
                                        UiSection,
                                        "Records side filter",
                                        SideFilterValues.Combined,
                                        new ConfigDescription(
                                                              "Default RECORDS filter: Combined, Pmc, or Scav. Change this using the filter in Records screen.",
                                                              null,
                                                              ReadOnly()
                                                             )
                                       );

        Entries.Add(Enabled);
        Entries.Add(_showPostCareerLogInChain);
        Entries.Add(ShowRecordsMenuButton);
        MovementSampleIntervalSec = config.Bind(
                                                MovementSection,
                                                "Movement sample interval (seconds)",
                                                0.6f,
                                                new ConfigDescription(
                                                                      "How often to record player position during a raid."
                                                                    + " The smaller value the clearer trail path.",
                                                                      new AcceptableValueRange<float>(0.4f, 5f),
                                                                      ConfigFloatUi.Attributes(false, step: 0.2f)
                                                                     )
                                               );

        MovementDouglasPeuckerEpsilonMeters = config.Bind(
                                                          MovementSection,
                                                          "Trail simplification epsilon (meters)",
                                                          0.3f,
                                                          new ConfigDescription(
                                                                                "Douglas–Peucker tolerance in world XZ meters before saving the trail.",
                                                                                new AcceptableValueRange<float>(
                                                                                     0.2f,
                                                                                     2f
                                                                                    ),
                                                                                ConfigFloatUi.Attributes(
                                                                                     true,
                                                                                     step: 0.1f
                                                                                    )
                                                                               )
                                                         );

        OptimizeMovementTrail = config.Bind(
                                            MovementSection,
                                            "Optimize movement trail",
                                            false,
                                            new ConfigDescription(
                                                                  "Reduce saved movement points before persisting. Disable to save raw samples.",
                                                                  null,
                                                                  Basic()
                                                                 )
                                           );

        CompressJson = config.Bind(
                                   GeneralSection,
                                   "Minify JSON files",
                                   true,
                                   new ConfigDescription(
                                                         "Write minified JSON for raid/profile saves (slightly smaller files).",
                                                         null,
                                                         Basic()
                                                        )
                                  );

        RestoreVanillaStatistics = config.Bind(
                                               MovementSection,
                                               "Restore vanilla statistics screen",
                                               false,
                                               new ConfigDescription(
                                                                     "Show the vanilla session statistics screen after the debrief"
                                                                   + " (can be enabled alongside mod's post-raid screens).",
                                                                     null,
                                                                     Basic()
                                                                    )
                                              );

        Entries.Add(DefaultSideFilter);
        Entries.Add(MovementSampleIntervalSec);
        Entries.Add(MovementDouglasPeuckerEpsilonMeters);
        Entries.Add(OptimizeMovementTrail);
        Entries.Add(CompressJson);
        Entries.Add(RestoreVanillaStatistics);

        KillstreakMaxGapSec = config.Bind(
                                          CollectorsSection,
                                          "Killstreak max gap (seconds)",
                                          30f,
                                          new ConfigDescription(
                                                                "Max seconds between consecutive kills to stay in one streak.",
                                                                new AcceptableValueRange<float>(20f, 240f),
                                                                ConfigFloatUi.Attributes(false, step: 10f)
                                                               )
                                         );

        KillstreakMinKills = config.Bind(
                                         CollectorsSection,
                                         "Killstreak minimum kills",
                                         4,
                                         new ConfigDescription(
                                                               "Kills required before merging into a killstreak marker.",
                                                               new AcceptableValueRange<int>(3, 10),
                                                               ConfigIntegralUi.IntAttributes(false, 1)
                                                              )
                                        );

        InjuryMergeWindowSec = config.Bind(
                                           CollectorsSection,
                                           "Injury merge window (seconds)",
                                           15f,
                                           new ConfigDescription(
                                                                 "Merge blacked-limb markers within this many seconds.",
                                                                 new AcceptableValueRange<float>(10f, 60f),
                                                                 ConfigFloatUi.Attributes(false, step: 5f)
                                                                )
                                          );

        LootMarkerMinValueRub = config.Bind(
                                            CollectorsSection,
                                            "Loot marker min value (₽)",
                                            50000L,
                                            new ConfigDescription(
                                                                  "Show a loot marker when handbook value is at least this.",
                                                                  new AcceptableValueRange<long>(30000L, 2500000L),
                                                                  ConfigIntegralUi.LongAttributes(false, 20000L)
                                                                 )
                                           );

        LootMarkerTemplateIds = config.Bind(
                                            CollectorsSection,
                                            "Loot marker template ids",
                                            string.Empty,
                                            new ConfigDescription(
                                                                  "Comma-separated item template ids that always get a loot marker.",
                                                                  null,
                                                                  Advanced()
                                                                 )
                                           );

        LootMarkerCategories = config.Bind(
                                           CollectorsSection,
                                           "Loot marker category tokens",
                                           string.Empty,
                                           new ConfigDescription(
                                                                 "Comma-separated parent-id substrings (e.g. keys, meds) for loot markers.",
                                                                 null,
                                                                 Advanced()
                                                                )
                                          );

        HealingLargeRestorePercent = config.Bind(
                                                 CollectorsSection,
                                                 "Healing large restore (%)",
                                                 20f,
                                                 new ConfigDescription(
                                                                       "Single heal event at or above this % of raid-start max body HP.",
                                                                       new AcceptableValueRange<float>(10f, 60f),
                                                                       ConfigFloatUi.Attributes(true, step: 5f)
                                                                      )
                                                );

        Entries.Add(KillstreakMaxGapSec);
        Entries.Add(KillstreakMinKills);
        Entries.Add(InjuryMergeWindowSec);
        Entries.Add(LootMarkerMinValueRub);
        Entries.Add(LootMarkerTemplateIds);
        Entries.Add(LootMarkerCategories);
        Entries.Add(HealingLargeRestorePercent);

        LootMarkerMergeMaxGapSec = config.Bind(
                                               CollectorsSection,
                                               "Loot marker merge max gap (seconds)",
                                               30f,
                                               new ConfigDescription(
                                                                     "Merge nearby loot markers if they occur within this time gap.",
                                                                     new AcceptableValueRange<float>(10f, 60f),
                                                                     ConfigFloatUi.Attributes(false, step: 5f)
                                                                    )
                                              );

        LootMarkerMergeDistanceFraction = config.Bind(
                                                      CollectorsSection,
                                                      "Loot marker merge distance (map fraction)",
                                                      0.02f,
                                                      new ConfigDescription(
                                                                            "Merge loot markers within this fraction of the map diagonal (map dependent).",
                                                                            new AcceptableValueRange<float>(
                                                                                 0.001f,
                                                                                 0.10f
                                                                                ),
                                                                            ConfigFloatUi.Attributes(true, step: 0.001f)
                                                                           )
                                                     );

        Entries.Add(LootMarkerMergeMaxGapSec);
        Entries.Add(LootMarkerMergeDistanceFraction);

        StashSnapshotOnMenuOpen = config.Bind(
                                              FinancialSection,
                                              "Capture stash snapshot on menu open",
                                              true,
                                              new ConfigDescription(
                                                                    "Append a stash worth snapshot when returning to the main menu.",
                                                                    null,
                                                                    Basic()
                                                                   )
                                             );

        _financialChartDefaultStyle = config.Bind(
                                                  FinancialSection,
                                                  "Financial chart default style",
                                                  "Line",
                                                  new ConfigDescription(
                                                                        "Default chart style on the Financial tab: Line or Bar.",
                                                                        null,
                                                                        Basic()
                                                                       )
                                                 );

        FinancialTopItemExcludedTemplateIds = config.Bind(
                                                          FinancialSection,
                                                          "Top items excluded template ids",
                                                          TopItemExclusion.DefaultLegaMedalTemplateId,
                                                          new ConfigDescription(
                                                                                "Comma-separated item template ids excluded from stash top-items lists.",
                                                                                null,
                                                                                Advanced()
                                                                               )
                                                         );

        FinancialTopItemExcludedCategories = config.Bind(
                                                         FinancialSection,
                                                         "Top items excluded category tokens",
                                                         string.Empty,
                                                         new ConfigDescription(
                                                                               "Comma-separated parent-id substrings excluded from top-items lists (e.g. meds, keys).",
                                                                               null,
                                                                               Advanced()
                                                                              )
                                                        );

        FinancialTopItemExcludeSecureContainers = config.Bind(
                                                              FinancialSection,
                                                              "Top items exclude secure containers",
                                                              true,
                                                              new ConfigDescription(
                                                                   "Exclude secured containers (Alpha, Gamma, Kappa, etc.) from top-items lists.",
                                                                   null,
                                                                   Basic()
                                                                  )
                                                             );

        Entries.Add(StashSnapshotOnMenuOpen);
        Entries.Add(_financialChartDefaultStyle);
        Entries.Add(FinancialTopItemExcludedTemplateIds);
        Entries.Add(FinancialTopItemExcludedCategories);
        Entries.Add(FinancialTopItemExcludeSecureContainers);
    }

    public static SideFilter ParseSideFilter(){
        return SideFilterValues.Parse(DefaultSideFilter?.Value);
    }

    public static ChartStyle ParseChartStyle(){
        var raw = _financialChartDefaultStyle?.Value;

        return string.Equals(raw, "Bar", StringComparison.OrdinalIgnoreCase) ? ChartStyle.Bar : ChartStyle.Line;
    }

    private static ConfigurationManagerAttributes Basic(){
        return new ConfigurationManagerAttributes{
                                                     Order = 0
                                                 };
    }

    private static ConfigurationManagerAttributes ReadOnly(){
        return new ConfigurationManagerAttributes{
                                                     IsAdvanced = true,
                                                     ReadOnly   = true
                                                 };
    }

    private static ConfigurationManagerAttributes Advanced(){
        return new ConfigurationManagerAttributes{
                                                     IsAdvanced = true
                                                 };
    }
}
