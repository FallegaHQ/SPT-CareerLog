using EFT;
using EFT.HealthSystem;
using System;

namespace Softwyx.CareerLog.Collectors.Health;

/// <summary>Sum of per-part max HP at raid start (mods may raise caps).</summary>
internal static class RaidHealthBaseline{
    public static float MaxBodyHealth{
        get;
        private set;
    }

    public static void Capture(Player player){
        MaxBodyHealth = 0f;

        if(player?.HealthController is not IHealthController health) return;

        foreach(EBodyPart part in Enum.GetValues(typeof(EBodyPart))){
            if(part == EBodyPart.Common) continue;

            MaxBodyHealth += health.GetBodyPartHealth(part, false).
                                    Maximum;
        }
    }

    public static void Clear(){
        MaxBodyHealth = 0f;
    }
}
