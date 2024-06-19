using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Survivor : PlayableCharactor
{
    CharacterController m_CharacterController;
    [SerializeField] Animator Animator;
    DOTweenAnimation m_DOTween;

    [SerializeField] GameObject VFX_FootPrintPref;
    [SerializeField] GameObject VFX_Bleeding;

    SurvivorStateMachine m_StateMachine;
    SurvivorHealthStateMachine m_healthStateMachine;

    Vector3 MoveDir;
    Vector2 dir;

    [SerializeField] float m_invincibleTime;
    [SerializeField] float m_moveSpeed;
    public float MoveSpeed { get { return m_moveSpeed; } set { m_moveSpeed = value; } }

    private float walkSpeed = 200.0f;
    public float GetWalkSpeed() { return walkSpeed; }
    private float runSpeed = 500.0f;
    public float GetRunSpeed() { return runSpeed; }
    private float crouchSpeed = 100.0f;
    public float GetCrouchSpeed() { return crouchSpeed; }


    [SerializeField] float rotateSpeed;

    bool isFreeze = false;
    private bool isBleeding = false;
    public bool IsBleeding
    {
        get { return isBleeding; }
        set
        {
            if (isBleeding != value)
                isBleeding = value;
            if (isBleeding)
            {
                VFX_Bleeding.SetActive(true);
            }
            else
            {
                VFX_Bleeding.SetActive(false);
            }
        }
    }
    bool isInvincible = false;



    public Vector3 GetMoveDir() { return MoveDir; }

    public Animator GetAnimator() { return Animator; }

    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_StateMachine = new SurvivorStateMachine(this);
        m_healthStateMachine = new SurvivorHealthStateMachine(this);
        DOTween.Init();
        m_DOTween = GetComponent<DOTweenAnimation>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // 로컬체크
        base.Update();
        PlayerMove();
        m_healthStateMachine.StateUpdate();

        if (m_healthStateMachine.GetCurState() != HealthStates.Down)
            m_StateMachine.StateUpdate();
    }


    void OnMove(InputValue val)
    {
        // 로컬체크

        //if (isFreeze) return;


        dir = val.Get<Vector2>();
        MoveDir = dir.y * Camera.main.transform.forward + dir.x * Camera.main.transform.right;
        MoveDir = new Vector3(MoveDir.x, 0, MoveDir.z);
        MoveDir.Normalize();
        MoveDir *= m_moveSpeed * Time.deltaTime;


        if (MoveDir != Vector3.zero) Animator.SetBool("isWalk", true);
        else Animator.SetBool("isWalk", false);
    }


    void PlayerMove()
    {
        if (isFreeze) return;


        if (MoveDir != Vector3.zero)
        {
            MoveDir = dir.y * Camera.main.transform.forward + dir.x * Camera.main.transform.right;
            MoveDir = new Vector3(MoveDir.x, 0, MoveDir.z);
            MoveDir.Normalize();
            MoveDir *= m_moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MoveDir), rotateSpeed * Time.deltaTime);
            m_CharacterController.SimpleMove(MoveDir);
        }
    }

    void OnJumpFence(Vector3 dest)
    {
        RotateTransformToDest(dest);

        m_StateMachine.ChangeState(SurvivorStateMachine.StateName.Walk);
        Animator.SetTrigger("JumpFence");
        StartCoroutine(CorJumpFence());
    }
    void PrintFoot()
    {
        //서버 작업 필
        var obj = Instantiate(VFX_FootPrintPref, new Vector3(transform.position.x, 0.001f, transform.position.z), Quaternion.Euler(-90, 0, 0));
    }
    void RotateTransformToDest(Transform dest)
    {
        transform.LookAt(dest);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
    void RotateTransformToDest(Vector3 look)
    {
        transform.rotation.SetLookRotation(look);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }


    public void GetHit()
    {
        if (m_healthStateMachine.GetCurState() != HealthStates.Down && !isInvincible)
        {
            OnHitted();
            StartCoroutine(CorSprint());
            StartCoroutine(CorInvincibleTime());
        }
    }


    public override void Interact(Generator generator)
    {
        

        if (Input.GetMouseButtonDown(0))
        {
            Animator.SetTrigger("Generate");
            Animator.SetBool("isGenerating", true);
        }
        if (Input.GetMouseButton(0))
        {
            if (generator.IsCompleted)
            {
                isFreeze = false;
                Animator.SetBool("isGenerating", false);
                return;
            }
            isFreeze = true;
            generator.Interact();
        }
        if (Input.GetMouseButtonUp(0))
        {
            generator.Interact();
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
                var center = palete.transform.position + palete.transform.right * -1.0f;

                OnJumpFence(center);
                //palete.Interact();
            }
        }
    }
    public override void Interact(JumpFence fence)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var center = fence.transform.position + fence.transform.right ;
            OnJumpFence(center);
        }
        // 창틀 뛰어넘기
    }


    public IEnumerator CorPrintFoot()
    {
        while (m_StateMachine.CurStateIs(SurvivorStateMachine.StateName.Run))
        {
            PrintFoot();
            yield return new WaitForSeconds(1.0f);
        }
    }
    IEnumerator CorSprint()
    {
        runSpeed += 200.0f;
        yield return new WaitForSeconds(1.5f);
        runSpeed -= 200.0f;
    }
    IEnumerator CorInvincibleTime()
    {
        isInvincible = true;
        yield return new WaitForSeconds(m_invincibleTime);
        isInvincible = false;
    }
    IEnumerator CorJumpFence()
    {
        float time = 0;
        isFreeze = true;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            m_CharacterController.Move(transform.up * 3.0f * Time.deltaTime);
            m_CharacterController.Move(transform.forward * 6.0f * Time.deltaTime);
            yield return null;
        }
        m_CharacterController.Move(-transform.up * 10f);
        isFreeze = false;
    }


    public event Action OnHitted;

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.GetComponent<IInteractableObject>() != null)
    //        InteractObject(other.GetComponent<IInteractableObject>());
    //}


}
