using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable 
{
    //[SerializeField] private float m_moveSpeed;
    public float MoveSpeed
    {
        get;
        set;
    }

}
