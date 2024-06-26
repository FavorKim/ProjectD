using UnityEngine;

public enum HandleValue
{
    MoveSpeed,
    AttackSpeed,
    HealSpeed,
    StunRecover,
    JumpSpeed,
    CrouchSpeed,
    BreakSpeed,
}
public class PerkData
{
    public string Owner;
    public string PerkName;
    public Sprite IconImg;
    public HandleValue EffectTarget;
    public float ValuePercentage;
    public float Duration;
    public string Description;
    public float CoolTime;
}

