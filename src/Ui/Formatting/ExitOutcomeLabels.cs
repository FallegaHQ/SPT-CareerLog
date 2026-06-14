using System;
using Softwyx.CareerLog.Interop;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Persistence;

namespace Softwyx.CareerLog.Ui.Formatting;

internal static class ExitOutcomeLabels{
    public static string DisplayLabel(string exitStatus){
        var category = RaidExitStatus.Classify(exitStatus);

        var fromGame = GameLocaleAccess.TryLocalize(GameKeysFor(category));

        if(!string.IsNullOrEmpty(fromGame)) return fromGame;

        if(category == RaidExitStatus.OutcomeCategory.Unknown && !string.IsNullOrEmpty(exitStatus)) return exitStatus;

        return LocaleLoader.Format(ModLabelKey(category));
    }

    public static string OutcomeNote(string exitStatus){
        var category = RaidExitStatus.Classify(exitStatus);

        if(category == RaidExitStatus.OutcomeCategory.Unknown) return string.Empty;

        var note = LocaleLoader.Format(ModNoteKey(category));

        return string.Equals(note, ModNoteKey(category), StringComparison.Ordinal) ? string.Empty : note;
    }

    private static string[] GameKeysFor(RaidExitStatus.OutcomeCategory category){
        return category switch{
                   RaidExitStatus.OutcomeCategory.Survived        =>["Survived", "ExpBonusSurvived"],
                   RaidExitStatus.OutcomeCategory.RunThrough      =>["ExpBonusRunner", "Ran Through"],
                   RaidExitStatus.OutcomeCategory.Killed          =>["Killed"],
                   RaidExitStatus.OutcomeCategory.MissingInAction =>["ExpBonusMissingInAction", "Missed in Action"],
                   RaidExitStatus.OutcomeCategory.Left            =>["Leave", "Left"],
                   RaidExitStatus.OutcomeCategory.Transit         =>["Transit"],
                   _                                              => null
               };
    }

    private static string ModLabelKey(RaidExitStatus.OutcomeCategory category){
        return category switch{
                   RaidExitStatus.OutcomeCategory.Survived        => LocaleKeys.ExitSurvived,
                   RaidExitStatus.OutcomeCategory.RunThrough      => LocaleKeys.ExitRunThrough,
                   RaidExitStatus.OutcomeCategory.Killed          => LocaleKeys.ExitKilled,
                   RaidExitStatus.OutcomeCategory.MissingInAction => LocaleKeys.ExitMissingInAction,
                   RaidExitStatus.OutcomeCategory.Left            => LocaleKeys.ExitLeft,
                   RaidExitStatus.OutcomeCategory.Transit         => LocaleKeys.ExitTransit,
                   _                                              => LocaleKeys.ExitUnknown
               };
    }

    private static string ModNoteKey(RaidExitStatus.OutcomeCategory category){
        return category switch{
                   RaidExitStatus.OutcomeCategory.Survived        => LocaleKeys.ExitNoteSurvived,
                   RaidExitStatus.OutcomeCategory.RunThrough      => LocaleKeys.ExitNoteRunThrough,
                   RaidExitStatus.OutcomeCategory.Killed          => LocaleKeys.ExitNoteKilled,
                   RaidExitStatus.OutcomeCategory.MissingInAction => LocaleKeys.ExitNoteMissingInAction,
                   RaidExitStatus.OutcomeCategory.Left            => LocaleKeys.ExitNoteLeft,
                   RaidExitStatus.OutcomeCategory.Transit         => LocaleKeys.ExitNoteTransit,
                   _                                              => LocaleKeys.ExitNoteUnknown
               };
    }
}
