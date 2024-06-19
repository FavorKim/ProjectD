using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HealthStates
{
    Hanged,
    Held,
    Down,
    Injured,
    Healthy,
}

public class SurvivorHealthStateMachine
{
    private SurvivorHealthBaseState curState;

    Dictionary<HealthStates, SurvivorHealthBaseState> states = new Dictionary<HealthStates, SurvivorHealthBaseState>();
    private Survivor owner;

    public SurvivorHealthStateMachine(Survivor owner)
    {
        this.owner = owner;
        states.Add(HealthStates.Healthy, new Healthy(owner, this));
        states.Add(HealthStates.Injured, new Injured(owner, this));
        states.Add(HealthStates.Down, new Down(owner, this));
        curState = states[HealthStates.Healthy];
        owner.OnHitted += GetHit;
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
    void GetHit()
    {
        if (curState.GetStateEnum() > HealthStates.Down)
            ChangeState(curState.GetStateEnum() - 1);
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

    }

    public virtual void Enter() { /*Debug.Log(myEnum.ToString());*/ }
    public virtual void Excute()
    {

    }
    public virtual void Exit() { }

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
        owner.GetAnimator().SetTrigger("Down");
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
public class Held : SurvivorHealthBaseState
{
    public Held(Survivor owner, SurvivorHealthStateMachine fsm) : base(owner, fsm) {myEnum = HealthStates.Held; } 

    public override void Enter() 
    {
        base.Enter();
        owner.IsBleeding = false;
        owner.IsFreeze = true;
    }
    public override void Excute()
    {
        base.Excute();
    }
    public override void Exit()
    {
        base.Exit();
        owner.IsFreeze = false;
    }
}
public class Hanged : SurvivorHealthBaseState
{
    public Hanged(Survivor owner, SurvivorHealthStateMachine fsm) : base(owner, fsm) { myEnum = HealthStates.Hanged; }

    public override void Enter()
    {
        base.Enter();
        owner.IsBleeding = false;
        owner.IsFreeze = true;
    }
    public override void Excute()
    {
        base.Excute();
    }
    public override void Exit()
    {
        base.Exit();
        owner.IsFreeze = false;
    }
}


