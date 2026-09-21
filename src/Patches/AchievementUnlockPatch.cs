using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT.Achievements;
using EFT.Quests;
using Softwyx.CareerLog.Collectors.Meta;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class AchievementUnlockPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(Achievement).GetMethod(
                                                      nameof(Achievement.SetStatus),
                                                      BindingFlags.Instance | BindingFlags.Public
                                                     );
    }

    [PatchPostfix]
    private static void Postfix(Achievement __instance, EQuestStatus status, bool notify, bool fromServer){
        if(!Settings.Enabled.Value || !CareerLogSession.CollectorsActive) return;

        if(status != EQuestStatus.Success || !notify) return;

        var template = __instance.Template;

        if(template == null) return;

        AchievementMarkerCollector.Record(template.Id, template.Title);
    }
}
