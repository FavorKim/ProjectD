using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectPool : NetworkBehaviour
{

    [SerializeField] GameObject ObjectPrefab;
    [SerializeField] int PoolSize;

    Queue<NetworkPoolObject> pool = new Queue<NetworkPoolObject>();


    private void Start()
    {
        InitPool();
    }

    public void InitPool()
    {
        if (pool.Count > 0) return;

        for (int i = 0; i < PoolSize; i++)
        {
            var obj = Instantiate(ObjectPrefab, transform).GetComponent<NetworkPoolObject>();
            pool.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }
    }

    public virtual NetworkPoolObject GetObj()
    {
        var obj = pool.Dequeue();
        obj.Cmd_SetActive(true);
        return obj;
    }

    public virtual void ReturnObj(NetworkPoolObject obj)
    {
        pool.Enqueue(obj);
        
        obj.Cmd_SetActive(false);
    }
}