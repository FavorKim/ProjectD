using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerkManager
{
    public void SetPerkStat(List<PerksScriptableObject> perkList, PlayableCharactor player)
    {

    }

    public static void SetSurvivorPerk(List<PerksScriptableObject> perkList, Survivor player)
    {
        foreach (var perk in perkList)
        {

        }
    }

    public static void SetKillerPerk(List<PerksScriptableObject> perkList, KillerBase player)
    {
        foreach (var perk in perkList)
        {
            if (perk == null) continue;

            switch (perk.Target)
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
