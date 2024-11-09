
using System.Collections;
using UnityEngine;

public class Sprint : StatusPerk
{
    protected float m_moveSpeed;

    private Survivor m_owner;

    private void Start()
    {
        StartCoroutine(CorCoolTime());
    }

    public void Init(float speed, float cool, float duration, Survivor owner, PerkType type)
    {
        m_duration = duration;
        m_moveSpeed = owner.RunSpeed * (speed - 100) * 0.01f;
        m_maxCoolTime = cool;
        m_curCoolTime = m_maxCoolTime;
        m_owner = owner;
        PerkType = type;
    }

    protected override IEnumerator CorCoolTime()
    {
        while (true)
        {
            CurCoolTime += Time.deltaTime;

            if (m_owner.GetMoveState().CurStateIs(SurvivorStateMachine.StateName.Run))
            {
                CurCoolTime -= Time.deltaTime;
            }
            
            yield return null;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnEffectStart += OnEffectStart_RunSpeedUp;
        OnEffectEnd += OnEffectEnd_ResetRunSpeed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnEffectEnd -= OnEffectEnd_ResetRunSpeed;
        OnEffectStart -= OnEffectStart_RunSpeedUp;
    }
    protected override bool Condition()
    {
        return m_owner.GetMoveState().CurStateIs(SurvivorStateMachine.StateName.Run);
    }

    void OnEffectStart_RunSpeedUp()
    {
        m_owner.RunSpeed += m_moveSpeed;
    }
    void OnEffectEnd_ResetRunSpeed()
    {
        m_owner.RunSpeed -= m_moveSpeed;
    }

}
