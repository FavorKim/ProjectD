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

    public int PlayerRemaining
    {
        get { return playerRemaining; }
        private set
        {
            if (playerRemaining != value)
            {
                playerRemaining = value;
                if (playerRemaining == 0)
                    SetPlayerNoRemaining();
            }
        }
    }
    [SerializeField]
    int playerKilled;
    


    private void Awake()
    {
        PlayerRemaining = 0;
        playerKilled = 0;
    }

    private void Start()
    {
        if(instance != null && instance.gameObject !=this.gameObject)
            DestroyImmediate(instance.gameObject);
        instance = this;

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
                PlayerRemaining--;
                if (result == GameResult.Sacrificed) playerKilled++;
                
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
        if (playerKilled == 0) SetGameResult(GameResult.NoKill);
        else if (playerKilled < 4) SetGameResult(GameResult.SOSO);
        else SetGameResult(GameResult.ALLKILL);
    }

    [Command(requiresAuthority =false)]
    public void CmdIncreaseRemainingPlayer()
    {
        PlayerRemaining++;
    }


}
