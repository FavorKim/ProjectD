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
    Dictionary<StateName,SurvivorMoveState> states = new Dictionary<StateName, SurvivorMoveState>();
    public SurvivorStateMachine(Survivor owner)
    {
        this.owner = owner;
        states.Add(StateName.Walk, new SurvivorWalk(owner));
        states.Add(StateName.Run, new SurvivorRun(owner));
        states.Add(StateName.Crouch, new SurvivorCrouch(owner));
        states.Add(StateName.Idle, new SurvivorIdle(owner));
        curState = states[StateName.Idle];
    }

    public void ChangeState(StateName stateName)
    {
        curState.Exit();
        curState = states[stateName];
        curState.Enter();
    }

    public void Transition()
    {
        if (owner.GetMoveDir() == Vector3.zero) 
        {
            if (Input.GetKey(KeyCode.LeftShift))
                ChangeState(StateName.Crouch);
            else
                ChangeState(StateName.Idle);
        }
        else ChangeState(StateName.Walk);

        
        if (Input.GetKey(KeyCode.LeftShift))
            ChangeState(StateName.Run);

        else if(Input.GetKey(KeyCode.LeftControl))
            ChangeState(StateName.Crouch);


    }
}

public class SurvivorMoveState
{
    protected readonly float walkSpeed = 200.0f;
    protected readonly float runSpeed = 500.0f;
    protected readonly float crouchSpeed = 100.0f;
    protected Animator Animator => owner.GetAnimator();
    protected Survivor owner;
    public SurvivorMoveState(Survivor owner)
    {
        this.owner = owner;
    }

    public virtual void Enter() { }
    public virtual void Excute() { }
    public virtual void Exit() { }
}

public class SurvivorWalk : SurvivorMoveState
{
    public SurvivorWalk(Survivor owner) : base(owner) { }

    public override void Enter() 
    {
        owner.MoveSpeed = walkSpeed;
    }
    public override void Excute() { }
    public override void Exit() 
    {

    }
}

public class SurvivorRun : SurvivorMoveState
{
    public SurvivorRun(Survivor owner) : base(owner) { }
    public override void Enter()
    {
        owner.MoveSpeed = runSpeed;
        Animator.SetBool("isRun", true);
    }

    public override void Excute() { }

    public override void Exit() 
    {
        Animator.SetBool("isRun", false);
    }
}

public class SurvivorCrouch : SurvivorMoveState
{
    public SurvivorCrouch(Survivor owner) : base(owner) { }
    public override void Enter()
    {
        Animator.SetBool("isCrouch", true);
        owner.MoveSpeed = crouchSpeed;
    }

    public override void Excute() 
    {
            
    }

    public override void Exit()
    {
        Animator.SetBool("isCrouch", false);
    }
}

public class SurvivorIdle : SurvivorMoveState
{
    public SurvivorIdle(Survivor owner) : base(owner) { }
    public override void Enter() 
    {
        Animator.Rebind();
    }
    public override void Excute() 
    {

    }
    public override void Exit() 
    {

    }
}
