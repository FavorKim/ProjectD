using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyStateHUD : SingletonNetwork<ReadyStateHUD>
{
    Image[] Images;
    
    void Start()
    {
        Images = GetComponentsInChildren<Image>();
    }
    /*
    [Command(requiresAuthority =false)]
    public void CmdSetImgColor(int index, bool isReady)
    {
        RpcSetImageColor(index, isReady);
    }
    */
    [ClientRpc]
    public void RpcSetImageColor(int index, bool isReady)
    {
        Images[index].color = isReady ? Color.red : Color.white;
    }
}
