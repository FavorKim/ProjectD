using DungeonArchitect;
using Mirror;
using UnityEngine;

public class CustomRoomPlayer : NetworkRoomPlayer
{
    [SerializeField] GameObject Killer;
    [SerializeField] GameObject Survivor;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        SpawnLobbyPlayer();
    }

    [Command(requiresAuthority =false)]
    void SpawnLobbyPlayer()
    {
        Transform startPos = CustomLobbyStartPosition.GetStartPosition();
        GameObject obj;
        if (CustomLobbyStartPosition.StartPositions.Count==4)
        {
            obj = Instantiate(Killer, startPos.position, startPos.rotation);
        }
        else
        {
            obj = Instantiate(Survivor, startPos.position, startPos.rotation);
        }
        NetworkServer.Spawn(obj);
    }

}
