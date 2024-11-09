using System;
using System.Collections;
using UnityEngine;



public abstract class StatusPerk : MonoBehaviour
{
    

    //public PlayableCharacter m_owner;
    public PerkType PerkType;

    protected float m_maxCoolTime;
    public float MaxCoolTime
    {
        get { return m_maxCoolTime; }
        protected set
        {
            if(m_maxCoolTime != value)
            {
                m_maxCoolTime = value;
            }
        }
    }

    protected float m_curCoolTime = 0;
    public float CurCoolTime
    {
        get
        {
            return m_curCoolTime;
        }
        protected set
        {
            if (m_curCoolTime != value)
            {
                m_curCoolTime = value;
                OnChangedCoolTime.Invoke();
                if (m_curCoolTime >= m_maxCoolTime)
                {
                    if (Condition() == true)
                        OnActivate.Invoke();
                }
            }
        }
    }
    protected float m_duration;

    protected abstract bool Condition();


    public virtual void Init(float cool, float duration, PerkType type)
    {
        m_maxCoolTime = cool;
        m_duration = duration;
        m_curCoolTime = m_maxCoolTime;
        PerkType = type;
    }

    public virtual void Init(float value, float cool, float duration, PerkType type)
    {
        m_maxCoolTime = cool;
        m_duration = duration;
        m_curCoolTime = m_maxCoolTime;
        PerkType = type;
    }



    protected virtual void OnEnable()
    {
        OnActivate += Activation;
        OnEffectStart += ResetCoolTime;


    }

    protected virtual void OnDisable()
    {
        OnEffectStart -= ResetCoolTime;
        OnActivate -= Activation;
    }



    protected virtual IEnumerator CorCoolTime()
    {
        while (true)
        {
            CurCoolTime += Time.deltaTime;
            yield return null;
        }
    }

    protected void Activation()
    {
        StartCoroutine(CorActivate());
    }

    protected void ResetCoolTime()
    {
        CurCoolTime = 0;
    }

    IEnumerator CorActivate()
    {
        OnEffectStart.Invoke();
        yield return new WaitForSeconds(m_duration);
        OnEffectEnd.Invoke();
    }

    public event Action OnActivate;
    public event Action OnEffectStart;
    public event Action OnEffectEnd;
    public event Action OnChangedCoolTime;


}
