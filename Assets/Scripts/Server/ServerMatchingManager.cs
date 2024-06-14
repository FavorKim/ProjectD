using Mirror;
using System.Collections.Generic;
using UnityEngine;


public class ServerMatchingManager : NetworkBehaviour
{
    
    private static ServerMatchingManager instance;
    public static ServerMatchingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<ServerMatchingManager>();
                if(instance == null)
                {
                    var obj = new GameObject(nameof(ServerMatchingManager));
                    obj.AddComponent<NetworkIdentity>();
                    instance = obj.AddComponent<ServerMatchingManager>();
                }
            }
            return instance;
        }
    }
    
    static List<ushort> portsAvailable = new List<ushort>();

    [SyncVar(hook =nameof(OnChangedRoomCount_Hook))]
    ushort roomCount = 0;

    //[Command(requiresAuthority =false)]
    public ushort GetRoomCount() { return roomCount; }


    public const ushort serverPort = 7777;

    

    [Command(requiresAuthority =false)]
    public void OnCreateRoom()
    {
        roomCount++;
        Debug.Log(roomCount);
        portsAvailable.Add(roomCount);
        //return roomCount;
    }
    
    [Command(requiresAuthority = false)]
    public void OnDeleteRoom(ushort roomPort)
    {
        if (portsAvailable.Contains(roomPort))
            portsAvailable.Remove(roomPort);
    }

    //[Command(requiresAuthority = false)]
    public ushort GetRoomAvailable() 
    {
        if (roomCount <= 0) return default;
        var randomRoom = Random.Range(0, roomCount + 1);
        return portsAvailable[randomRoom];
    }


    private void OnChangedRoomCount_Hook(ushort old, ushort recent)
    {
        roomCount = recent;
    }
}
