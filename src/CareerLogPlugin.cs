using BepInEx;
using BepInEx.Logging;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Localization;
using Softwyx.CareerLog.Patches;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.SPT.core", "4.0.0")]
public class CareerLogPlugin : BaseUnityPlugin{
    internal static ManualLogSource Log;

    internal static CareerLogPlugin Instance;

    private void Awake(){
        Instance = this;

        Log = Logger;

        if(!PreloadDefaultLocale()) return;

        Settings.Init(Config);

        Log.LogInfo(PluginInfo.Format($"{PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} loaded (BepInEx)."));

        EnablePatch<LocaleApplicationLanguagePatch>("LocaleApplicationLanguagePatch");
        EnablePatch<RaidStartedPatch>("RaidStartedPatch");
        EnablePatch<PlayerRaidEndPatch>("PlayerRaidEndPatch");
        EnablePatch<RaidEndPatch>("RaidEndPatch");
        EnablePatch<CommonUiAwakePatch>("CommonUiAwakePatch");
        EnablePatch<SessionEndUiAwakePatch>("SessionEndUiAwakePatch");
        EnablePatch<PostRaidBeforeKillListPatch>("PostRaidBeforeKillListPatch");
        EnablePatch<PostRaidBeforeStatisticsPatch>("PostRaidBeforeStatisticsPatch");
        EnablePatch<MenuScreenRecordsPatch>("MenuScreenRecordsPatch");
        EnablePatch<MenuScreenShowLegacyPatch>("MenuScreenShowLegacyPatch");
        EnablePatch<MenuScreenShowPatch>("MenuScreenShowPatch");
        EnablePatch<MenuScreenShowActionPatch>("MenuScreenShowActionPatch");
        EnablePatch<MenuScreenMinimizePatch>("MenuScreenMinimizePatch");
        EnablePatch<StatisticsEnemyKillPatch>("StatisticsEnemyKillPatch");
        EnablePatch<StatisticsGrabLootPatch>("StatisticsGrabLootPatch");
        EnablePatch<HealthBodyPartDestroyedPatch>("HealthBodyPartDestroyedPatch");
        EnablePatch<HealthRestoreBodyPartPatch>("HealthRestoreBodyPartPatch");
        EnablePatch<HealthChangePatch>("HealthChangePatch");
        EnablePatch<AchievementUnlockPatch>("AchievementUnlockPatch");
        EnablePatch<DoorUnlockPatch>("DoorUnlockPatch");
        EnablePatch<LootInventoryEventsPatch>("LootInventoryEventsPatch");
    }

    private static bool PreloadDefaultLocale(){
        if(LocaleLoader.PreloadDefaultLocale(out var error)) return true;

        Log.LogError(PluginInfo.Format($"Locale preload failed: {error} Mod patches were not enabled."));

        return false;
    }

    private static void EnablePatch<T>(string name) where T : ModulePatch, new(){
        try{
            new T().Enable();

            Log.LogInfo(PluginInfo.Format($"{name} enabled."));
        }

        catch(System.Exception ex){
            Log.LogError(PluginInfo.Format($"{name} failed: {ex}"));
        }
    }
}
