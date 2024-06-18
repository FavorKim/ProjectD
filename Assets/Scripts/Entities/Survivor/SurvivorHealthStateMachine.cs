using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public enum HealthStates
    {
        Down,
        Injured,
        Healthy,
    }

public class SurvivorHealthStateMachine
{
    private SurvivorHealthBaseState curState;

    Dictionary<HealthStates, SurvivorHealthBaseState> states = new Dictionary<HealthStates, SurvivorHealthBaseState>();

    //public Dictionary<HealthStates,SurvivorHealthBaseState> GetStateDict() {  return states; }

    public SurvivorHealthStateMachine(Survivor owner)
    {
        states.Add(HealthStates.Healthy, new Healthy(owner, this));
        states.Add(HealthStates.Injured, new Injured(owner, this));
        states.Add(HealthStates.Down, new Down(owner, this));
        curState = states[HealthStates.Healthy];
    }

    public void ChangeState(HealthStates toChangeStateEnum)
    {
        curState.Exit();
        curState = states[toChangeStateEnum];
        curState.Enter();
    }
    public void ChangeState(SurvivorHealthBaseState toChangeState)
    {
        curState.Exit();
        curState = toChangeState;
        curState.Enter();
    }
    public HealthStates GetCurState() { return curState.GetStateEnum(); }
    public void StateUpdate()
    {
        curState.Excute();
    }
}

public class SurvivorHealthBaseState
{
    protected HealthStates myEnum;

    protected Survivor owner;
    protected SurvivorHealthStateMachine fsm;
    public SurvivorHealthBaseState(Survivor owner, SurvivorHealthStateMachine fsm)
    {
        this.owner = owner;
        this.fsm = fsm;
        owner.OnHitted += GetHit;

    }

    public virtual void Enter() { }
    public virtual void Excute()
    {

    }
    public virtual void Exit() { }

    protected void GetHit()
    {
        var lowerState = (HealthStates)((int)myEnum - 1);
        fsm.ChangeState(lowerState);
    }
    public HealthStates GetStateEnum() { return myEnum; }
}

public class Healthy : SurvivorHealthBaseState
{
    public Healthy(Survivor owner, SurvivorHealthStateMachine fsm) : base(owner, fsm) { myEnum = HealthStates.Healthy; }

    public override void Enter()
    {
        base.Enter();
        owner.IsBleeding = false;
    }
    public override void Excute()
    {
        base.Excute();
    }
    public override void Exit()
    {
        base.Exit();
    }
}
public class Injured : SurvivorHealthBaseState
{
    public Injured(Survivor owner, SurvivorHealthStateMachine fsm) : base(owner, fsm) { myEnum = HealthStates.Injured; }

    public override void Enter()
    {
        base.Enter();
        owner.IsBleeding = true;
    }
    public override void Excute()
    {
        base.Excute();
    }
    public override void Exit()
    {
        base.Exit();
    }
}
public class Down : SurvivorHealthBaseState
{
    public Down(Survivor owner, SurvivorHealthStateMachine fsm) : base(owner, fsm) { myEnum = HealthStates.Down; }

    public override void Enter()
    {
        base.Enter();
        owner.IsBleeding = true;
        owner.MoveSpeed = 100;
        owner.GetAnimator().SetBool("isDown", true);
    }
    public override void Excute()
    {
        base.Excute();
    }
    public override void Exit()
    {
        base.Exit();
        owner.GetAnimator().SetBool("isDown", false);
    }
}

