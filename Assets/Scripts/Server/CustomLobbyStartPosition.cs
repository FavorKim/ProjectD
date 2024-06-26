using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLobbyStartPosition : MonoBehaviour
{
    public static Stack<Transform> StartPositions;

    private void Awake()
    {
        StartPositions = new Stack<Transform>();
        for(int i=0; i<transform.childCount; i++)
        {
            StartPositions.Push(transform.GetChild(i));
        }
    }

    public static Transform GetStartPosition()
    {
        return StartPositions.Pop();
    }
    public static void OnDiconnected(Transform pushDest)
    {
        StartPositions.Push(pushDest);
    }

    
}
