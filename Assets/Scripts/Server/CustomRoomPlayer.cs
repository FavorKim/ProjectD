using Michsky.UI.Dark;
using Mirror;
using UnityEngine;

public class CustomRoomPlayer : NetworkRoomPlayer
{
    GameObject OutLook;


    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        if (!isLocalPlayer) return;
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

    public override void OnClientExitRoom()
    {
        base.OnClientExitRoom();
        OutLook = gameObject.transform.GetChild(0).gameObject;
        OutLook.SetActive(false);
    }
}
