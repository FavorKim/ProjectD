using kcp2k;
using Mirror;
using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientsNetworkManager : NetworkRoomManager
{
    //ServerMatchingManager serverManager;

    KcpTransport port;
    public override void Start()
    {
        base.Start();
        port = GetComponent<KcpTransport>();
        //serverManager = new();
    }


    public void CreateRoom()
    {
        ServerMatchingManager.Instance.OnCreateRoom();

        port.Port = ServerMatchingManager.Instance.GetRoomCount();
        Debug.Log(port.Port.ToString());
        NetworkClient.Disconnect();
        StartHost();
    }

    public void JoinRoom()
    {
        var randomPort = ServerMatchingManager.Instance.GetRoomAvailable();
        if (randomPort == default) 
        {
            Debug.Log("¹æ ¾øÀ½");
            return; 
        }
        port.Port = randomPort;
        NetworkClient.Disconnect();
        StartClient();
    }

    //public override void OnStopClient()
    //{
    //    base.OnStopClient();
    //    port.Port = ServerMatchingManager.serverPort;
    //    StartClient();
    //}

    //public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    //{
    //    base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);


    //    if (port.Port == 7777)
    //        SceneManager.LoadScene(newSceneName);
    //}
}
