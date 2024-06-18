using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInvoke : MonoBehaviour
{
    [SerializeField] float InvokeTime;
    private void OnEnable()
    {
        Invoke(nameof(DestroySelf), InvokeTime);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
