using Mirror;
using Mirror.Discovery;
using System;
using System.Net;

public class CustomDiscoveryBase : NetworkDiscoveryBase<ServerRequest, ServerResponse>
{
    long serverId;
    MyNetworkManager manager;

    public override void Start()
    {
        serverId = RandomLong();
        manager = GetComponent<MyNetworkManager>();
        base.Start();
    }
    protected override void ProcessResponse(ServerResponse response, IPEndPoint endpoint)
    {
        if (manager != null && !NetworkClient.isConnected)
        {
            manager.StartClient(response.uri);
        }
    }

    protected override ServerRequest GetRequest() => new ServerRequest();

    protected override ServerResponse ProcessRequest(ServerRequest request, IPEndPoint endpoint)
    {
        return new ServerResponse
        {
            serverId = serverId,
            uri = transport.ServerUri()
        };
    }

    

}
/*
[Serializable]
public class ServerRequest : NetworkMessage { }

[Serializable]
public class ServerResponse : NetworkMessage
{
    public long serverId;
    public IPEndPoint EndPoint { get; set; }
    public Uri uri;

    public bool SupportsUri() => true;

    public void Deserialize(NetworkReader reader)
    {
        serverId = reader.ReadLong();
        uri = new Uri(reader.ReadString());
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteLong(serverId);
        writer.Write(uri.ToString());
    }
}
*/
