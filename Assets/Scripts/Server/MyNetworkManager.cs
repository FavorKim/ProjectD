using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    /*
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Transform startPos = CustomLobbyStartPosition.GetStartPosition();

        if (CustomLobbyStartPosition.StartPositions.Count == 4)
        {
            Instantiate(Killer, startPos.position, startPos.rotation);
        }
        else
        {
            Instantiate(Survivor, startPos.position, startPos.rotation);
        }
    }
    

    public override void OnRoomClientEnter()
    {
        base.OnRoomClientEnter();
        
        
        Transform startPos = CustomLobbyStartPosition.GetStartPosition();

        if (CustomLobbyStartPosition.StartPositions.Count == 4)
        {
            Instantiate(Killer, startPos.position, startPos.rotation);
        }
        else
        {
            Instantiate(Survivor, startPos.position, startPos.rotation);
        }
    }
    
    public override void OnRoomClientConnect()
    {
        base.OnRoomClientConnect();
    }
    */
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