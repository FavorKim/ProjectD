using Mirror;

public class NetworkPoolObject : NetworkBehaviour
{
    [Command]
    public void Cmd_SetActive(bool isTrue)
    {
        Rpc_SetActive(isTrue);
    }

    [ClientRpc]
    public void Rpc_SetActive(bool isTrue) 
    {
        gameObject.SetActive(isTrue);
    }

}
