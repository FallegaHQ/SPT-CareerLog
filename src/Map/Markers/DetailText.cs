using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence.Models;
using Softwyx.CareerLog.Ui.Formatting;
using System;
using System.Text;

namespace Softwyx.CareerLog.Map.Markers;

/// <summary>Title and body copy for map marker popovers.</summary>
internal static class DetailText{
    public static bool IsKillFamily(RaidMovementValue marker){
        return marker?.Type is RaidMarkerTypes.Kill or RaidMarkerTypes.BossKill or RaidMarkerTypes.Killstreak;
    }

    public static bool TryGet(RaidMovementValue marker, RaidRecord raid, out string title, out string body){
        title = null;
        body  = null;

        if(marker is not{
                            IsMarker: true
                        })
            return false;

        var timeLabel = FormatRaidTime(marker.UtcOffsetSec);

        switch(marker.Type){
            case RaidMarkerTypes.Spawn:
                title = LocaleLoader.Format(LocaleKeys.MapMarkerSpawnTitle);
                body  = BuildSpawnExtractBody(marker, timeLabel);

                return true;
            case RaidMarkerTypes.Death:
            case RaidMarkerTypes.Extract:
                title = ExitOutcomeLabels.DisplayLabel(raid?.ExitStatus);
                body  = BuildOutcomeBody(raid?.ExitStatus, timeLabel, marker.ValueRub ?? 0L);

                return true;
            case RaidMarkerTypes.Kill:
                title = LocaleLoader.Format(LocaleKeys.MapMarkerKillTitle);
                body  = BuildKillBody(marker, timeLabel, true);

                return true;
            case RaidMarkerTypes.BossKill:
                title = LocaleLoader.Format(LocaleKeys.MapMarkerBossKillTitle);
                body  = BuildKillBody(marker, timeLabel, true);

                return true;
            case RaidMarkerTypes.Killstreak:
                title = LocaleLoader.Format(LocaleKeys.MapMarkerKillstreakTitle, marker.KillCount ?? 0);
                body  = BuildKillBody(marker, timeLabel, false);

                return true;
            case RaidMarkerTypes.Loot:
                title = LocaleLoader.Format(LocaleKeys.MapMarkerLootTitle);
                body  = BuildLootBody(marker, timeLabel);

                return true;
            case RaidMarkerTypes.Injury:
                title = LocaleLoader.Format(LocaleKeys.MapMarkerInjuryTitle);
                body  = BuildInjuryBody(marker, timeLabel);

                return true;
            case RaidMarkerTypes.Healing:
                title = LocaleLoader.Format(LocaleKeys.MapMarkerHealingTitle);
                body  = BuildHealingBody(marker, timeLabel);

                return true;
            case RaidMarkerTypes.Achievement:
                title = LocaleLoader.Format(LocaleKeys.MapMarkerAchievementTitle);
                body  = BuildAchievementBody(marker, timeLabel);

                return true;
            case RaidMarkerTypes.DoorUnlock:
                title = LocaleLoader.Format(LocaleKeys.MapMarkerDoorUnlockTitle);
                body  = BuildDoorUnlockBody(marker, timeLabel);

                return true;
            default:
                return false;
        }
    }

    private static string BuildKillBody(RaidMovementValue marker, string timeLabel, bool single){
        var builder = new StringBuilder();
        var victims = marker.Victims;

        if(single){
            var v = victims is{
                                  Count: > 0
                              }
                        ? victims[0]
                        : null;
            if(v != null && !string.IsNullOrEmpty(v.Name))
                builder.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerKillVictim, v.Name));

