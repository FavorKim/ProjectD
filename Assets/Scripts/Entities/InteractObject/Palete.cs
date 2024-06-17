using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Palete : IInteractableObject
{
    Animator m_anim;
    public bool isUsed;
    public void Interact()
    {
        if (isUsed)
        {
            //부서지기
        }
        else
        {
            //내려지기
        }
    }
}
