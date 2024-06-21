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
        NetworkConnectionToClient conn = NetworkServer.localConnection;
        pref = conn.connectionId == 0 ? KillerPref : SurvivorPref;
        CmdSpawnPlayer(conn, pref);
    }


    [Command(requiresAuthority =false)]
    void CmdSpawnPlayer(NetworkConnectionToClient conn, GameObject pref)
    {
        var obj = Instantiate(pref, transform.position, Quaternion.identity);
        NetworkServer.ReplacePlayerForConnection(conn, obj);
    }
}
