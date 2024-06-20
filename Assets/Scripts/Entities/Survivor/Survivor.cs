using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 서버에 전달할 요소들 (서버 내의 모든 플레이어가 알아야할 정보 및 순간들)
발전기
 발전기 진척도
 발전기 진척도에 따른 이펙트
 발전기 사보타지 여부
 발전기 완료(에 걸려있는 이벤트들)

판자
 판자 사용됨
 판자 파괴됨

갈고리
 생존자 매달림
 생존자 구출됨
 
 
생존자
 생존자의 부상 여부 (부상/빈사)
 생존자 발자국 (Spawn, 풀링 해야)
 생존자 혈흔 이펙트
 생존자 피격
 생존자 치료됨
 생존자 탈락함
 생존자 탈출함

살인마
 살인마가 생존자를 들어올림
 (살인마 타격 -> X. 살인마의 역할은 애니메이션 재생일 뿐, 생존자가 피격되었는지만 판단하면 됨.)
 

 
 
 */
public class Survivor : PlayableCharactor
{
    CharacterController m_CharacterController;
    [SerializeField] Animator Animator;

    [SerializeField] GameObject VFX_FootPrintPref;
    [SerializeField] GameObject VFX_Bleeding;

    SurvivorStateMachine m_StateMachine;
    SurvivorHealthStateMachine m_healthStateMachine;

    Vector3 MoveDir;
    Vector2 dir;
    public Vector3 HeldPosition { get; private set; }

    [SerializeField] float m_invincibleTime;
    [SerializeField] float m_moveSpeed;
    public float MoveSpeed { get { return m_moveSpeed; } set { m_moveSpeed = value; } }

    private float walkSpeed = 200.0f;
    private float runSpeed = 500.0f;
    private float crouchSpeed = 100.0f;

    [SerializeField] float m_corruptTime = 120;
    public float CorruptTime
    {
        get
        {
            return m_corruptTime;
        }
        private set
        {
            if (m_corruptTime != value)
            {
                m_corruptTime = value;
                PlayerUIManager.Instance.SetPlayerUIGauge(m_playerID, m_corruptTime / 120);
                if (m_corruptTime <= 0)
                {
                    OnSacrificed();
                }
            }
        }
    }
    [SerializeField] float DebugOnly_CorruptMulti;

    public float GetWalkSpeed() { return walkSpeed; }
    public float GetRunSpeed() { return runSpeed; }
    public float GetCrouchSpeed() { return crouchSpeed; }

    [SerializeField] float rotateSpeed;

    bool isFreeze = false;
    public bool IsFreeze { get { return isFreeze; } set { isFreeze = value; } }

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

    public bool isCorrupted = false;
    public bool IsCorrupted
    {
        get
        {
            return isCorrupted;
        }
        set
        {
            if (isCorrupted != value)
            {
                isCorrupted = value;
                if (value == true)
                {
                    CorruptTime = 60.0f;
                }
            }
        }
    }

    int hangedCount = 0;
    public int HangedCount
    {
        get { return hangedCount; }
        private set
        {
            if (hangedCount != value)
            {
                hangedCount = value;
                if (value == 2)
                {
                    IsCorrupted = true;
                }
                if (value == 3)
                {
                    OnSacrificed();
                }
            }
        }
    }




    int m_playerID;
    public int PlayerID() { return m_playerID; }


    public Vector3 GetMoveDir() { return MoveDir; }

