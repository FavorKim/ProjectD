using UnityEngine;
using Mirror;
using System.Collections.Generic;
using kcp2k;

public class Client : NetworkBehaviour
{
    public NetworkManager networkManager;
    public CentralServer centralServer;

    // KcpTransport ����
    private KcpTransport kcpTransport;

    // �� ����� ������ ����Ʈ
    private List<string> roomIPs = new List<string>();
    private List<int> roomPorts = new List<int>();

    private void Start()
    {
        kcpTransport = networkManager.GetComponent<KcpTransport>();
    }
    public void ConnectToServer(string ip, int port)
    {
        networkManager.networkAddress = ip; // �߾� ���� IP ����
        networkManager.GetComponent<KcpTransport>().Port = (ushort)port; // ��Ʈ ����
        networkManager.StartClient(); // Ŭ���̾�Ʈ ����
        Debug.Log($"Connecting to server at {ip}:{port}");
    }
    // �߾� ������ �� ����� ��û�ϴ� �Լ�
    public void RequestRoomList()
    {
        // CmdRequestRoomList�� �Ű����� ���� ȣ�� ����
        centralServer.CmdRequestRoomList();
    }

    // �߾� �������� �� ����� �޴� TargetRpc �Լ�
    [TargetRpc]
    public void ReceiveRoomList(NetworkConnection conn, List<string> ipList, List<int> portList)
    {
        roomIPs = ipList;
        roomPorts = portList;

        Debug.Log("Received room list:");
        for (int i = 0; i < roomIPs.Count; i++)
        {
            Debug.Log($"Room {i}: {roomIPs[i]}:{roomPorts[i]}");
        }

        // �� ��� �� ù ��° �濡 �����ϴ� ���� (���ϴ� �� ��ȣ�� ������ �� ����)
        if (roomIPs.Count > 0)
        {
            // 0��° �濡 ����
            ConnectToRoom(roomIPs[0], roomPorts[0]);
        }
    }

    // ������ IP�� ��Ʈ�� ����� ȣ��Ʈ���� Ŭ���̾�Ʈ�� �����ϴ� �Լ�
    public void ConnectToRoom(string ip, int port)
    {
        networkManager.networkAddress = ip;
        kcpTransport.Port = (ushort)port;  // KcpTransport�� ��Ʈ�� ����

        networkManager.StartClient();
        Debug.Log($"Connecting to host at {ip}:{port}");
    }
}
