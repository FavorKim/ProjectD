using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySetting : NetworkBehaviour
{
    [SerializeField] GameObject KillerCam;
    [SerializeField] GameObject SurvivorCam;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (isClientOnly)
        {
            SurvivorCam.SetActive(true);
        }
        else
        {
            KillerCam.SetActive(true);
        }
    }
}
