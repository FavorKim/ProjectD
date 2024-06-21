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

    
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        NetworkConnectionToClient conn = NetworkServer.localConnection;
        var pref = conn.connectionId == 0 ? Killer : Survivor;
        Debug.Log(conn);
        SpawnPlayer(pref, conn);
    }

    
    /*
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        if (newSceneName == "GameScene")
        {
            NetworkConnectionToClient conn = NetworkServer.localConnection;
            var pref = conn.connectionId == 0 ? Killer : Survivor;
            Debug.Log(conn);
            SpawnPlayer(pref, conn);
        }
    }
    */
    [Server]
    void SpawnPlayer(GameObject pref, NetworkConnectionToClient conn)
    {
        var obj = Instantiate(pref, GetStartPosition().position, Quaternion.identity);
        NetworkServer.ReplacePlayerForConnection(conn, obj);
    }
}