using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FootPrintPool : SingletonNetwork<FootPrintPool>
{
    NetworkObjectPool footPrintPool;
    [SerializeField] float Duration_FootPrint;

    private void Start()
    {
        if (instance != null)
        {
            if (instance != this)
                DestroyImmediate(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this);


    }
    private void Awake()
    {
        footPrintPool = GetComponent<NetworkObjectPool>();
    }


    [ClientRpc]
    public void PrintFootPrint(Vector3 pos, Quaternion rot)
    {
        var obj = footPrintPool.GetObj();
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        StartCoroutine(CorInvokeReturn(obj));
    }

    IEnumerator CorInvokeReturn(GameObject obj)
    {
        yield return new WaitForSeconds(Duration_FootPrint);
        footPrintPool.ReturnObj(obj);
        obj.gameObject.SetActive(false);
    }
}
