using Cinemachine;
using Mirror;
using Org.BouncyCastle.Crypto.Signers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KillerBase : PlayableCharactor
{
    CharacterController m_controller;
    Animator Animator;
    NetworkAnimator netAnim;
    [SerializeField] BoxCollider m_AttackCollider;
    Survivor m_holdSurvivor;
    [SerializeField] Transform Pos_HoldSurvivor;

    [SerializeField] GameObject Prefab_KillerCam;

    public Transform GetHoldPosition() { return Pos_HoldSurvivor; }

    Vector3 m_moveDir;

    [SerializeField] float m_moveSpeed;
    [SerializeField] float m_rotateSpeed;
    [SerializeField] float m_attackCool;
    [SerializeField] float m_stunLength;
    public float StunLength
    {
        get
        {
            return m_stunLength / StunRecoverPer;
        }
    }
    [SerializeField] float m_stunRecoverPer = 0.0f;
    public float StunRecoverPer
    {
        get
        {
            return 1 + m_stunRecoverPer * 0.01f;
        }
    }

    float m_lungeLength;

    [SerializeField] float m_actionSpeedPer;
    public float ActionSpeed
    {
        get
        {
            return 1 + (m_actionSpeedPer * 0.01f);
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
    bool isStunable = true;


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
    }

    // Start is called before the first frame update
    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        Animator = GetComponentInChildren<Animator>();
        netAnim = Animator.gameObject.GetComponent<NetworkAnimator>();
        //m_AttackCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        OnStun += OnStun_GetHit;
    }
    private void OnDisable()
    {
        OnStun -= OnStun_GetHit;
        OnStun = null;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isLocalPlayer)
        {
            base.Update();
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
        goDirection *= Time.deltaTime * m_moveSpeed;
        m_controller.SimpleMove(goDirection);
    }
    void KillerAttack()
    {
        if (Input.GetMouseButton(0) && canAttack && m_lungeLength < 1.2f)
        {
            IsAttacking = true;
            m_controller.SimpleMove(transform.forward * Time.deltaTime * m_moveSpeed * 1.3f);
            m_lungeLength += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0) || m_lungeLength >= 1.2f)
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

    public override void Interact(Generator generator)
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!generator.IsSabotaging)
            {
                Animator.SetTrigger("Break");
                netAnim.SetTrigger("Break");
                generator.KillerInteract();
            }
        }
    }
    public override void Interact(JumpFence jumpFence)
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space) && !IsFreeze)
        {
            StartCoroutine(CorJumpFence());
            //StartCoroutine(CorFreezeWhileSec(1.0f));
        }
    }
    public override void Interact(Palete palete)
    {

        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (palete.isUsed)
            {
                StartCoroutine(CorFreezeWhileSec(1.5f));
                Animator.SetTrigger("Break");
                netAnim.SetTrigger("Break");
                palete.KillerInteract();
            }
            else
            {

            }
        }
    }
    public override void Interact(Hanger hanger)
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space) && m_holdSurvivor != null && hanger.IsAvailable())
        {
            HangerManager.Instance.TurnHangersXRay(false);
            SetAnimator_HangOrHold();
            hanger.HangedSurvivor = m_holdSurvivor;
            m_holdSurvivor.BeingHanged(hanger);
            hanger.KillerInteract();
        }
    }
    public override void Interact(Lever lever)
    {

    }



    IEnumerator CorAttackCool()
    {
        m_attackCool = m_lungeLength + 0.8f;
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
        while (time < 0.7f / ActionSpeed)
        {
            time += Time.deltaTime;
            m_controller.Move(transform.up * 1.5f * ActionSpeed * Time.deltaTime);
            m_controller.Move(transform.forward * 2.0f * ActionSpeed * Time.deltaTime);
            yield return null;
        }
        while (time < 1.8f / ActionSpeed)
        {
            time += Time.deltaTime;
            m_controller.Move(transform.up * -1.5f * ActionSpeed * Time.deltaTime);
            m_controller.Move(transform.forward * 2.0f * ActionSpeed * Time.deltaTime);
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
        yield return new WaitForSeconds(StunLength);
        Animator.SetTrigger("StunOver");
        netAnim.SetTrigger("StunOver");
        isStunable = true;
    }
    IEnumerator CorWeaponColliderSet()
    {
        m_AttackCollider.enabled = true;
        yield return new WaitForSeconds(0.8f);
        m_AttackCollider.enabled = false;
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        base.OnTriggerEnter(collision);


        //var survivor = collision.GetComponent<Survivor>();
        if (collision.TryGetComponent(out Survivor survivor))
        {
            if (IsAttacking)
            {
                //survivor.CmdGetHit();
                IsAttacking = false;
            }


        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (other.TryGetComponent(out Survivor survivor))
            {
                SetAnimator_HangOrHold();
                m_holdSurvivor = survivor;
                m_holdSurvivor.BeingHeld(this);
                HangerManager.Instance.TurnHangersXRay(true);
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
    public void RpcOnStunCall() { OnStun(); }

    void OnStun_GetHit()
    {

        if (!isStunable) return;
        Animator.SetTrigger("GetHit");
        netAnim.SetTrigger("GetHit");
        StartCoroutine(CorFreezeWhileSec(StunLength));
        StartCoroutine(CorStunCool());
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


}
