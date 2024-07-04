using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomRoomPlayer : NetworkRoomPlayer
{
    GameObject OutLook;

    public int StartPositionIndex
    {
        get; set;
    }

    public bool isReady = false;

    Button Btn_Ready;

    public override void Start()
    {
        base.Start();
        if (Btn_Ready == null)
        {
            Btn_Ready = GameObject.Find("LobbySetting/Btn_Ready").GetComponent<Button>();
            Btn_Ready.onClick.AddListener(OnReadyButtonClick);
        }
    }

    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        SetOutLook(true);
        if (!isLocalPlayer) return;

        SetLobbyCamera();
    }

    public override void OnClientExitRoom()
    {

        base.OnClientExitRoom();
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            SetOutLook(false);
            showRoomGUI = false;
        }
        else if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            SetOutLook(true);
            showRoomGUI = true;
        }
    }

    private void OnApplicationQuit()
    {
        Btn_Ready.onClick.RemoveListener(OnReadyButtonClick);
    }

    public void SetOutLook(bool onOff)
    {
        if (OutLook == null)
            OutLook = gameObject.transform.GetChild(0).gameObject;
        OutLook.SetActive(onOff);
    }

    void SetLobbyCamera()
    {
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

    void OnReadyButtonClick()
    {
        CmdSetReady();
    }

    [Command]
    void CmdSetReady()
    {
        RpcSetReady();
        
    }
    [ClientRpc]
    void RpcSetReady()
    {
        isReady = !isReady;
        ReadyStateHUD.Instance.RpcSetImageColor(StartPositionIndex, isReady);
        var lobbyManager = (MyNetworkManager)NetworkManager.singleton;
        lobbyManager.CheckAllReady();
    }

}
