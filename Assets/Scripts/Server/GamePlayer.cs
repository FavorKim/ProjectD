using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayer : NetworkBehaviour
{
    [SerializeField] GameObject SurvivorPref;
    [SerializeField] GameObject KillerPref;

    [SerializeField] GameObject pref;

    

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSpawnPlayer();
    }



    [Command(requiresAuthority =false)]
    void CmdSpawnPlayer()
    {
        NetworkConnectionToClient conn = connectionToClient;

        pref = conn == NetworkServer.localConnection ? KillerPref : SurvivorPref;

        //pref = conn.connectionId == 0 ? KillerPref : SurvivorPref;
        var obj = Instantiate(pref, transform.position, Quaternion.identity);
        NetworkServer.Spawn(obj, conn);
        NetworkServer.ReplacePlayerForConnection(conn, obj, false);
    }
}
