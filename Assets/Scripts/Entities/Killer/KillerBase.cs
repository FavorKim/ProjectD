using Cinemachine;
using Mirror;
using Org.BouncyCastle.Crypto.Signers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KillerBase : NetworkBehaviour, IMoveable, IKillerVisitor
{
    public IKillerInteractable KillerInteractableObject
    {
        get;
        set;
    }

    CharacterController m_controller;
    Animator Animator;
    NetworkAnimator netAnim;
    [SerializeField] BoxCollider m_AttackCollider;
    [SerializeField] Survivor m_holdSurvivor;
    public Survivor HoldSurvivor
    {
        get { return m_holdSurvivor; }
        set
        {
            m_holdSurvivor = value;
            if (value == null)
            {
                HangerManager.Instance.TurnHangersXRay(false);
            }
        }
    }

    [SerializeField] Transform Pos_HoldSurvivor;

    [SerializeField] GameObject Prefab_KillerCam;

    public Transform GetHoldPosition() { return Pos_HoldSurvivor; }

    [SerializeField] AudioSource heartBeat;

    Vector3 m_moveDir;


    [SerializeField] float m_rotateSpeed;
    [SerializeField] float m_attackCool;
    float m_stunLength = 2.0f;
    public float StunLength
    {
        get
        {
            return m_stunLength / StunRecover;
        }
    }
    [SerializeField] float m_stunRecover = 0.0f;
    public float StunRecover
    {
        get
        {
            return 1 + m_stunRecover * 0.01f;
        }
        set
        {
            if (m_stunRecover != value)
            {
                m_stunRecover = value;
            }
        }
    }

    float m_lungeLength;

    [SerializeField] float m_jumpSpeed;
    public float JumpSpeed
    {
        get
        {
            return 1 + (m_jumpSpeed * 0.01f);
        }
        set
        {
            if (m_jumpSpeed != value)
            {
                m_jumpSpeed = value;
            }
        }
    }

    [SerializeField] float m_breakSpeed;
    public float BreakSpeed
    {
        get
        {
            return 1 + (m_breakSpeed * 0.01f);
        }
        set
        {
            if (m_breakSpeed != value)
            {
                m_breakSpeed = value;
            }
        }
    }
    [SerializeField] float m_attackSpeed;
    public float AttackSpeed
    {
        get
        {
            return 1 + (m_attackSpeed * 0.01f);
        }
        set
        {
            if (m_attackSpeed != value)
            {
                m_attackSpeed = value;
            }
        }
    }

    bool isFreeze = false;
    bool IsFreeze
    {
        get { return isFreeze; }
        set
        {
            if (isFreeze != value)
            {
                isFreeze = value;
            }
        }
    }
    bool isAttacking = false;
    bool canAttack = true;
    [SerializeField] bool isStunable = true;
    [SerializeField] bool isStun = false;

    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            if (moveSpeed != value)
                moveSpeed = value;
        }
    }
    [SerializeField] float moveSpeed;
    public bool IsAttacking
    {
        get { return isAttacking; }
        set
        {
            if (value != isAttacking)
            {
                isAttacking = value;
                //m_AttackCollider.enabled = value;
                if (isAttacking == true)
                {
                    IsFreeze = true;
                    Animator.SetTrigger("Attack");
                    netAnim.SetTrigger("Attack");
                }
                else
                {
                    IsFreeze = false;
                    Animator.SetTrigger("AttackSuccess");
                    netAnim.SetTrigger("AttackSuccess");

                    StartCoroutine(CorWeaponColliderSet());
                    StartCoroutine(CorAttackCool());
                    m_lungeLength = 0;
                }
            }
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        GetComponent<PlayerInput>().enabled = true;
        var cam = Instantiate(Prefab_KillerCam).GetComponent<CinemachineVirtualCamera>();
        cam.Follow = transform;
        cam.LookAt = transform;
        PlayerPerkManager.SetKillerPerk(SelectedPerkManager.EquippedPerkList, this);
        InGamePerkSlot.Instance.SetPerkIcons(SelectedPerkManager.EquippedPerkList);
        heartBeat.enabled = false;
        AudioListener listener = GetComponent<AudioListener>();
        listener.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        Animator = GetComponentInChildren<Animator>();
        netAnim = Animator.gameObject.GetComponentInChildren<NetworkAnimator>();
    }

    private void OnEnable()
    {
        OnStun += OnStun_GetHit;
        OnStun += OnStun_RevealSurvivor;
    }
    private void OnDisable()
    {
        OnStun -= OnStun_RevealSurvivor;
        OnStun -= OnStun_GetHit;
        OnStun = null;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (isLocalPlayer)
        {
            
            OnKillerInteract(KillerInteractableObject);
            KillerMove();
            KillerAttack();
            Look();
        }
    }

    void Look()
    {
        if (IsFreeze) return;
        transform.LookAt(Camera.main.transform.forward * 5000f);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
    void KillerMove()
    {
        if (IsFreeze) return;

        //if (m_moveDir == Vector3.zero) return;

        Vector3 goDirection = transform.TransformDirection(m_moveDir);
        //goDirection.Normalize();
        goDirection *= Time.deltaTime * MoveSpeed;
        m_controller.SimpleMove(goDirection);
    }
    void KillerAttack()
    {
        if (isStun) return;

        if (Input.GetMouseButton(0) && canAttack && m_lungeLength < 0.5f)
        {
            IsAttacking = true;
            m_controller.SimpleMove(transform.forward * Time.deltaTime * MoveSpeed * 1.568f);
            m_lungeLength += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0) || m_lungeLength >= 0.5f)
        {
            IsAttacking = false;
        }
    }
    void SetAnimator_HangOrHold()
    {
        StartCoroutine(CorFreezeWhileSec(1.0f));
        Animator.SetTrigger("HangHold");
        netAnim.SetTrigger("HangHold");
    }
    void LookAtDest(Vector3 dest)
    {
        m_controller.Move(dest - transform.position);
        transform.LookAt(dest);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
    void OnJumpFence(Vector3 dest)
    {
        LookAtDest(dest);
        StartCoroutine(CorJumpFence());
    }

    public void OnKillerVisitWith(Generator generator)
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!generator.IsSabotaging)
            {
                Animator.SetTrigger("Break");
                netAnim.SetTrigger("Break");
                StartCoroutine(CorFreezeWhileSec(1.0f / BreakSpeed));
            }
        }
    }
    public void OnKillerVisitWith(JumpFence jumpFence)
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space) && !IsFreeze)
        {
            Vector3 center = jumpFence.transform.position + jumpFence.transform.forward * -1.5f;
            OnJumpFence(center);
            //StartCoroutine(CorFreezeWhileSec(1.0f));
        }
    }
    public void OnKillerVisitWith(Palete palete)
    {
        if (palete.IsAttack && isStunable)
        {
            var dist = (transform.forward - palete.transform.right) / (transform.forward - palete.transform.right).magnitude;
            m_controller.Move(dist * -2.5f);
            OnStun.Invoke();
        }
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (palete.isUsed)
            {
                StartCoroutine(CorFreezeWhileSec(1.5f / BreakSpeed));
                Animator.SetTrigger("Break");
                netAnim.SetTrigger("Break");
            }
            else
            {

            }
        }
    }

    public void OnKillerVisitWith(Hanger hanger)
    {
        if (!isLocalPlayer || HoldSurvivor == null || IsFreeze) return;

        if (Input.GetKeyDown(KeyCode.Space)
            && hanger.IsAvailable())
        {
            SetAnimator_HangOrHold();
            hanger.HangedSurvivor = HoldSurvivor;
            HoldSurvivor.BeingHanged(hanger);
            HoldSurvivor = null;
            StartCoroutine(CorFreezeWhileSec(0.8f));
        }
    }

    public void OnKillerInteract(IKillerInteractable obj)
    {
        obj?.KillerInteract(this);
    }
    /*
    public void OnKillerVisit(IKillerInteractable obj)
    {
        switch (obj)
        {
            case Generator:
                OnInteractWithGenerator(obj as Generator);
                break;
            case JumpFence:
                OnInteractWithJumpFence(obj as JumpFence);
                break;
            case Hanger:
                OnInteractWithHanger(obj as Hanger);
                break;
            case Palete:
                OnInteractWithPalete(obj as Palete);
                break;
        }
    }
    */


    IEnumerator CorAttackCool()
    {
        m_attackCool = (m_lungeLength + 1.0f) / AttackSpeed;
        m_attackCool = m_lungeLength == 0.5f ? 2.7f / AttackSpeed : (1.0f + m_lungeLength) / AttackSpeed;
        canAttack = false;
        IsFreeze = true;
        yield return new WaitForSeconds(m_attackCool);
        canAttack = true;
        IsFreeze = false;
    }
    IEnumerator CorJumpFence()
    {
        float time = 0;
        IsFreeze = true;

        while (time < 0.7f / JumpSpeed)
        {
            time += Time.deltaTime;
            m_controller.Move(transform.up * 1.5f * JumpSpeed * Time.deltaTime);
            m_controller.Move(transform.forward * 2.0f * JumpSpeed * Time.deltaTime);
            yield return null;
        }
        while (time < 1.8f / JumpSpeed)
        {
            time += Time.deltaTime;
            m_controller.Move(transform.up * -1.5f * JumpSpeed * Time.deltaTime);
            m_controller.Move(transform.forward * 2.0f * JumpSpeed * Time.deltaTime);
            yield return null;
        }
        IsFreeze = false;
    }
    IEnumerator CorFreezeWhileSec(float time)
    {
        IsFreeze = true;
        yield return new WaitForSeconds(time);
        IsFreeze = false;
    }
    IEnumerator CorStunCool()
    {
        isStunable = false;
        isStun = true;
        yield return new WaitForSeconds(StunLength);
        Animator.SetTrigger("StunOver");
        netAnim.SetTrigger("StunOver");
        isStunable = true;
        isStun = false;
    }
    IEnumerator CorWeaponColliderSet()
    {
        m_AttackCollider.enabled = true;
        yield return new WaitForSeconds(0.8f);
        m_AttackCollider.enabled = false;
    }

    protected void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out IKillerInteractable obj))
        {
            KillerInteractableObject = obj;
        }
        //var survivor = collision.GetComponent<Survivor>();
        if (collision.TryGetComponent(out Survivor survivor))
        {
            if (IsAttacking)
            {
                IsAttacking = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IKillerInteractable obj))
        {
            KillerInteractableObject = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isLocalPlayer) return;

        if (IsAttacking)
        {
            if (other.CompareTag("Wall"))
            {
                if (other.TryGetComponent(out Palete pal))
                {
                    if (pal.isUsed)
                    {
                        OnStunCall();
                    }
                }
                else
                    OnStunCall();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !IsFreeze)
        {
            if (other.TryGetComponent(out Survivor survivor))
            {
                if (survivor.GetHealthStateMachine().GetCurState() < HealthStates.Hanged)
                {
                    SetAnimator_HangOrHold();
                    HangerManager.Instance.TurnHangersXRay(true);
                    HoldSurvivor = survivor;
                    HoldSurvivor.BeingHeld(this);
                }
            }
        }
    }


    private event Action OnStun;

    [Command(requiresAuthority = false)]
    public void OnStunCall()
    {
        RpcOnStunCall();
    }
    [ClientRpc]
    public void RpcOnStunCall()
    {
        if (isStunable)
            OnStun();
    }

    void OnStun_GetHit()
    {
        //if (!isLocalPlayer) return;
        IsAttacking = false;
        StartCoroutine(CorStunCool());
        StartCoroutine(CorFreezeWhileSec(StunLength));
        m_AttackCollider.enabled = false;
        Animator.SetTrigger("GetHit");
        netAnim.SetTrigger("GetHit");
    }
    void OnStun_RevealSurvivor()
    {
        if (HoldSurvivor != null)
        {
            HoldSurvivor = null;
        }
    }

    public void OnMove(InputValue val)
    {
        if (!isLocalPlayer) return;

        Vector2 dir = val.Get<Vector2>();
        m_moveDir = new Vector3(dir.x, 0, dir.y);


        if (IsFreeze) return;
        if (m_moveDir != Vector3.zero)
        {
            Animator.SetBool("isMoving", true);
        }
        else
        {
            Animator.SetBool("isMoving", false);
        }
        Animator.SetFloat("inputX", m_moveDir.x);
        Animator.SetFloat("inputY", m_moveDir.z);
    }



    void OnEscapedFromKiller_GetStun()
    {
        OnStunCall();
    }

    public void MoveToDirection(Vector3 dir)
    {
        m_controller.SimpleMove(dir);
    }

}
