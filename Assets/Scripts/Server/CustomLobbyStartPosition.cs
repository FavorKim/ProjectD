using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class CustomLobbyStartPosition : SingletonNetwork<CustomLobbyStartPosition>
{
    public List<Transform> StartPositions;
    [SyncVar(hook = nameof(Hook_OnChangedIndex))]
    int index;
    public int GetIndex() {  return index; }
    private void Awake()
    {
        StartPositions = new List<Transform>();
        for(int i=0; i<transform.childCount; i++)
        {
            StartPositions.Add(transform.GetChild(i));
        }
    }

    public Transform GetStartPosition()
    {
        var transform = StartPositions[index];
        index++;
        return transform;
    }
    public void OnDiconnected()
    {
        index--;
    }

    void Hook_OnChangedIndex(int old, int recent)
    {
        index = recent;
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