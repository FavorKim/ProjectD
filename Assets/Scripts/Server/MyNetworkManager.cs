using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkRoomManager
{

    public void OnClick_Survivor()
    {
        StartClient();
    }
    public void OnClick_Killer()
    {
        StartHost();
    }

}