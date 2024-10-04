using UnityEngine;
using Mirror;
using System.Net;
using kcp2k;

public class HostClient : NetworkBehaviour
{
    public NetworkManager networkManager;
    public CentralServer centralServer;

    // KcpTransport �Ǵ� TelepathyTransport ����
    private KcpTransport kcpTransport;

    private void Start()
    {
        kcpTransport = networkManager.GetComponent<KcpTransport>();
    }

    public void CreateRoom()
    {
        // ȣ��Ʈ�� ��ȯ
        networkManager.StartHost();
        // IP �� ��Ʈ ����
        string hostIP = GetLocalIPAddress();
        if(gameObject.activeSelf==false)
        {
            gameObject.SetActive(true);

        }
        if(kcpTransport == null)
        {
            kcpTransport = networkManager.GetComponent<KcpTransport>();
        }
        int hostPort = kcpTransport.Port;  // Transport�� ��Ʈ ����

        // �߾� ������ �� ���� ���� ����
        centralServer.CmdCreateRoom(hostIP, hostPort);

        Debug.Log($"Room created by host at {hostIP}:{hostPort}");
    }

    // IP �ּҸ� �������� �Լ� (���� �׽�Ʈ��)
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
