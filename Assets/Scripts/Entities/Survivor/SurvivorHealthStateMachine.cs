using System;
using System.Collections.Generic;


public enum HealthStates
{
    Healthy,
    Injured,
    Down,
    Hanged,
    Held,
}

public class SurvivorHealthStateMachine
{
    private SurvivorHealthBaseState curState;

    Dictionary<HealthStates, SurvivorHealthBaseState> states = new Dictionary<HealthStates, SurvivorHealthBaseState>();
    public HealthStates GetCurState() { return curState.GetStateEnum(); }
    private Survivor owner;

    public SurvivorHealthStateMachine(Survivor owner)
    {
        this.owner = owner;
        states.Add(HealthStates.Healthy, new Healthy(owner, this));
        states.Add(HealthStates.Injured, new Injured(owner, this));
        states.Add(HealthStates.Down, new Down(owner, this));
        states.Add(HealthStates.Held, new Held(owner, this));
        states.Add(HealthStates.Hanged, new Hanged(owner, this));
        curState = states[HealthStates.Healthy];
        owner.OnHitted += GetHit;
    }

    public void ChangeState(HealthStates toChangeStateEnum)
    {
        curState.Exit();
        curState = states[toChangeStateEnum];
        curState.Enter();
        PlayerUIManager.Instance.SetPlayerUIState(owner.PlayerID(), curState.GetStateEnum());
        owner.HealGauge = 0;
    }

    public void Healed()
    {
        ChangeState(curState.GetStateEnum() - 1);
    }
    
    public void StateUpdate()
    {
        curState.Excute();
    }

    public void UnRegisterEvent()
    {
        owner.OnHitted -= GetHit;
    }
    void GetHit()
    {
        if (curState.GetStateEnum() < HealthStates.Down)
        {
            ChangeState(curState.GetStateEnum() + 1);
            owner.IsFreeze = false;
        }

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
        owner.MoveSpeed = owner.GetDownSpeed();
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
        owner.GetAnimator().SetTrigger("Held");
    }
    public override void Excute()
    {
        base.Excute();
        //owner.transform.position = owner.HeldPosition;
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
        owner.GetAnimator().SetTrigger("Hang");
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


