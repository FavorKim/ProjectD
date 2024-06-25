using kcp2k;
using Mirror;
using System.Collections.Generic;

public class MatchRoom : NetworkBehaviour
{
    KcpTransport roomPort;
    public KcpTransport GetRoomPort() {  return roomPort; }
    List<LobbyPlayer> roomMembers = new List<LobbyPlayer>();
    public bool CanJoin { get; private set; }

    public void InitRoom(KcpTransport port)
    {
        roomPort = port;
        CanJoin = true;
    }

    public void AddPlayer(LobbyPlayer player)
    {
        roomMembers.Add(player);
    }

    public void OnJoinedRoom()
    {

    }
}
