
public class Sprint : ExhaustPerk
{
    public Sprint(float speed, float cool, float duration, Survivor owner) : base(speed, cool, duration, owner) { }

    protected override bool Condition()
    {
        return m_owner.GetMoveState().CurStateIs(SurvivorStateMachine.StateName.Run) ? true : false;
    }
}
