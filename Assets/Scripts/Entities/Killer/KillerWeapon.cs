using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerWeapon : MonoBehaviour
{
    BoxCollider m_col;

    private void Start()
    {
        m_col = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var survivor = other.GetComponent<Survivor>();
        if (survivor!=null)
        {
            survivor.GetHit();
            m_col.enabled = false;
        }
        
    }
}
