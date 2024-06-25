using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : SingletonMono<LobbyPlayer>
{
    List<PerksScriptableObject> perks = new();

    public void InitPerkList(List<PerksScriptableObject> perks)
    {
        foreach (var perk in perks)
        {
            this.perks.Add(perk);
        }
    }

    public List<PerksScriptableObject> GetPerkList() { return perks; }
}
