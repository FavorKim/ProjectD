using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkRoomManager
{
    [SerializeField] GameObject Survivor;
    [SerializeField] GameObject Killer;

    GameObject playerPref;

    public void OnClick_Survivor()
    {
        StartClient();
    }
    public void OnClick_Killer()
    {
        StartHost();
    }

}