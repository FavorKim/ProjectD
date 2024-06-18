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
    bool isFreeze = false;
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
                    isFreeze = true;
                    Animator.SetTrigger("Attack");
                }
                else
                {
                    isFreeze = false;
                    Animator.SetTrigger("AttackSuccess");
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
        Vector3 goDriection = transform.TransformDirection(m_moveDir);
        m_controller.SimpleMove(goDriection * Time.deltaTime*m_moveSpeed);
    }
    void KillerAttack()
    {
        if (Input.GetMouseButton(0) && canAttack)
        {
            IsAttacking = true;
            m_controller.SimpleMove(transform.forward * Time.deltaTime * m_moveSpeed * 1.3f);
        }
        if (Input.GetMouseButtonUp(0))
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



    IEnumerator CorKillColl()
    {
        canAttack = false;
        yield return new WaitForSeconds(m_attackCool);
        canAttack = true;
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        var palete = collision.GetComponent<Palete>();
        if (palete != null && palete.IsAttack)
        {
            OnStun();
        }

        var player = collision.GetComponent<Survivor>();
        if (player != null && isAttacking)
        {
            Debug.Log("hi");
            player.GetHit();
            IsAttacking = false;
            StartCoroutine(CorKillColl());
        }


    }

    private event Action OnStun;
    void OnStun_GetHit()
    {
        Animator.SetTrigger("GetHit");
        //isFreeze = true;
    }

    public void OnMove(InputValue val)
    {
        if (isFreeze)
        {
            m_moveDir = Vector3.zero;
            return;
        }

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
