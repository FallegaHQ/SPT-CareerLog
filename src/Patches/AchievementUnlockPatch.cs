using System.Diagnostics.CodeAnalysis;
using EFT.Quests;
using Softwyx.CareerLog.Collectors.Meta;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;
using System.Reflection;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class AchievementUnlockPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(AchievementTaskClass).GetMethod(
                                                      nameof(AchievementTaskClass.SetStatus),
                                                      BindingFlags.Instance | BindingFlags.Public
                                                     );
    }

    [PatchPostfix]
    private static void Postfix(AchievementTaskClass __instance, EQuestStatus status, bool notify, bool fromServer){
        if(!Settings.Enabled.Value || !CareerLogSession.CollectorsActive) return;

        if(status != EQuestStatus.Success || !notify) return;

        var template = __instance.Template;

        if(template == null) return;

        AchievementMarkerCollector.Record(template.Id, template.Title);
    }
}
