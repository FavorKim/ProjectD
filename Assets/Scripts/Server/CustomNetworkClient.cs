using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkClient : MonoBehaviour
{
    
    void Start()
    {
        //ConnectToServer();
    }

    void ConnectToServer()
    {
        var manager = MyNetworkManager.singleton as MyNetworkManager;

        manager.networkAddress = manager.CentralServerIPAddress;
        manager.StartClient();
    }
}
