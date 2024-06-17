using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Palete : MonoBehaviour,IInteractableObject
{
    Animator m_anim;
    public bool isUsed = false;
    private void Awake()
    {
        m_anim = GetComponent<Animator>();
    }
    public void Interact()
    {
        if (isUsed)
        {
            //부서지기
        }
        else
        {
            m_anim.SetTrigger("Fall");
            isUsed = true;
        }
    }
}
