using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySetting : NetworkBehaviour
{
    private void Start()
    {
        SelectedPerkManager.Instance.ResetPerkList();
    }
}
