using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    /*
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        if(newSceneName == "GameScene")
        {
            List<PerksScriptableObject> perks = new List<PerksScriptableObject>();
            foreach(SelectedPerk p in PerkSettingModel.Instance.selectedPerkList)
            {
                perks.Add(p.perk);


            }
            LobbyPlayer.Instance.InitPerkList(perks);
        }
    }
    */
    public override void OnRoomClientSceneChanged()
    {
        base.OnRoomClientSceneChanged();
        List<PerksScriptableObject> perks = new List<PerksScriptableObject>();
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