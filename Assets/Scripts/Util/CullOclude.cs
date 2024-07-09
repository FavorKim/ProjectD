using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullOclude : MonoBehaviour
{

    private void Update()
    {
        Cull();
    }


    private void Cull()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.forward,out hit, 3.0f))
        {
            transform.position = hit.point + transform.forward;
        }
    }
}
