using kcp2k;
using Mirror;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkRoomManager
{
    private static LobbyManager instance;
    public static LobbyManager Instance {  get { return instance; } }


    public KcpTransport kcp;
    public override void Awake()
    {
        base.Awake();
        instance = this;
        kcp=GetComponent<KcpTransport>();
    }

    public void Login()
    {
        //SceneManager.LoadScene("MainScene");
        StartClient();
    }

    //[Command(requiresAuthority =false)]
    public void CreateRoom()
    {
        MatchingRoomManager.Instance.CreateRoom();
        var roomPort = MatchingRoomManager.Instance.GetRoomCount();
        kcp.port = roomPort;
        StopClient();
        StartHost();
    }

    public void JoinRoom(ushort roomPort)
    {
        StopClient();
        kcp.port = roomPort;
        StartClient();
    }
}
