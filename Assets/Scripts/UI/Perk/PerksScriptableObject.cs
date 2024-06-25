using System.Collections;
using System.Collections.Generic;
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

[CreateAssetMenu(fileName = "Perks", menuName = "Perk Scriptable Objects")]
public class PerksScriptableObject : ScriptableObject
{
    public string Owner;
    public string PerkName;
    public HandleValue Target;
    public float ValuePercentage;
    public float CoolTime;
    public float Duration;
    public Sprite IconImg;
    public string Description;
}
