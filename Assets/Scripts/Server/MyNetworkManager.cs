using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkRoomManager
{
    [SerializeField] GameObject Survivor;
    [SerializeField] GameObject Killer;


    public void OnClick_Survivor()
    {
        StartClient();
    }
    public void OnClick_Killer()
    {
        StartHost();
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