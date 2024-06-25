using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HavingPerkList : NetworkBehaviour
{
    [SerializeField] Transform GridTransform;
    [SerializeField] GameObject Prefab_Perk;

    private void Start()
    {
        SetPerkList();
    }

    
    void SetPerkList()
    {
        if (NetworkClient.connection.connectionId == 0)
        {
            SetKillerPerkList();
        }
        else
        {
            SetSurvivorPerkList();
        }
    }

    void SetSurvivorPerkList()
    {
        foreach (var item in PerkDataManager.Instance.LoadedPerkList.Values)
        {
            if (item.Owner != "Survivor") continue;
            var perk = PerkDataManager.Instance.GetPerkByName(item.PerkName);
            var perkObj = Instantiate(Prefab_Perk, GridTransform).GetComponentInChildren<Perk>();
            perkObj.Init(perk);
        }
    }

    void SetKillerPerkList()
    {
        foreach (var item in PerkDataManager.Instance.LoadedPerkList.Values)
        {
            if (item.Owner != "Killer") continue;
            var perk = PerkDataManager.Instance.GetPerkByName(item.PerkName);
            var perkObj = Instantiate(Prefab_Perk, GridTransform).GetComponentInChildren<Perk>();
            perkObj.Init(perk);
            //Instantiate(perk, GridTransform);
        }
    }
}
