using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkRoomManager
{
    [SerializeField] GameObject Killer;
    [SerializeField]GameObject killercamPref;

    [SerializeField] GameObject Survivor;
    [SerializeField] GameObject survivorcamPref;

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
        StartClient();
    }
    public void OnClick_Killer()
    {
        StartHost();
    }

    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        GameObject roomObj;

        Transform startPos = CustomLobbyStartPosition.GetStartPosition();

        if(conn.connectionId == 0)
        {
            roomObj = Instantiate(Killer,startPos.position,startPos.rotation);
        }
        else
        {
            roomObj = Instantiate(Survivor, startPos.position, startPos.rotation);
        }

        

        return roomObj;
    }


    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerDisconnect(conn);

        CustomLobbyStartPosition.OnDiconnected(conn.identity.transform);
        Destroy(conn.identity);
    }

    public override void OnRoomClientSceneChanged()
    {
        base.OnRoomClientSceneChanged();
        List<PerkData> perks = new List<PerkData>();
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            foreach (SelectedPerk p in PerkSettingModel.Instance.selectedPerkList)
            {
                perks.Add(p.perk);
            }
            LobbyPlayer.Instance.InitPerkList(perks);
        }
    }
    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        PerkSettingModel.Instance.selectedPerkList.Clear();
    }
}