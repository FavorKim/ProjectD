using Mirror;

public class LoginPlayer : NetworkBehaviour
{
    public void CreateRoom()
    {
        MatchingRoomManager.Instance.CreateRoom();
    }

    public void JoinRoom()
    {
        NetworkManager.singleton.StopClient();
        
    }
}
