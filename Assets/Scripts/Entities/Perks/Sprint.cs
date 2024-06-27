
public class Sprint : StatusPerk
{
    protected float m_moveSpeed;

    private void Start()
    {
        StartCoroutine(CorCoolTime());
    }

    public void Init(float speed, float cool, float duration, Survivor owner, PerkType type)
    {
        m_duration = duration;
        m_moveSpeed = speed;
        m_maxCoolTime = cool;
        m_curCoolTime = m_maxCoolTime;
        m_owner = owner;
        PerkType = type;
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
        return (m_owner as Survivor).GetMoveState().CurStateIs(SurvivorStateMachine.StateName.Run);
    }

    void OnEffectStart_RunSpeedUp()
    {
        m_owner.MoveSpeed += m_moveSpeed;
    }
    void OnEffectEnd_ResetRunSpeed()
    {
        m_owner.MoveSpeed -= m_moveSpeed;
    }

}
