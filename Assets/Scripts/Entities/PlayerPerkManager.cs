using System.Collections;
using System.Collections.Generic;

public class PlayerPerkManager
{
    public static void SetSurvivorPerk(List<PerkData> perkList, Survivor player)
    {

        foreach (var perk in perkList)
        {
            if (perk == null) continue;

            if (perk.PerkType == PerkType.Passive)
            {
                switch (perk.EffectTarget)
                {
                    case HandleValue.HealSpeed:
                        player.HealSpeed += perk.ValuePercentage;
                        break;
                    case HandleValue.CrouchSpeed:
                        player.CrouchSpeed += perk.ValuePercentage;
                        break;
                }
            }

            else if (perk.PerkType == PerkType.Exhaust)
            {
                switch (perk.PerkName)
                {
                    case "Sprint":
                        var dest = new Sprint(perk.ValuePercentage, perk.CoolTime, perk.Duration, player);
                        break;
                }
            }
        }
    }

    public static void SetKillerPerk(List<PerkData> perkList, KillerBase player)
    {
        foreach (var perk in perkList)
        {
            if (perk == null) continue;
            if (perk.PerkType == PerkType.Passive)
            {
                switch (perk.EffectTarget)
                {
                    case HandleValue.AttackSpeed:
                        player.AttackSpeed += perk.ValuePercentage;
                        break;
                    case HandleValue.StunRecover:
                        player.StunRecover += perk.ValuePercentage;
                        break;
                    case HandleValue.JumpSpeed:
                        player.JumpSpeed += perk.ValuePercentage;
                        break;
                    case HandleValue.BreakSpeed:
                        player.BreakSpeed += perk.ValuePercentage;
                        break;
                }
            }
        }
    }
}
