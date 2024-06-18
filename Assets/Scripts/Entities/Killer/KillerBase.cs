using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerBase : PlayableCharactor
{
    CharacterController m_controller;
    Animator Animator;

    // Start is called before the first frame update
    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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
            Animator.SetTrigger("Break");
            palete.Break();
        }
    }


    protected override void OnTriggerEnter(Collider collision)
    {
        var palete = collision.GetComponent<Palete>();
        if (palete != null && palete.IsAttack)
        {
            OnStun();
        }
    }

    private event Action OnStun;
}
