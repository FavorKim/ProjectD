using kcp2k;
using Mirror;
using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientsNetworkManager : NetworkRoomManager
{
    public void CreateRoom()
    {
        var port = GetComponent<KcpTransport>();
        port.Port = 1;
        StartHost();
    }

    public void JoinRoom()
    {

    }
}
