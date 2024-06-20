using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPoolObject : MonoBehaviour // NetworkBehaviour
{
    //[Command]
    public void Cmd_SetActive(bool isTrue)
    {
        Rpc_SetActive(isTrue);
    }

    //[ClientRpc]
    public void Rpc_SetActive(bool isTrue) 
    {
        gameObject.SetActive(isTrue);
    }

}
