using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExhaustPerk
{
    public Survivor m_owner;


    private float m_maxCoolDown;

    protected float m_curCoolDown=0;
    protected float CurCoolDown
    {
        get
        {
            return m_curCoolDown;
        }
        set
        {
            if (m_curCoolDown != value)
            {
                m_curCoolDown = value;
                if (m_curCoolDown >= m_maxCoolDown)
                {
                    Activate();
                }
            }
        }
    }
    protected float m_duration;
    protected float m_moveSpeed;

    public ExhaustPerk(float speed, float cool, float duration)
    {
        m_duration = duration;
        m_moveSpeed = speed;
        m_maxCoolDown = cool;
    }


    protected abstract bool Condition();

    protected virtual void Activate()
    {
        if (Condition()==true)
        {
            m_owner.StartCoroutine(CorActivation());
        }
    }
    IEnumerator CorActivation()
    {
        CurCoolDown = -m_duration;
        m_owner.MoveSpeed += m_moveSpeed;
        yield return new WaitForSeconds(m_duration);
        m_owner.MoveSpeed -= m_moveSpeed;
    }
    public IEnumerator CorCoolTime()
    {
        while (true)
        {
            CurCoolDown--;
            yield return null;
        }
    }
}
