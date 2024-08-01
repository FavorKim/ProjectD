using kcp2k;
using Mirror;
using System.Collections.Generic;

public class MatchRoom : NetworkBehaviour
{
    KcpTransport roomPort;
    public KcpTransport GetRoomPort() {  return roomPort; }
    List<SelectedPerkManager> roomMembers = new List<SelectedPerkManager>();
    public bool CanJoin { get; private set; }

    public void InitRoom(KcpTransport port)
    {
        roomPort = port;
        CanJoin = true;
    }

    public void AddPlayer(SelectedPerkManager player)
    {
        roomMembers.Add(player);
    }

    public void OnJoinedRoom()
    {

    }
}