    public Animator GetAnimator() { return Animator; }


    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_StateMachine = new SurvivorStateMachine(this);
        m_healthStateMachine = new SurvivorHealthStateMachine(this);
        DOTween.Init();
        m_playerID = PlayerUIManager.Instance.CreatePlayerUI();
    }

    private void OnEnable()
    {
        OnBeingHeld += OnBeingHeld_SetState;
        OnBeingHeld += OnBeingHeld_SetPosition;

        OnBeingHanged += OnBeingHanged_SetState;
        OnBeingHanged += OnBeingHanged_SetPosition;
        OnBeingHanged += OnBeingHanged_SetCorrupt;

        OnSacrificed += OnSacrificed_SetState;
    }
    private void OnDisable()
    {
        m_healthStateMachine.UnRegisterEvent();
        OnHitted = null;


        OnSacrificed -= OnSacrificed_SetState;

        OnBeingHanged -= OnBeingHanged_SetCorrupt;
        OnBeingHanged -= OnBeingHanged_SetPosition;
        OnBeingHanged -= OnBeingHanged_SetState;

        OnBeingHeld -= OnBeingHeld_SetState;
        OnBeingHeld -= OnBeingHeld_SetState;

        OnBeingHanged = null;
        OnBeingHeld = null;
        OnSacrificed = null;
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

        if (isFreeze) return;
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
        }
        m_CharacterController.SimpleMove(MoveDir);

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
        //[TODO] NetworkPool로 바꿀 것
        PootPrintPool.Instance.PrintPootPrint(new Vector3(transform.position.x, 0.001f, transform.position.z), Quaternion.Euler(-90, 0, 0));
    }




    // command와 rpc는 참조형이 아닌 NetworkBehaviour를 상속받은 객체와 구조체만을 매개변수로 사용할 수 있음

    public void BeingHeld(KillerBase holdPos)
    {
        OnBeingHeld(holdPos);

    }
    public void BeingHanged(Hanger hangerPos)
    {
        OnBeingHanged(hangerPos);
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

    void RotateTransformToDest(Vector3 look)
    {
        //transform.rotation.SetLookRotation(look);
        m_CharacterController.Move(look - transform.position);
        transform.LookAt(look);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
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
        }
        if (Input.GetMouseButtonUp(0))
        {
            Animator.SetBool("isGenerating", false);
            isFreeze = false;
        }
        generator.SurvivorInteract();
    }
    public override void Interact(Palete palete)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!palete.isUsed)
            {
                // 판자 내리기 애니메이션
                palete.SurvivorInteract();
            }
            else
            {
                // 판자 넘고, 넘는 애니메이션
                var center = palete.transform.position + palete.transform.right * -1.0f;

                OnJumpFence(center);
                //palete.SurvivorInteract();
            }
        }
    }
    public override void Interact(JumpFence fence)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var center = fence.transform.position + fence.transform.forward * -1.5f;
            OnJumpFence(center);
        }
        // 창틀 뛰어넘기
    }
    public override void Interact(Hanger hanger)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (hanger.HangedSurvivor != null && hanger.HangedSurvivor != this)
            {
                hanger.SurvivorInteract();
                CorFreeze(1.2f);
            }
        }
    }
    public override void Interact(Lever lever)
    {
        if (!lever.IsAvailable) return;

        if (Input.GetMouseButtonDown(0))
        {
            Animator.SetTrigger("Pull");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Animator.SetTrigger("PullOver");
        }
        lever.SurvivorInteract();
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
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            m_CharacterController.Move(transform.up * 3.0f * Time.deltaTime);
            m_CharacterController.Move(transform.forward * 6.0f * Time.deltaTime);
            yield return null;
        }
        while (time < 0.7f)
        {
            time += Time.deltaTime;
            m_CharacterController.Move(transform.up * -5.0f * Time.deltaTime);
            m_CharacterController.Move(transform.forward * 6.0f * Time.deltaTime);
            yield return null;
        }
        //m_CharacterController.Move(-transform.up * 10f);
        isFreeze = false;
    }
    IEnumerator CorCorrupt()
    {

        while (CorruptTime > 0 && m_healthStateMachine.GetCurState() == HealthStates.Hanged)
        {
            CorruptTime -= Time.deltaTime * DebugOnly_CorruptMulti;
            if (CorruptTime < 60.0f) IsCorrupted = true;
            yield return null;
        }
    }
    IEnumerator CorFreeze(float time)
    {
        IsFreeze = true;
        yield return new WaitForSeconds(time);
        IsFreeze = false;
    }


    public event Action OnHitted;
    public event Action<KillerBase> OnBeingHeld;
    public event Action<Hanger> OnBeingHanged;
    public event Action OnSacrificed;

    // 들렸을 때
    void OnBeingHeld_SetState(KillerBase killer)
    {
        if (m_healthStateMachine.GetCurState() == HealthStates.Down)
            m_healthStateMachine.ChangeState(HealthStates.Held);
    }
    void OnBeingHeld_SetPosition(KillerBase killer)
    {
        transform.parent = killer.GetHoldPosition();
        transform.localPosition = Vector3.zero;
    }

    // 걸렸을 때
    void OnBeingHanged_SetState(Hanger hanger)
    {
        if (m_healthStateMachine.GetCurState() == HealthStates.Held)
        {
            m_healthStateMachine.ChangeState(HealthStates.Hanged);
        }
    }
    void OnBeingHanged_SetPosition(Hanger hanger)
    {
        transform.parent = null;
        transform.position = hanger.GetHangedPos().position;
        transform.localRotation = Quaternion.identity;
    }
    void OnBeingHanged_SetCorrupt(Hanger hanger)
    {
        HangedCount++;
        StartCoroutine(CorCorrupt());
    }

    void OnSacrificed_SetState()
    {
        PlayerUIManager.Instance.SetPlayerUIState(m_playerID, PlayerUI.Icons.Killed);
        IsFreeze = true; // 원래는 게임 결과 창으로 이동해야 함
    }

    public void OnResqued()
    {
        m_healthStateMachine.ChangeState(HealthStates.Injured);
        IsFreeze = false;
        m_CharacterController.Move(transform.up * -10f);
        Animator.SetTrigger("Resqued");
    }

    public void OnHealed()
    {

    }


    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (other.TryGetComponent(out Survivor survivor))
            {
                //survivor.
            }

        }
    }


}