            if(v != null && !string.IsNullOrEmpty(v.BodyPart)) builder.AppendLine($"Hit: {v.BodyPart}");
        }
        else{
            builder.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerKillstreakVictims, marker.KillCount ?? 0));

            if(victims is{
                             Count: > 0
                         }){
                // Show a compact per-victim list; keep it bounded to avoid huge popovers.
                var max = Math.Min(8, victims.Count);

                for(var i = 0; i < max; i++){
                    var v = victims[i];

                    if(v == null || string.IsNullOrEmpty(v.Name)) continue;

                    var part = string.IsNullOrEmpty(v.BodyPart) ? "" : $" ({v.BodyPart})";
                    builder.AppendLine($"- {v.Name}{part}");
                }

                if(victims.Count > max)
                    builder.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerTruncatedMore, victims.Count - max));
            }
        }

        if((marker.KillDistance ?? 0f) > 0f)
            builder.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerKillDistance, marker.KillDistance ?? 0f));

        if(marker.LongestKill == true) builder.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerLongestKillNote));

        if(!string.IsNullOrEmpty(marker.WeaponTemplateId))
            builder.AppendLine(
                               LocaleLoader.Format(
                                                   LocaleKeys.MapMarkerKillWeapon,
                                                   ItemLocale.ShortName(marker.WeaponTemplateId)
                                                  )
                              );

        builder.Append(LocaleLoader.Format(LocaleKeys.MapMarkerEventTime, timeLabel));

        return builder.ToString().
                       Trim();
    }

    private static string BuildLootBody(RaidMovementValue marker, string timeLabel){
        var lines = new StringBuilder();

        if(marker.ItemTemplateIds is{
                                        Count: > 0
                                    }){
            var max = Math.Min(12, marker.ItemTemplateIds.Count);

            for(var i = 0; i < max; i++){
                var tpl = marker.ItemTemplateIds[i];

                if(string.IsNullOrEmpty(tpl)) continue;

                lines.AppendLine(ItemLocale.Name(tpl) ?? tpl);
            }

            if(marker.ItemTemplateIds.Count > max)
                lines.AppendLine(
                                 LocaleLoader.Format(
                                                     LocaleKeys.MapMarkerTruncatedMore,
                                                     marker.ItemTemplateIds.Count - max
                                                    )
                                );
        }
        else if(!string.IsNullOrEmpty(marker.ItemTemplateId)){
            lines.AppendLine(ItemLocale.Name(marker.ItemTemplateId));
        }
        else if(!string.IsNullOrEmpty(marker.Label)){
            lines.AppendLine(marker.Label);
        }

        if((marker.ValueRub ?? 0L) > 0)
            lines.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerLootValue, marker.ValueRub ?? 0L));

        lines.Append(LocaleLoader.Format(LocaleKeys.MapMarkerEventTime, timeLabel));

        return lines.ToString().
                     Trim();
    }

    private static string BuildInjuryBody(RaidMovementValue marker, string timeLabel){
        var parts = marker.BodyParts is{
                                           Count: > 0
                                       }
                        ? string.Join(", ", marker.BodyParts)
                        : string.Empty;

        var lines = new StringBuilder();

        if(!string.IsNullOrEmpty(parts)) lines.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerInjuryParts, parts));

        lines.Append(LocaleLoader.Format(LocaleKeys.MapMarkerEventTime, timeLabel));

        return lines.ToString().
                     Trim();
    }

    private static string BuildHealingBody(RaidMovementValue marker, string timeLabel){
        var kindKey = marker.HealKind == Collectors.Health.HealingMarkerCollector.KindSurgery
                          ? LocaleKeys.MapMarkerHealingSurgery
                          : LocaleKeys.MapMarkerHealingLarge;

        var lines = new StringBuilder();
        lines.AppendLine(LocaleLoader.Format(kindKey));

        if((marker.HealAmount ?? 0f) > 0f)
            lines.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerHealingAmount, marker.HealAmount ?? 0f));

        lines.Append(LocaleLoader.Format(LocaleKeys.MapMarkerEventTime, timeLabel));

        return lines.ToString().
                     Trim();
    }

    private static string BuildAchievementBody(RaidMovementValue marker, string timeLabel){
        var lines = new StringBuilder();

        if(!string.IsNullOrEmpty(marker.Label)) lines.AppendLine(marker.Label);

        lines.Append(LocaleLoader.Format(LocaleKeys.MapMarkerEventTime, timeLabel));

        return lines.ToString().
                     Trim();
    }

    private static string BuildDoorUnlockBody(RaidMovementValue marker, string timeLabel){
        var lines = new StringBuilder();

        if(!string.IsNullOrEmpty(marker.Label))
            lines.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerDoorUnlockId, marker.Label));

        if(!string.IsNullOrEmpty(marker.ItemTemplateId))
            lines.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerDoorUnlockKey, marker.ItemTemplateId));

        lines.Append(LocaleLoader.Format(LocaleKeys.MapMarkerEventTime, timeLabel));

        return lines.ToString().
                     Trim();
    }

    private static string BuildSpawnExtractBody(RaidMovementValue marker, string timeLabel){
        var lines = new StringBuilder();

        if(marker != null && (marker.ValueRub ?? 0L) > 0)
            lines.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerInventoryValue, marker.ValueRub ?? 0L));

        lines.Append(LocaleLoader.Format(LocaleKeys.MapMarkerEventTime, timeLabel));

        return lines.ToString().
                     Trim();
    }

    private static string BuildOutcomeBody(string exitStatus, string timeLabel, long valueRub){
        var note     = ExitOutcomeLabels.OutcomeNote(exitStatus);
        var timeLine = LocaleLoader.Format(LocaleKeys.MapMarkerEventTime, timeLabel);

        var lines = new StringBuilder();

        if(!string.IsNullOrEmpty(note)) lines.AppendLine(note);
        if(valueRub > 0) lines.AppendLine(LocaleLoader.Format(LocaleKeys.MapMarkerInventoryValue, valueRub));
        lines.Append(timeLine);

        return lines.ToString().
                     Trim();
    }

    private static string FormatRaidTime(float secondsFromStart){
        if(secondsFromStart < 0f) secondsFromStart = 0f;

        var span = TimeSpan.FromSeconds(secondsFromStart);

        return span.TotalHours >= 1
                   ? $"{(int) span.TotalHours}:{span.Minutes:D2}:{span.Seconds:D2}"
                   : $"{span.Minutes}:{span.Seconds:D2}";
    }
}
