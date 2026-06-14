using EFT.UI.SessionEnd;
using Softwyx.CareerLog.Infrastructure;
using Softwyx.CareerLog.Ui.SessionEnd.Debrief;
using Softwyx.CareerLog.Ui.SessionEnd.Map;

namespace Softwyx.CareerLog.Ui.SessionEnd;

/// <summary>Session-end screen registration -- mirror of menu <see cref="Ui.Records.ScreenBootstrap" />.</summary>
internal static class SessionEndUiBootstrap{
    public static SessionResultRaidMap RaidMapScreen{
        get;
        private set;
    }

    public static SessionResultCareerLog DebriefScreen{
        get;
        private set;
    }

    public static void Register(SessionEndUI sessionEndUi){
        if(!sessionEndUi?.SessionResultStatistics){
            CareerLogPlugin.Log?.LogWarning(PluginInfo.Format("Session end UI template unavailable."));

            return;
        }

        if(RaidMapScreen && DebriefScreen) return;

        var template = sessionEndUi.SessionResultStatistics;

        if(!RaidMapScreen){
            RaidMapScreen = SessionResultRaidMap.CreateFromTemplate(template);

            if(RaidMapScreen)
                CurrentScreenSingletonClass.Instance.RegisterScreen(ScreenTypes.RaidMap, RaidMapScreen);
            else
                CareerLogPlugin.Log?.LogError(PluginInfo.Format("Failed to create raid map screen."));
        }

        if(!DebriefScreen){
            DebriefScreen = SessionResultCareerLog.CreateFromTemplate(template);

            if(DebriefScreen)
                CurrentScreenSingletonClass.Instance.RegisterScreen(ScreenTypes.Debrief, DebriefScreen);
            else
                CareerLogPlugin.Log?.LogError(PluginInfo.Format("Failed to create raid debrief screen."));
        }

        if(RaidMapScreen && DebriefScreen)
            CareerLogPlugin.Log?.LogInfo(PluginInfo.Format("Registered raid map + debrief session-end screens."));
    }
}
