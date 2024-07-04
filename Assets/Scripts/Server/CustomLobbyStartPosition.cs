using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class CustomLobbyStartPosition : SingletonNetwork<CustomLobbyStartPosition>
{
    public CustomStartPosition[] StartPositions;
    private void Awake()
    {
        
        for(int i=0; i<transform.childCount; i++)
        {
            StartPositions = GetComponentsInChildren<CustomStartPosition>();
        }
    }
    
    public Transform GetStartPosition(bool isHost, out int index)
    {
        Transform pos = default;
        if (isHost)
        {
            index = 0;
            pos = StartPositions[0].transform;
            return pos;
        }
        else
        {
            for (int i = 0; i < StartPositions.Length; i++)
            {
                if (i == 0) continue;
                if (StartPositions[i].GetIsAvailable())
                {
                    pos = StartPositions[i].transform;
                    StartPositions[i].CmdSetIsAvailable(false);
                    index = i;
                    return pos;
                }
            }
        }
        index = 999;
        return pos;
    }
    
    public void OnDiconnected(int index)
    {
        StartPositions[index].CmdSetIsAvailable(true);
    }

    /*
    [ClientRpc]
    void RpcSetStartPosIsAvailable(int index, bool isAvailable)
    {
        StartPositions[index].SetIsAvailable(isAvailable);
    }
    */
}
/*
public class StartPos
{
    Transform transform;
    bool isAvailable;

    public StartPos(Transform transform)
    {
        this.transform = transform;
        isAvailable = true;
    }

    public Transform GetTransform()
    {
        isAvailable = false;
        return transform;
    }
    public void SetIsAvailable(bool isAvailable)
    {
        this.isAvailable = isAvailable;
    }
}*/