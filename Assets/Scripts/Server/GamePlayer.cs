using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayer : NetworkBehaviour
{
    [SerializeField] GameObject SurvivorPref;
    [SerializeField] GameObject KillerPref;

    GameObject pref;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSpawnPlayer();
    }


    [Command(requiresAuthority =false)]
    void CmdSpawnPlayer()
    {
        NetworkConnectionToClient conn = connectionToClient;
        pref = conn == NetworkServer.localConnection? KillerPref : SurvivorPref;

        var obj = Instantiate(pref, transform.position, Quaternion.identity);
        NetworkServer.Spawn(obj);
        NetworkServer.ReplacePlayerForConnection(conn, obj);
    }
}
