using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameResult
{
    Escape, Sacrificed, ALLKILL, SOSO, NoKill
}

public class GameResultManager : SingletonNetwork<GameResultManager>
{
    [SerializeField] GameObject Prefab_Survivor_Clear;
    [SerializeField] GameObject Prefab_Killer_Clear;
    [SerializeField] Transform Pos_GameResult;
    [SerializeField] GameObject PlayerUI;

    [SerializeField]
    int playerRemaining;

    [SerializeField]
    int playerKilled = 0;
    public int PlayerKilled
    {
        get { return playerKilled; }
        set 
        {
            if (playerKilled != value)
            {
                playerKilled = value;
                CmdSetPlayerKilled(value);
            }
        }
    }
    private void Start()
    {
        if(instance != null && instance.gameObject !=this.gameObject)
            DestroyImmediate(instance.gameObject);
        instance = this;
        PlayerKilled = 0;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Invoke(nameof(SetPlayerRemaining), 10);
    }

    void SetPlayerRemaining()
    {
        playerRemaining = NetworkServer.connections.Count - 1;
    }

    public void SetGameResult(GameResult result)
    {
        PlayerUI.SetActive(false);

        if (result < GameResult.ALLKILL)
        {
            var survivor = Instantiate(Prefab_Survivor_Clear, Pos_GameResult.position, Pos_GameResult.rotation).GetComponent<ClearSurvivor>();
            if (survivor != null)
            {
                survivor.SetResultText(result);
                CmdDecreasePlayerRemaining();
                if (result == GameResult.Sacrificed) 
                {
                    PlayerKilled = playerKilled + 1;
                }
                
            }
        }
        else
        {
            var killer = Instantiate(Prefab_Killer_Clear, Pos_GameResult.position, Pos_GameResult.rotation).GetComponent<ClearKiller>();
            if (killer != null)
            {
                killer.SetResultText(result);
            }
        }
    }

    [Command(requiresAuthority =false)]
    void SetPlayerNoRemaining()
    {
        if (PlayerKilled == 0) SetGameResult(GameResult.NoKill);
        else if (PlayerKilled < 4) SetGameResult(GameResult.SOSO);
        else SetGameResult(GameResult.ALLKILL);
    }

    [Command(requiresAuthority =false)]
    void CmdDecreasePlayerRemaining()
    {
        playerRemaining--;
        if (playerRemaining == 0)
            SetPlayerNoRemaining();
    }

    [Command(requiresAuthority =false)]
    void CmdSetPlayerKilled(int val)
    {
        playerKilled = val;
    }


}
