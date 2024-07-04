using Michsky.UI.Dark;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomRoomPlayer : NetworkRoomPlayer
{
    GameObject OutLook;

    public int StartPositionIndex
    {
        get;set;
    }

    private void Awake()
    {
        OutLook = gameObject.transform.GetChild(0).gameObject;
    }

    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        OutLook.SetActive(true);
        if (!isLocalPlayer) return;
        var manager = NetworkRoomManager.singleton as MyNetworkManager;
        if (isServer)
        {
            manager.KillerSideCam.SetActive(true);
            manager.SurvivorSideCam.SetActive(false);

        }
        else
        {
            manager.SurvivorSideCam.SetActive(true);
            manager.KillerSideCam.SetActive(false);
        }
    }

    public override void OnClientExitRoom()
    {
        
        base.OnClientExitRoom();
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            //OutLook = gameObject.transform.GetChild(0).gameObject;
            OutLook.SetActive(false);
        }
    }

    public void SetOutLook(bool onOff)
    {
        if(OutLook== null)
            OutLook = gameObject.transform.GetChild(0).gameObject;
        OutLook.SetActive(onOff);
    }
    
}
