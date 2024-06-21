using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorStateMachine
{
    public enum StateName
    {
        Walk, Run, Crouch, Idle
    }

    SurvivorMoveState curState;
    Survivor owner;
    Dictionary<StateName, SurvivorMoveState> states = new Dictionary<StateName, SurvivorMoveState>();
    public bool CurStateIs(StateName stateName) { return curState == states[stateName]; }

    public SurvivorStateMachine(Survivor owner)
    {
        this.owner = owner;
        states.Add(StateName.Walk, new SurvivorWalk(owner, this));
        states.Add(StateName.Run, new SurvivorRun(owner, this));
        states.Add(StateName.Crouch, new SurvivorCrouch(owner, this));
        states.Add(StateName.Idle, new SurvivorIdle(owner, this));
        curState = states[StateName.Idle];
    }

    public void ChangeState(StateName stateName)
    {
        curState.Exit();
        curState = states[stateName];
        curState.Enter();
    }

    public void StateUpdate() { curState.Excute(); }
}

public class SurvivorMoveState
{
    protected float walkSpeed => owner.GetWalkSpeed();
    protected float runSpeed => owner.GetRunSpeed();
    protected float crouchSpeed => owner.GetCrouchSpeed();
    protected Animator Animator => owner.GetAnimator();
    protected Survivor owner;
    protected SurvivorStateMachine state;

    public SurvivorMoveState(Survivor owner, SurvivorStateMachine state)
    {
        this.owner = owner;
        this.state = state;
    }

    public virtual void Enter() { }
    public virtual void Excute()
    {
        if (owner.GetMoveDir() == Vector3.zero && !Input.GetKey(KeyCode.LeftControl))
        {
            state.ChangeState(SurvivorStateMachine.StateName.Idle); 
        }
    }
    public virtual void Exit() { }
}

public class SurvivorWalk : SurvivorMoveState
{
    public SurvivorWalk(Survivor owner, SurvivorStateMachine state) : base(owner, state) { }

    public override void Enter()
    {
        owner.MoveSpeed = walkSpeed;
    }
    public override void Excute()
    {
        base.Excute();
        if (Input.GetKey(KeyCode.LeftShift) 
            && owner.GetMoveDir() != Vector3.zero) 
            state.ChangeState(SurvivorStateMachine.StateName.Run);

        if (Input.GetKey(KeyCode.LeftControl)) state.ChangeState(SurvivorStateMachine.StateName.Crouch);
    }
    public override void Exit()
    {

    }
}

public class SurvivorRun : SurvivorMoveState
{
    public SurvivorRun(Survivor owner, SurvivorStateMachine state) : base(owner, state) { }
    public override void Enter()
    {
        owner.MoveSpeed = runSpeed;
        Animator.SetBool("isRun", true);
        owner.StartCoroutine(owner.CorPrintFoot());
    }

    public override void Excute()
    {
        base.Excute();

        if (Input.GetKey(KeyCode.LeftControl)) state.ChangeState(SurvivorStateMachine.StateName.Crouch);
        if (Input.GetKeyUp(KeyCode.LeftShift)) state.ChangeState(SurvivorStateMachine.StateName.Walk);

    }

    public override void Exit()
    {
        Animator.SetBool("isRun", false);
        //owner.StopCoroutine(owner.CorPrintFoot());

    }
}

public class SurvivorCrouch : SurvivorMoveState
{
    public SurvivorCrouch(Survivor owner, SurvivorStateMachine state) : base(owner, state) { }
    public override void Enter()
    {

        Animator.SetBool("isCrouch", true);
        owner.MoveSpeed = crouchSpeed;
    }

    public override void Excute()
    {
        base.Excute();
        if (Input.GetKey(KeyCode.LeftShift)
            && owner.GetMoveDir() != Vector3.zero)
            state.ChangeState(SurvivorStateMachine.StateName.Run);

        if (Input.GetKeyUp(KeyCode.LeftControl)) state.ChangeState(SurvivorStateMachine.StateName.Walk);

    }

    public override void Exit()
    {
        Animator.SetBool("isCrouch", false);
    }
}

public class SurvivorIdle : SurvivorMoveState
{
    public SurvivorIdle(Survivor owner, SurvivorStateMachine state) : base(owner, state) { }
    public override void Enter()
    {
        Animator.SetBool("isCrouch", false);
        Animator.SetBool("isWalk", false);
        Animator.SetBool("isRun", false);

    }
    public override void Excute()
    {
        if (owner.GetMoveDir() != Vector3.zero) state.ChangeState(SurvivorStateMachine.StateName.Walk);
        
        if (Input.GetKey(KeyCode.LeftShift)
            && owner.GetMoveDir() != Vector3.zero)
            state.ChangeState(SurvivorStateMachine.StateName.Run);

        if (Input.GetKey(KeyCode.LeftControl)) state.ChangeState(SurvivorStateMachine.StateName.Crouch);
    }
    public override void Exit()
    {

    }
}
