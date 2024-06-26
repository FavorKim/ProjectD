using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : SingletonMono<LobbyPlayer>
{
    List<PerkData> perks = new();

    public void InitPerkList(List<PerkData> perks)
    {
        foreach (var perk in perks)
        {
            this.perks.Add(perk);
        }
    }

    public List<PerkData> GetPerkList() { return perks; }
}
