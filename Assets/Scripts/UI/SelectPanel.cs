using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPanel : MonoBehaviour
{
    public void OnClick_Survivor()
    {
        var manager = MyNetworkManager.singleton as MyNetworkManager;
        manager.OnClick_Survivor();
    }

    public void OnClick_Killer()
    {
        var manager = MyNetworkManager.singleton as MyNetworkManager;
        manager.OnClick_Killer();
    }
}
