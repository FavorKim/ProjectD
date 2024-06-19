using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KillerBase : PlayableCharactor
{
    CharacterController m_controller;
    Animator Animator;
    BoxCollider m_AttackCollider;

    Vector3 m_moveDir;

    [SerializeField] float m_moveSpeed;
    [SerializeField] float m_rotateSpeed;
    [SerializeField] float m_attackCool;
    float m_lungeLength;
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


    public bool IsAttacking
    {
        get { return IsAttacking; }
        set
        {
            if (value != isAttacking)
            {
                isAttacking = value;
                m_AttackCollider.enabled = value;
                if (isAttacking == true)
                {
                    IsFreeze = true;
                    Animator.SetTrigger("Attack");
                }
                else
                {
                    IsFreeze = false;
                    Animator.SetTrigger("AttackSuccess");
                    StartCoroutine(CorAttackCool());
                    m_lungeLength = 0;
                }
            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        Animator = GetComponentInChildren<Animator>();
        m_AttackCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        KillerMove();
        KillerAttack();
        Look();
    }

    void Look()
    {
        transform.LookAt(Camera.main.transform.forward * 5000f);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    void KillerMove()
    {
        if (!isFreeze)
        {
            Vector3 goDriection = transform.TransformDirection(m_moveDir);
            m_controller.SimpleMove(goDriection * Time.deltaTime * m_moveSpeed);
        }
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


    public override void Interact(Generator generator)
    {
        if (Input.GetKeyDown(KeyCode.Space))
            generator.Sabotage();
    }
    public override void Interact(JumpFence jumpFence)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CorJumpFence());
        }
    }
    public override void Interact(Palete palete)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (palete.isUsed)
            {
                Animator.SetTrigger("Break");
                palete.Break();
            }
            else
            {

            }
        }
    }



    IEnumerator CorAttackCool()
    {
        m_attackCool = m_lungeLength;
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
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            m_controller.Move(transform.up * 1.5f * Time.deltaTime);
            m_controller.Move(transform.forward *2.5f * Time.deltaTime);
            yield return null;
        }
        m_controller.Move(-transform.up * 10f);
        isFreeze = false;
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        base.OnTriggerEnter(collision);
        var palete = collision.GetComponent<Palete>();
        if (palete != null && palete.IsAttack)
        {
            OnStun();
        }

        var player = collision.GetComponent<Survivor>();
        if (player != null && isAttacking)
        {
            player.GetHit();
            IsAttacking = false;
        }


    }

    private event Action OnStun;
    void OnStun_GetHit()
    {
        Animator.SetTrigger("GetHit");
        //IsFreeze = true;
    }

    public void OnMove(InputValue val)
    {
        Vector2 dir = val.Get<Vector2>();
        m_moveDir = new Vector3(dir.x, 0, dir.y);

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
