using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class CentralServer : NetworkBehaviour
{
    [System.Serializable]
    public class RoomInfo
    {
        public string ip;
        public int port;

        public RoomInfo(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }
    }

    public List<RoomInfo> rooms = new List<RoomInfo>();

    // 클라이언트가 방을 생성할 때 호출하는 함수
    [Command]
    public void CmdCreateRoom(string ip, int port)
    {
        RoomInfo newRoom = new RoomInfo(ip, port);
        rooms.Add(newRoom);
        Debug.Log($"Room created: {ip}:{port}");
    }

    // 클라이언트가 방 목록을 요청할 때 호출되는 함수
    [Command]
    public void CmdRequestRoomList()
    {
        List<string> ipList = new List<string>();
        List<int> portList = new List<int>();

        foreach (var room in rooms)
        {
            ipList.Add(room.ip);
            portList.Add(room.port);
        }

        // 요청한 클라이언트에게 방 목록을 전달
        TargetReceiveRoomList(connectionToClient, ipList, portList);
    }

    // 방 목록을 클라이언트에게 전달하는 TargetRpc 함수
    [TargetRpc]
    public void TargetReceiveRoomList(NetworkConnection conn, List<string> ipList, List<int> portList)
    {
        Client client = conn.identity.GetComponent<Client>();
        if (client != null)
        {
            client.ReceiveRoomList(conn, ipList, portList);
        }
    }
}
