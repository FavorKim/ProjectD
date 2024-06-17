using kcp2k;
using Mirror;
using System.Collections.Generic;

public class MatchRoom : NetworkBehaviour
{
    KcpTransport roomPort;
    public KcpTransport GetRoomPort() {  return roomPort; }
    List<LoginPlayer> roomMembers = new List<LoginPlayer>();
    public bool CanJoin { get; private set; }

    public void InitRoom(KcpTransport port)
    {
        roomPort = port;
        CanJoin = true;
    }

    public void AddPlayer(LoginPlayer player)
    {
        roomMembers.Add(player);
    }

    public void OnJoinedRoom()
    {

    }
}
