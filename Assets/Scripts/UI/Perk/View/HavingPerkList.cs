using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HavingPerkList : NetworkBehaviour
{
    [SerializeField] Transform GridTransform;
    [SerializeField] GameObject Prefab_Perk;
    [SerializeField] GameObject parent;

    private void Start()
    {
        SetPerkList();
        parent.SetActive(false);
    }


    void SetPerkList()
    {
        if (isClientOnly)
        {
            SetSurvivorPerkList();
        }
        else
        {
            SetKillerPerkList();
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
