using Mirror;
using UnityEngine;

public class CustomRoomPlayer : NetworkRoomPlayer
{


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        var manager = NetworkRoomManager.singleton as MyNetworkManager;
        if (isServer)
        {
            manager.KillerSideCam.SetActive(true);
            manager.SurvivorSideCam.SetActive(false);

        }
        else
        {
            manager.SurvivorSideCam.SetActive(true);
            manager.KillerSideCam.SetActive(false);
        }
    }
}
