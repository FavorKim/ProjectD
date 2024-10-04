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

    // Ŭ���̾�Ʈ�� ���� ������ �� ȣ���ϴ� �Լ�
    [Command]
    public void CmdCreateRoom(string ip, int port)
    {
        RoomInfo newRoom = new RoomInfo(ip, port);
        rooms.Add(newRoom);
        Debug.Log($"Room created: {ip}:{port}");
    }

    // Ŭ���̾�Ʈ�� �� ����� ��û�� �� ȣ��Ǵ� �Լ�
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

        // ��û�� Ŭ���̾�Ʈ���� �� ����� ����
        TargetReceiveRoomList(connectionToClient, ipList, portList);
    }

    // �� ����� Ŭ���̾�Ʈ���� �����ϴ� TargetRpc �Լ�
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
