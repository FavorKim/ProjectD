using UnityEngine;
using Mirror;
using System.Collections.Generic;
using kcp2k;

public class Client : NetworkBehaviour
{
    public NetworkManager networkManager;
    public CentralServer centralServer;

    // KcpTransport 참조
    private KcpTransport kcpTransport;

    // 방 목록을 저장할 리스트
    private List<string> roomIPs = new List<string>();
    private List<int> roomPorts = new List<int>();

    private void Start()
    {
        kcpTransport = networkManager.GetComponent<KcpTransport>();
    }
    public void ConnectToServer(string ip, int port)
    {
        networkManager.networkAddress = ip; // 중앙 서버 IP 설정
        networkManager.GetComponent<KcpTransport>().Port = (ushort)port; // 포트 설정
        networkManager.StartClient(); // 클라이언트 시작
        Debug.Log($"Connecting to server at {ip}:{port}");
    }
    // 중앙 서버에 방 목록을 요청하는 함수
    public void RequestRoomList()
    {
        // CmdRequestRoomList는 매개변수 없이 호출 가능
        centralServer.CmdRequestRoomList();
    }

    // 중앙 서버에서 방 목록을 받는 TargetRpc 함수
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

        // 방 목록 중 첫 번째 방에 입장하는 예제 (원하는 방 번호를 선택할 수 있음)
        if (roomIPs.Count > 0)
        {
            // 0번째 방에 입장
            ConnectToRoom(roomIPs[0], roomPorts[0]);
        }
    }

    // 선택한 IP와 포트를 사용해 호스트에게 클라이언트로 연결하는 함수
    public void ConnectToRoom(string ip, int port)
    {
        networkManager.networkAddress = ip;
        kcpTransport.Port = (ushort)port;  // KcpTransport의 포트를 설정

        networkManager.StartClient();
        Debug.Log($"Connecting to host at {ip}:{port}");
    }
}
