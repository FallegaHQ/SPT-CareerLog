using System;
using EFT;

namespace Softwyx.CareerLog.Persistence;

internal static class RaidExitStatus{
    public static OutcomeCategory Classify(string exitStatus){
        if(string.IsNullOrEmpty(exitStatus) || !Enum.TryParse(exitStatus, true, out ExitStatus parsed))
            return OutcomeCategory.Unknown;

        return parsed switch{
                   ExitStatus.Survived        => OutcomeCategory.Survived,
                   ExitStatus.Runner          => OutcomeCategory.RunThrough,
                   ExitStatus.Killed          => OutcomeCategory.Killed,
                   ExitStatus.MissingInAction => OutcomeCategory.MissingInAction,
                   ExitStatus.Left            => OutcomeCategory.Left,
                   ExitStatus.Transit         => OutcomeCategory.Transit,
                   _                          => OutcomeCategory.Unknown
               };
    }

    public static bool IsFullSurvival(OutcomeCategory category){
        return category == OutcomeCategory.Survived;
    }

    public static bool IsExtract(OutcomeCategory category){
        return category is OutcomeCategory.Survived or OutcomeCategory.RunThrough or OutcomeCategory.Transit;
    }

    public static bool IsLoss(OutcomeCategory category){
        return category is OutcomeCategory.Killed
                        or OutcomeCategory.MissingInAction
                        or OutcomeCategory.Left
                        or OutcomeCategory.Unknown;
    }

    internal enum OutcomeCategory{
        Unknown,
        Survived,
        RunThrough,
        Killed,
        MissingInAction,
        Left,
        Transit
    }
}
