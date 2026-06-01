using EFT;

namespace Softwyx.CareerLog.Collectors.Loot;

internal static class LoadoutValueCollector{
    private static long _startRub;
    private static long _endRub;

    public static void CaptureStart(Profile profile){
        _startRub = SumEquipped(profile);
        _endRub   = _startRub;
    }

    public static void CaptureEnd(Profile profile){
        _endRub = SumEquipped(profile);
    }

    public static (long startRub, long endRub) TakeValues(){
        return (_startRub, _endRub);
    }

    public static void Clear(){
        _startRub = 0L;
        _endRub   = 0L;
    }

    private static long SumEquipped(Profile profile){
        var equipment = profile?.Inventory?.Equipment;

        if(equipment?.Slots == null) return 0L;

        var sum = 0L;

        foreach(var slot in equipment.Slots){
            var item = slot?.ContainedItem;

            if(item == null) continue;

            sum += LootHandbookPricing.EstimateInventoryRoot(item);
        }

        return sum;
    }
}
