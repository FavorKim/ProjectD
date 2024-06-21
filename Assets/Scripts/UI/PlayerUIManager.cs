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
    //rpc
    public int CreatePlayerUI()
    {
        var obj = Instantiate(PlayerUIPrefab, GridLayout.transform).GetComponent<PlayerUI>();
        var id = playerUIs.Count;
        playerUIs.Add(id, obj);
        return id;
    }

    //rpc
    public void SetPlayerUIState(int id, HealthStates state)
    {
        playerUIs[id].SetIcon(state);
    }
    //rpc
    public void SetPlayerUIState(int id, PlayerUI.Icons state)
    {
        playerUIs[id].SetIcon(state);
    }

    
    public void SetPlayerUIGauge(int id, float value)
    {
        playerUIs[id].OnCorruptTimeChanged(value);
    }

}
