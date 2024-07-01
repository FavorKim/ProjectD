using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectPool : NetworkBehaviour
{

    [SerializeField] GameObject ObjectPrefab;
    [SerializeField] int PoolSize;

    Queue<GameObject> pool = new Queue<GameObject>();


    private void Start()
    {
        InitPool();
    }
    /*
    public override void OnStartServer()
    {
        base.OnStartServer();
        InitPool();
    }
    */
    public void InitPool()
    {
        if (pool.Count > 0) return;

        for (int i = 0; i < PoolSize; i++)
        {
            var obj = Instantiate(ObjectPrefab, transform);
            
            pool.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }
    }

    public virtual GameObject GetObj()
    {
        var obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public virtual void ReturnObj(GameObject obj)
    {
        pool.Enqueue(obj);
        obj.SetActive(false);
    }
}