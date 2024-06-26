using DungeonArchitect;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : SingletonNetwork<LobbyPlayer>
{
    List<PerkData> perks;


    public void InitPerkList(List<PerkData> perks)
    {
        this.perks = new();
        foreach (var perk in perks)
        {
            this.perks.Add(perk);
        }
    }

    public List<PerkData> GetPerkList() 
    {
        var list = new List<PerkData>();
        list = perks;
        return list; 
    }


    private void OnApplicationQuit()
    {
        perks = null;
    }
}
