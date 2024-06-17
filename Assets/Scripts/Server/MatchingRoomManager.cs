using kcp2k;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class MatchingRoomManager : NetworkBehaviour
{
    private static MatchingRoomManager instance;
    public static MatchingRoomManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindAnyObjectByType<MatchingRoomManager>();
                if(instance == null)
                {
                    var obj = new GameObject();
                    obj.AddComponent<NetworkIdentity>();
                    instance = obj.AddComponent<MatchingRoomManager>();
                    DontDestroyOnLoad(instance);
                }
            }
            return instance;
        }
    }

    List<MatchRoom> matchRooms = new();
    [SerializeField] MatchRoom roomPrefab;

    [SyncVar]
    ushort roomCount = 0;
    public ushort GetRoomCount() { return  roomCount; }

    [Command(requiresAuthority = false)]
    public void CreateRoom()
    {
        MatchRoom room = Instantiate(roomPrefab);
        var newPort = new KcpTransport();
        newPort.Port = roomCount;
        roomCount++;
        room.InitRoom(newPort);
        matchRooms.Add(room);
        NetworkServer.Spawn(room.gameObject);
    }

    public MatchRoom GetRoomAvailable()
    {
        MatchRoom roomAvailable = null;
        foreach(MatchRoom room in matchRooms)
        {
            if(room.CanJoin)
                roomAvailable = room;
        }
        return roomAvailable;
    }

}
