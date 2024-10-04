using UnityEngine;
using Mirror;
using System.Net;
using kcp2k;

public class HostClient : NetworkBehaviour
{
    public NetworkManager networkManager;
    public CentralServer centralServer;

    // KcpTransport 또는 TelepathyTransport 참조
    private KcpTransport kcpTransport;

    private void Start()
    {
        kcpTransport = networkManager.GetComponent<KcpTransport>();
    }

    public void CreateRoom()
    {
        // 호스트로 전환
        networkManager.StartHost();
        // IP 및 포트 설정
        string hostIP = GetLocalIPAddress();
        if(gameObject.activeSelf==false)
        {
            gameObject.SetActive(true);

        }
        if(kcpTransport == null)
        {
            kcpTransport = networkManager.GetComponent<KcpTransport>();
        }
        int hostPort = kcpTransport.Port;  // Transport의 포트 참조

        // 중앙 서버에 방 생성 정보 전달
        centralServer.CmdCreateRoom(hostIP, hostPort);

        Debug.Log($"Room created by host at {hostIP}:{hostPort}");
    }

    // IP 주소를 가져오는 함수 (로컬 테스트용)
    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1";
    }
}
