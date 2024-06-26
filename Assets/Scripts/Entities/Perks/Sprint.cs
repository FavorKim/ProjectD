
public class Sprint : ExhaustPerk
{
    public Sprint(float speed, float cool, float duration) : base(speed, cool, duration) { }

    protected override bool Condition()
    {
        if (m_owner.GetMoveState().CurStateIs(SurvivorStateMachine.StateName.Run))
            return true;
        else
            return false;
    }
}
