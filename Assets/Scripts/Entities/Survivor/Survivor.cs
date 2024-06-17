using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Survivor : PlayableCharactor
{
    CharacterController m_CharacterController;
    [SerializeField] Animator Animator;
    SurvivorStateMachine m_StateMachine;

    Vector3 MoveDir;
    Vector2 dir;

    [SerializeField] float moveSpeed;
    public float MoveSpeed {  get { return moveSpeed; } set {  moveSpeed = value; } }
    [SerializeField] float rotateSpeed;

    public Vector3 GetMoveDir() { return MoveDir; }

    public Animator GetAnimator() { return Animator; }

    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_StateMachine = new SurvivorStateMachine(this);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        m_StateMachine.Transition();
    }


    void OnMove(InputValue val)
    {
        //if (isDead) return;

        dir = val.Get<Vector2>();
        MoveDir = dir.y * Camera.main.transform.forward + dir.x * Camera.main.transform.right;
        MoveDir = new Vector3(MoveDir.x, 0, MoveDir.z);
        MoveDir.Normalize();
        MoveDir *= moveSpeed * Time.deltaTime;

        if (MoveDir != Vector3.zero) Animator.SetBool("isWalk", true);
        else Animator.SetBool("isWalk", false);
    }


    void PlayerMove()
    {
        MoveDir = dir.y * Camera.main.transform.forward + dir.x * Camera.main.transform.right;
        MoveDir = new Vector3(MoveDir.x, 0, MoveDir.z);
        MoveDir.Normalize();
        MoveDir *= moveSpeed * Time.deltaTime;

        if (MoveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MoveDir), rotateSpeed * Time.deltaTime);
        }
        m_CharacterController.SimpleMove(MoveDir);
    }


    public override void InteractObject(Generator generator)
    {
        // 발전기 작동 애니메이션 출력
    }
    public override void InteractObject(Palete palete)
    {
        // 발전기 내리기 혹은 발전기 넘기
        if (palete.isUsed)
        {
            // 발전기 넘기
        }
        else
        {
            // 발전기 내리기
        }
    }

    public override void InteractObject(Window window)
    {
        // 창틀 뛰어넘기
    }
}
