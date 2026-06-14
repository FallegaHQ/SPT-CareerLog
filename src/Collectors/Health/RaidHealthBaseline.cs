using System;
using EFT;

namespace Softwyx.CareerLog.Collectors.Health;

/// <summary>Sum of per-part max HP at raid start (mods may raise caps).</summary>
internal static class RaidHealthBaseline{
    public static float MaxBodyHealth{
        get;
        private set;
    }

    public static void Capture(Player player){
        MaxBodyHealth = 0f;

        if(player?.HealthController is not{} health) return;

        foreach(EBodyPart part in Enum.GetValues(typeof(EBodyPart))){
            if(part == EBodyPart.Common) continue;

            MaxBodyHealth += health.GetBodyPartHealth(part).
                                    Maximum;
        }
    }

    public static void Clear(){
        MaxBodyHealth = 0f;
    }
}
