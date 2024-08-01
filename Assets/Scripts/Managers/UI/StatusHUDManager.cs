using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHUDManager : SingletonMono<StatusHUDManager>
{
    [SerializeField] GameObject Prefab_StatusHUD;
    [SerializeField] Transform Transform_StatusHUDGroup;

    public void CreateStatusHUD(StatusPerk perk)
    {
        var statusHud = Instantiate(Prefab_StatusHUD,Transform_StatusHUDGroup).GetComponent<StatusHUD>();
        statusHud.InitPerk(perk);
    }
}
