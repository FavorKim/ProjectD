using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Survivor : PlayableCharactor
{
    CharacterController m_CharacterController;
    [SerializeField] Animator Animator;
    SurvivorStateMachine m_StateMachine;
    DOTweenAnimation m_DOTween;
    IInteractableObject m_interactDest;

    Vector3 MoveDir;
    Vector2 dir;

    [SerializeField] float moveSpeed;
    public float MoveSpeed {  get { return moveSpeed; } set {  moveSpeed = value; } }
    [SerializeField] float rotateSpeed;

    bool isFreeze = false;

    public Vector3 GetMoveDir() { return MoveDir; }

    public Animator GetAnimator() { return Animator; }

    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_StateMachine = new SurvivorStateMachine(this);
        DOTween.Init();
        m_DOTween = GetComponent<DOTweenAnimation>();   
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        m_StateMachine.Transition();
    }


    void OnMove(InputValue val)
    {
        if (isFreeze) return;

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


    public override void Interact(Generator generator)
    {
        if(Input.GetMouseButtonDown(0))
        {
            Animator.SetTrigger("Generate");
            Animator.SetBool("isGenerating", true);
        }
        if (Input.GetMouseButton(0))
        {
            isFreeze = true;
            generator.Interact();
        }
        if(Input.GetMouseButtonUp(0))
        {
            Animator.SetBool("isGenerating", false);
            isFreeze = false;
        }
    }
    public override void Interact(Palete palete)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!palete.isUsed)
            {
                // 판자 내리기 애니메이션
                palete.Interact();
            }
            else
            {
                // 판자 넘고, 넘는 애니메이션
                OnJumpFence();
                //palete.Interact();
            }
        }
    }

    public override void Interact(JumpFence fence)
    {
        OnJumpFence();
        // 창틀 뛰어넘기
    }

    void OnJumpFence()
    {
        Animator.SetTrigger("JumpFence");
        m_DOTween.DORestart();
        Debug.Log("Do");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<IInteractableObject>() != null)
            InteractObject(other.GetComponent<IInteractableObject>());
    }

}
