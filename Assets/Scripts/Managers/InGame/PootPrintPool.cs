using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PootPrintPool : SingletonNetwork<PootPrintPool>
{
    NetworkObjectPool pootPrintPool;
    [SerializeField] float Duration_PootPrint;

    private void Start()
    {
        if (instance != null)
        {
            if (instance != this)
                DestroyImmediate(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this);

        pootPrintPool = GetComponent<NetworkObjectPool>();
    }

    public void Init()
    {
        pootPrintPool.InitPool();
    }

    [ClientRpc]
    public void PrintPootPrint(Vector3 pos, Quaternion rot)
    {
        var obj = pootPrintPool.GetObj();
        obj.transform.position = pos;
        obj.transform.rotation = rot;

        StartCoroutine(CorInvokeReturn(obj));
    }

    IEnumerator CorInvokeReturn(NetworkPoolObject obj)
    {
        yield return new WaitForSeconds(Duration_PootPrint);
        pootPrintPool.ReturnObj(obj);
    }
}
