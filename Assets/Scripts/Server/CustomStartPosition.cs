using Mirror;
using UnityEngine;

public class CustomStartPosition : NetworkBehaviour
{
    bool isAvailable;
    public bool GetIsAvailable() {  return isAvailable; }

    private void Start()
    {
        isAvailable = true;
        
    }

    [Command(requiresAuthority = false)]
    public void CmdSetIsAvailable(bool isAvailable)
    {
        RpcSetIsAvailable(isAvailable);
    }

    [ClientRpc]
    void RpcSetIsAvailable(bool isAvailable)
    {
        this.isAvailable = isAvailable;
    }
}
