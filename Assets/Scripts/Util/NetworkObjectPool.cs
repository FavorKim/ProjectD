using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectPool : NetworkBehaviour
{
    //private static NetworkObjectPool instance;
    //public static NetworkObjectPool Instance
    //{
    //    get
    //    {
    //        return instance;
    //    }
    //}

    [SerializeField] NetworkPoolObject ObjectPrefab;
    [SerializeField] int PoolSize;

    Queue<NetworkPoolObject> pool = new Queue<NetworkPoolObject>();

   
    

    public void InitPool()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            var obj = Instantiate(ObjectPrefab, transform);
            NetworkServer.Spawn(obj.gameObject);
            pool.Enqueue(obj);
            obj.Cmd_SetActive(false);
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
