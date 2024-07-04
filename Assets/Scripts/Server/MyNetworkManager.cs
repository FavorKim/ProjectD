using Mirror;
using Mirror.Discovery;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyNetworkManager : NetworkRoomManager
{
    [SerializeField] GameObject Killer;
    [SerializeField]GameObject killercamPref;

    [SerializeField] GameObject Survivor;
    [SerializeField] GameObject survivorcamPref;

    [SerializeField] NetworkDiscovery discovery;

    [SerializeField] private GameObject m_killerSideCam;

    public GameObject KillerSideCam
    {
        get
        {
            if(m_killerSideCam == null)
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

    public void OnClick_Survivor()
    {
        //StartClient();
        discovery.StartDiscovery();
    }
    public void OnClick_Killer()
    {
        StartHost();
    }

    public override void Start()
    {
        base.Start();
        discovery = GetComponent<NetworkDiscovery>();
        discovery.OnServerFound.AddListener(OnServerFound);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        discovery.AdvertiseServer();
    }

    


    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        GameObject roomObj;

        

        if(conn.connectionId == 0)
        {
            Transform startPos = CustomLobbyStartPosition.Instance.GetStartPosition(true,out int index);
            roomObj = Instantiate(Killer,startPos.position,startPos.rotation);
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

        if(conn.identity.TryGetComponent(out CustomRoomPlayer room))
        {
            room.SetOutLook(true);
        }
        CustomLobbyStartPosition.Instance.OnDiconnected(room.StartPositionIndex);
    }

    void OnServerFound(ServerResponse response)
    {
        StartClient();
    }

    public override void OnApplicationQuit()
    {
        PerkSettingModel.Instance.selectedPerkList.Clear();
        discovery.OnServerFound.RemoveListener(OnServerFound);
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