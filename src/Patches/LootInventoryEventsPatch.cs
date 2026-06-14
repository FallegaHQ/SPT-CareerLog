using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using Softwyx.CareerLog.Collectors.Loot;
using Softwyx.CareerLog.Config;
using Softwyx.CareerLog.Session;
using SPT.Reflection.Patching;

namespace Softwyx.CareerLog.Patches;

/// <summary>
///     Hooks local player's inventory add/remove events during raid so we can cancel loot markers when the item is
///     dropped.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class LootInventoryEventsPatch : ModulePatch{
    protected override MethodBase GetTargetMethod(){
        return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
    }

    [PatchPostfix]
    private static void Postfix(GameWorld __instance){
        if(!Settings.Enabled.Value || !CareerLogSession.CollectorsActive) return;

        var player = __instance?.MainPlayer;

        if(!player || !player.IsYourPlayer) return;

        IItemOwner owner = player.InventoryController;

        if(owner == null) return;

        // Ensure we don't double-subscribe across edge cases.
        LootInventoryEventsWire.Detach(owner);
        LootInventoryEventsWire.Attach(owner);
    }

    private static class LootInventoryEventsWire{
        private static IItemOwner _owner;

        public static void Attach(IItemOwner owner){
            if(owner == null) return;

            _owner                 =  owner;
            _owner.AddItemEvent    += OnItemAdded;
            _owner.RemoveItemEvent += OnItemRemoved;
        }

        public static void Detach(IItemOwner owner){
            if(owner == null) return;

            owner.AddItemEvent    -= OnItemAdded;
            owner.RemoveItemEvent -= OnItemRemoved;

            if(_owner == owner) _owner = null;
        }

        private static void OnItemAdded(GEventArgs2 args){
            if(!CareerLogSession.CollectorsActive) return;

            var item = args?.Item;

            if(item == null) return;

            LootMarkerCollector.RecordAdded(item);
        }

        private static void OnItemRemoved(GEventArgs3 args){
            if(!CareerLogSession.CollectorsActive) return;

            var item = args?.Item;

            if(item == null) return;

            LootMarkerCollector.RecordRemoved(item);
        }
    }
}
