using kcp2k;
using Mirror;
using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyNetworkManager : NetworkRoomManager
{
    [SerializeField] GameObject Killer;
    [SerializeField] GameObject killercamPref;

    [SerializeField] GameObject Survivor;
    [SerializeField] GameObject survivorcamPref;

    [SerializeField] NetworkDiscovery discovery;

    [SerializeField] private GameObject m_killerSideCam;

    public GameObject KillerSideCam
    {
        get
        {
            if (m_killerSideCam == null)
            {
                m_killerSideCam = Instantiate(killercamPref);
                NetworkServer.Spawn(m_killerSideCam);
            }
            return m_killerSideCam;
        }
    }

    [SerializeField] private GameObject m_survivorSideCam;
    public GameObject SurvivorSideCam
    {
        get
        {
            if (m_survivorSideCam == null)
            {
                m_survivorSideCam = Instantiate(survivorcamPref);
                NetworkServer.Spawn(m_survivorSideCam);
            }
            return m_survivorSideCam;
        }
    }

    public string CentralServerIPAddress = "3.38.185.62";
    [SerializeField] int CentralServerPort = 7777;
    Dictionary<string, NetworkConnectionToClient> connectedClients = new();
    string hostClientId;
    ushort hostPort;
    bool isMatching = false;


    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<RegisterClientMessage>(OnRegisterClient);
        NetworkServer.RegisterHandler<SetHostMessage>(OnSetHost);
        NetworkServer.RegisterHandler<GetHostMessage>(OnGetHost);
    }
    public override void OnStopServer()
    {
        NetworkServer.UnregisterHandler<GetHostMessage>();
        NetworkServer.UnregisterHandler<SetHostMessage>();
        NetworkServer.UnregisterHandler<RegisterClientMessage>();
        base.OnStopServer();
    }

    private void OnHostInfoReceived(HostInfoMessage msg)
    {
        networkAddress = msg.hostAddress;
        var port = Transport.active as KcpTransport;
        port.port = msg.hostPort;
        StartClient();
    }

    private void OnRegisterClient(NetworkConnectionToClient conn, RegisterClientMessage msg)
    {
        connectedClients[msg.clientId] = conn;
        
        Debug.Log($"Client {msg.clientId} registered with address {conn.address}");
    }

    private void OnSetHost(NetworkConnectionToClient conn, SetHostMessage msg)
    {
        if (connectedClients.ContainsKey(msg.clientId))
        {
            hostClientId = msg.clientId;
            Debug.Log($"Client {msg.clientId} is now the host");
        }
    }

    private void OnGetHost(NetworkConnectionToClient conn, GetHostMessage msg)
    {
        if (!string.IsNullOrEmpty(hostClientId) && connectedClients.ContainsKey(hostClientId))
        {
            var hostConn = connectedClients[hostClientId];
            var response = new HostInfoMessage
            {
                hostAddress = hostConn.address,
                //hostPort = hostConn.
            };
            conn.Send(response);
        }
        else
        {
            Debug.LogError("Host not found or not set");
        }
    }

    
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        NetworkClient.RegisterHandler<HostInfoMessage>(OnHostInfoReceived);
        StartCoroutine(RegisterClient());
    }
    
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        NetworkClient.UnregisterHandler<HostInfoMessage>();
    }

    IEnumerator RegisterClient()
    {
        var msg = new RegisterClientMessage
        {
            clientId = SystemInfo.deviceUniqueIdentifier
        };
        NetworkClient.Send(msg);
        yield return null;
    }

    public void SetHost()
    {
        var msg = new SetHostMessage
        {
            clientId = SystemInfo.deviceUniqueIdentifier,
            clientPort = connectedClients.Count
        };
        NetworkClient.Send(msg);
    }

    void GetHost()
    {
        var msg = new GetHostMessage();
        NetworkClient.Send(msg);
    }

    IEnumerator CorMathcing()
    {
        while (true)
        {
            GetHost();
            yield return new WaitForSeconds(3);
        }
    }






    public void OnClick_Survivor()
    {
        isMatching = !isMatching;
        if (isMatching)
            StartCoroutine(CorMathcing());
        else
        {
            StopCoroutine(CorMathcing());
        }
    }
    public void OnClick_Killer()
    {
        SetHost();
        StopClient();
        networkAddress = hostClientId;
        KcpTransport port = Transport.active as KcpTransport;
        port.port = 1;
        StartHost();
        
        
    }






    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        GameObject roomObj;



        if (conn.connectionId == 0)
        {
            Transform startPos = CustomLobbyStartPosition.Instance.GetStartPosition(true, out int index);
            roomObj = Instantiate(Killer, startPos.position, startPos.rotation);
            roomObj.GetComponent<CustomRoomPlayer>().StartPositionIndex = index;
        }
        else
        {
            Transform startPos = CustomLobbyStartPosition.Instance.GetStartPosition(false, out int index);
            roomObj = Instantiate(Survivor, startPos.position, startPos.rotation);
            roomObj.GetComponent<CustomRoomPlayer>().StartPositionIndex = index;
        }



        return roomObj;
    }


    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerDisconnect(conn);

        //CustomLobbyStartPosition.OnDiconnected(conn.identity.transform);
        //CustomRoomPlayer room = conn.identity.GetComponent<CustomRoomPlayer>();

        if (conn.identity.TryGetComponent(out CustomRoomPlayer room))
        {
            room.SetOutLook(true);
        }
        CustomLobbyStartPosition.Instance.OnDiconnected(room.StartPositionIndex);
    }

 
    public override void OnApplicationQuit()
    {
        PerkSettingModel.Instance.selectedPerkList.Clear();
        base.OnApplicationQuit();
    }

    public void CheckAllReady()
    {
        bool allReady = true;
        foreach (var conn in NetworkServer.connections)
        {
            var player = conn.Value.identity.GetComponent<CustomRoomPlayer>();
            if (player != null && !player.isReady)
            {
                allReady = false;
                break;
            }
        }

        if (allReady)
        {
            ServerChangeScene("GameScene");
        }
    }
}

public struct RegisterClientMessage : NetworkMessage
{
    public string clientId;
}

public struct SetHostMessage : NetworkMessage
{
    public string clientId;
    public int clientPort;
}

public struct GetHostMessage : NetworkMessage { }

public struct HostInfoMessage : NetworkMessage
{
    public string hostAddress;
    public ushort hostPort;
}