using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : NetworkBehaviour
{
    [SerializeField] GameObject PlayerUIPrefab;
    Dictionary<int, PlayerUI> playerUIs = new Dictionary<int, PlayerUI>();
    [SerializeField] GridLayoutGroup GridLayout;

    private static PlayerUIManager instance;
    public static PlayerUIManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    [Command(requiresAuthority =false)]
    public void CreatePlayerUI(Survivor survivor)
    {
        var obj = Instantiate(PlayerUIPrefab, GridLayout.transform).GetComponent<PlayerUI>();
        NetworkServer.Spawn(obj.gameObject);
        RpcCreatePlayerUI(obj,survivor);
    }

    [ClientRpc]
    public void RpcCreatePlayerUI(PlayerUI obj, Survivor survivor)
    {
        var id = playerUIs.Count;
        survivor.SetPlayerID(id);
        playerUIs.Add(id, obj);
    }

    

    public void SetPlayerUIState(int id, HealthStates state)
    {
        playerUIs[id].SetIcon(state);
    }
    public void SetPlayerUIState(int id, PlayerUI.Icons state)
    {
        playerUIs[id].SetIcon(state);
    }


    [Command(requiresAuthority =false)]
    public void SetPlayerUIGauge(int id, float value)
    {
        RpcSetPlayerUIGauge(id, value);
    }
    //rpc
    [ClientRpc]
    public void RpcSetPlayerUIGauge(int id, float value)
    {
        playerUIs[id].OnCorruptTimeChanged(value);
    }

}
